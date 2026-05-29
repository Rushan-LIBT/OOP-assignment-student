using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;

namespace WareHouseApp.Data
{
    /// <summary>
    /// Records stock in/out movements and keeps the material quantity in sync.
    /// The quantity update and the movement insert run in one transaction.
    /// </summary>
    public class StockMovementRepository
    {
        public const string In = "IN";
        public const string Out = "OUT";

        private readonly DatabaseManager db = DatabaseManager.Instance;

        public void RecordMovement(int materialId, string type, int quantity, string movedBy)
        {
            if (quantity <= 0)
            {
                throw new ValidationException("Quantity must be greater than zero.");
            }

            using (SqlConnection connection = db.GetConnection())
            {
                connection.Open();
                using (SqlTransaction tx = connection.BeginTransaction())
                {
                    try
                    {
                        int current = GetCurrentQuantity(connection, tx, materialId);
                        int newQuantity = type == In ? current + quantity : current - quantity;
                        if (newQuantity < 0)
                        {
                            throw new ValidationException("Not enough stock to ship out.");
                        }

                        Execute(connection, tx,
                            "UPDATE Materials SET Quantity = @qty WHERE MaterialID = @id",
                            new SqlParameter("@qty", newQuantity),
                            new SqlParameter("@id", materialId));

                        Execute(connection, tx,
                            "INSERT INTO StockMovements (MaterialID, MovementType, Quantity, MovedBy) " +
                            "VALUES (@id, @type, @qty, @by)",
                            new SqlParameter("@id", materialId),
                            new SqlParameter("@type", type),
                            new SqlParameter("@qty", quantity),
                            new SqlParameter("@by", (object)movedBy ?? DBNull.Value));

                        tx.Commit();
                    }
                    catch (ValidationException)
                    {
                        tx.Rollback();
                        throw;
                    }
                    catch (SqlException ex)
                    {
                        tx.Rollback();
                        throw new DataAccessException("Stock movement failed.", ex);
                    }
                }
            }
        }

        public List<StockMovement> GetRecent(int top)
        {
            var list = new List<StockMovement>();
            DataTable table = db.GetDataTable(
                "SELECT TOP (@top) m.MovementID, m.MaterialID, mat.MaterialName, " +
                "m.MovementType, m.Quantity, m.MovedBy, m.MovedAt " +
                "FROM StockMovements m " +
                "LEFT JOIN Materials mat ON m.MaterialID = mat.MaterialID " +
                "ORDER BY m.MovedAt DESC, m.MovementID DESC",
                new SqlParameter("@top", top));

            foreach (DataRow row in table.Rows)
            {
                list.Add(new StockMovement
                {
                    MovementID = (int)row["MovementID"],
                    MaterialID = (int)row["MaterialID"],
                    MaterialName = row["MaterialName"] == DBNull.Value ? "(deleted)" : row["MaterialName"].ToString(),
                    MovementType = row["MovementType"].ToString(),
                    Quantity = (int)row["Quantity"],
                    MovedBy = row["MovedBy"] == DBNull.Value ? null : row["MovedBy"].ToString(),
                    MovedAt = (DateTime)row["MovedAt"]
                });
            }
            return list;
        }

        private static int GetCurrentQuantity(SqlConnection connection, SqlTransaction tx, int materialId)
        {
            using (var command = new SqlCommand(
                "SELECT Quantity FROM Materials WHERE MaterialID = @id", connection, tx))
            {
                command.Parameters.AddWithValue("@id", materialId);
                object result = command.ExecuteScalar();
                if (result == null)
                {
                    throw new ValidationException("The selected material no longer exists.");
                }
                return (int)result;
            }
        }

        private static void Execute(SqlConnection connection, SqlTransaction tx, string sql,
            params SqlParameter[] parameters)
        {
            using (var command = new SqlCommand(sql, connection, tx))
            {
                command.Parameters.AddRange(parameters);
                command.ExecuteNonQuery();
            }
        }
    }
}
