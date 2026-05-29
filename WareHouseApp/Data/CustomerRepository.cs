using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;

namespace WareHouseApp.Data
{
    /// <summary>CRUD access to the Customers table.</summary>
    public class CustomerRepository : BaseRepository<Customer>
    {
        public override List<Customer> GetAll()
        {
            var list = new List<Customer>();
            DataTable table = Db.GetDataTable(
                "SELECT CustomerID, Name, Email, Phone, Address FROM Customers ORDER BY Name");
            foreach (DataRow row in table.Rows)
            {
                list.Add(Map(row));
            }
            return list;
        }

        public List<Customer> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return GetAll();
            }

            var list = new List<Customer>();
            DataTable table = Db.GetDataTable(
                "SELECT CustomerID, Name, Email, Phone, Address FROM Customers " +
                "WHERE Name LIKE @term OR Email LIKE @term OR Phone LIKE @term ORDER BY Name",
                new SqlParameter("@term", "%" + term + "%"));
            foreach (DataRow row in table.Rows)
            {
                list.Add(Map(row));
            }
            return list;
        }

        public override Customer GetById(int id)
        {
            DataTable table = Db.GetDataTable(
                "SELECT CustomerID, Name, Email, Phone, Address FROM Customers WHERE CustomerID = @id",
                new SqlParameter("@id", id));
            return table.Rows.Count == 0 ? null : Map(table.Rows[0]);
        }

        public override void Add(Customer entity)
        {
            Validate(entity);
            Db.ExecuteNonQuery(
                "INSERT INTO Customers (Name, Email, Phone, Address) VALUES (@name, @email, @phone, @address)",
                new SqlParameter("@name", entity.Name),
                new SqlParameter("@email", (object)entity.Email ?? System.DBNull.Value),
                new SqlParameter("@phone", (object)entity.Phone ?? System.DBNull.Value),
                new SqlParameter("@address", (object)entity.Address ?? System.DBNull.Value));
        }

        public override void Update(Customer entity)
        {
            Validate(entity);
            Db.ExecuteNonQuery(
                "UPDATE Customers SET Name = @name, Email = @email, Phone = @phone, Address = @address " +
                "WHERE CustomerID = @id",
                new SqlParameter("@name", entity.Name),
                new SqlParameter("@email", (object)entity.Email ?? System.DBNull.Value),
                new SqlParameter("@phone", (object)entity.Phone ?? System.DBNull.Value),
                new SqlParameter("@address", (object)entity.Address ?? System.DBNull.Value),
                new SqlParameter("@id", entity.CustomerID));
        }

        public override void Delete(int id)
        {
            Db.ExecuteNonQuery("DELETE FROM Customers WHERE CustomerID = @id", new SqlParameter("@id", id));
        }

        private static void Validate(Customer entity)
        {
            if (entity == null || string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new ValidationException("Customer name is required.");
            }
        }

        private static Customer Map(DataRow row)
        {
            return new Customer
            {
                CustomerID = (int)row["CustomerID"],
                Name = row["Name"].ToString(),
                Email = row["Email"] == System.DBNull.Value ? null : row["Email"].ToString(),
                Phone = row["Phone"] == System.DBNull.Value ? null : row["Phone"].ToString(),
                Address = row["Address"] == System.DBNull.Value ? null : row["Address"].ToString()
            };
        }
    }
}
