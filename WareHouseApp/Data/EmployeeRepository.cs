using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;

namespace WareHouseApp.Data
{
    /// <summary>CRUD access to the Employees table.</summary>
    public class EmployeeRepository : BaseRepository<Employee>
    {
        public override List<Employee> GetAll()
        {
            var list = new List<Employee>();
            DataTable table = Db.GetDataTable(
                "SELECT EmployeeID, Name, Role, Email, Phone, Salary FROM Employees ORDER BY Name");
            foreach (DataRow row in table.Rows)
            {
                list.Add(Map(row));
            }
            return list;
        }

        public List<Employee> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return GetAll();
            }

            var list = new List<Employee>();
            DataTable table = Db.GetDataTable(
                "SELECT EmployeeID, Name, Role, Email, Phone, Salary FROM Employees " +
                "WHERE Name LIKE @term OR Role LIKE @term OR Email LIKE @term ORDER BY Name",
                new SqlParameter("@term", "%" + term + "%"));
            foreach (DataRow row in table.Rows)
            {
                list.Add(Map(row));
            }
            return list;
        }

        public override Employee GetById(int id)
        {
            DataTable table = Db.GetDataTable(
                "SELECT EmployeeID, Name, Role, Email, Phone, Salary FROM Employees WHERE EmployeeID = @id",
                new SqlParameter("@id", id));
            return table.Rows.Count == 0 ? null : Map(table.Rows[0]);
        }

        public override void Add(Employee entity)
        {
            Validate(entity);
            Db.ExecuteNonQuery(
                "INSERT INTO Employees (Name, Role, Email, Phone, Salary) " +
                "VALUES (@name, @role, @email, @phone, @salary)",
                new SqlParameter("@name", entity.Name),
                new SqlParameter("@role", (object)entity.Role ?? System.DBNull.Value),
                new SqlParameter("@email", (object)entity.Email ?? System.DBNull.Value),
                new SqlParameter("@phone", (object)entity.Phone ?? System.DBNull.Value),
                new SqlParameter("@salary", entity.Salary));
        }

        public override void Update(Employee entity)
        {
            Validate(entity);
            Db.ExecuteNonQuery(
                "UPDATE Employees SET Name = @name, Role = @role, Email = @email, " +
                "Phone = @phone, Salary = @salary WHERE EmployeeID = @id",
                new SqlParameter("@name", entity.Name),
                new SqlParameter("@role", (object)entity.Role ?? System.DBNull.Value),
                new SqlParameter("@email", (object)entity.Email ?? System.DBNull.Value),
                new SqlParameter("@phone", (object)entity.Phone ?? System.DBNull.Value),
                new SqlParameter("@salary", entity.Salary),
                new SqlParameter("@id", entity.EmployeeID));
        }

        public override void Delete(int id)
        {
            Db.ExecuteNonQuery("DELETE FROM Employees WHERE EmployeeID = @id", new SqlParameter("@id", id));
        }

        private static void Validate(Employee entity)
        {
            if (entity == null || string.IsNullOrWhiteSpace(entity.Name))
            {
                throw new ValidationException("Employee name is required.");
            }
            if (entity.Salary < 0)
            {
                throw new ValidationException("Salary cannot be negative.");
            }
        }

        private static Employee Map(DataRow row)
        {
            return new Employee
            {
                EmployeeID = (int)row["EmployeeID"],
                Name = row["Name"].ToString(),
                Role = row["Role"] == System.DBNull.Value ? null : row["Role"].ToString(),
                Email = row["Email"] == System.DBNull.Value ? null : row["Email"].ToString(),
                Phone = row["Phone"] == System.DBNull.Value ? null : row["Phone"].ToString(),
                Salary = (decimal)row["Salary"]
            };
        }
    }
}
