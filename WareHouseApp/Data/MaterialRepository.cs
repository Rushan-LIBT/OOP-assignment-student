using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;

namespace WareHouseApp.Data
{
    /// <summary>CRUD access to the Materials (inventory) table.</summary>
    public class MaterialRepository : BaseRepository<Material>
    {
        public override List<Material> GetAll()
        {
            var list = new List<Material>();
            DataTable table = Db.GetDataTable(
                "SELECT MaterialID, MaterialName, Category, Quantity, UnitPrice FROM Materials ORDER BY MaterialName");
            foreach (DataRow row in table.Rows)
            {
                list.Add(Map(row));
            }
            return list;
        }

        public List<Material> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return GetAll();
            }

            var list = new List<Material>();
            DataTable table = Db.GetDataTable(
                "SELECT MaterialID, MaterialName, Category, Quantity, UnitPrice FROM Materials " +
                "WHERE MaterialName LIKE @term OR Category LIKE @term ORDER BY MaterialName",
                new SqlParameter("@term", "%" + term + "%"));
            foreach (DataRow row in table.Rows)
            {
                list.Add(Map(row));
            }
            return list;
        }

        public override Material GetById(int id)
        {
            DataTable table = Db.GetDataTable(
                "SELECT MaterialID, MaterialName, Category, Quantity, UnitPrice FROM Materials WHERE MaterialID = @id",
                new SqlParameter("@id", id));
            return table.Rows.Count == 0 ? null : Map(table.Rows[0]);
        }

        public override void Add(Material entity)
        {
            Validate(entity);
            Db.ExecuteNonQuery(
                "INSERT INTO Materials (MaterialName, Category, Quantity, UnitPrice) " +
                "VALUES (@name, @category, @quantity, @price)",
                new SqlParameter("@name", entity.MaterialName),
                new SqlParameter("@category", (object)entity.Category ?? System.DBNull.Value),
                new SqlParameter("@quantity", entity.Quantity),
                new SqlParameter("@price", entity.UnitPrice));
        }

        public override void Update(Material entity)
        {
            Validate(entity);
            Db.ExecuteNonQuery(
                "UPDATE Materials SET MaterialName = @name, Category = @category, " +
                "Quantity = @quantity, UnitPrice = @price WHERE MaterialID = @id",
                new SqlParameter("@name", entity.MaterialName),
                new SqlParameter("@category", (object)entity.Category ?? System.DBNull.Value),
                new SqlParameter("@quantity", entity.Quantity),
                new SqlParameter("@price", entity.UnitPrice),
                new SqlParameter("@id", entity.MaterialID));
        }

        public override void Delete(int id)
        {
            Db.ExecuteNonQuery("DELETE FROM Materials WHERE MaterialID = @id", new SqlParameter("@id", id));
        }

        private static void Validate(Material entity)
        {
            if (entity == null || string.IsNullOrWhiteSpace(entity.MaterialName))
            {
                throw new ValidationException("Material name is required.");
            }
            if (entity.Quantity < 0)
            {
                throw new ValidationException("Quantity cannot be negative.");
            }
            if (entity.UnitPrice < 0)
            {
                throw new ValidationException("Unit price cannot be negative.");
            }
        }

        private static Material Map(DataRow row)
        {
            return new Material
            {
                MaterialID = (int)row["MaterialID"],
                MaterialName = row["MaterialName"].ToString(),
                Category = row["Category"] == System.DBNull.Value ? null : row["Category"].ToString(),
                Quantity = (int)row["Quantity"],
                UnitPrice = (decimal)row["UnitPrice"]
            };
        }
    }
}
