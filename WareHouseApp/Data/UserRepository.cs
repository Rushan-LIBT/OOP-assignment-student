using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;
using WareHouseApp.Security;

namespace WareHouseApp.Data
{
    /// <summary>
    /// Handles account registration and authentication against the Users table.
    /// </summary>
    public class UserRepository : BaseRepository<User>
    {
        public override List<User> GetAll()
        {
            var list = new List<User>();
            DataTable table = Db.GetDataTable(
                "SELECT UserID, Username, FullName, Role, CreatedAt FROM Users ORDER BY Username");
            foreach (DataRow row in table.Rows)
            {
                list.Add(Map(row));
            }
            return list;
        }

        public override User GetById(int id)
        {
            DataTable table = Db.GetDataTable(
                "SELECT UserID, Username, FullName, Role, CreatedAt FROM Users WHERE UserID = @id",
                new SqlParameter("@id", id));
            return table.Rows.Count == 0 ? null : Map(table.Rows[0]);
        }

        public override void Add(User entity)
        {
            // Registration goes through Register so a password can be supplied.
            Register(entity.Username, "changeme", entity.FullName, entity.Role);
        }

        public override void Update(User entity)
        {
            Db.ExecuteNonQuery(
                "UPDATE Users SET FullName = @fullName, Role = @role WHERE UserID = @id",
                new SqlParameter("@fullName", (object)entity.FullName ?? System.DBNull.Value),
                new SqlParameter("@role", entity.Role),
                new SqlParameter("@id", entity.UserID));
        }

        public override void Delete(int id)
        {
            Db.ExecuteNonQuery("DELETE FROM Users WHERE UserID = @id", new SqlParameter("@id", id));
        }

        /// <summary>Creates a new account. Throws if the username already exists.</summary>
        public void Register(string username, string password, string fullName, string role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new ValidationException("Username and password are required.");
            }
            if (password.Length < 6)
            {
                throw new ValidationException("Password must be at least 6 characters long.");
            }

            var exists = (int)Db.ExecuteScalar(
                "SELECT COUNT(*) FROM Users WHERE Username = @username",
                new SqlParameter("@username", username));
            if (exists > 0)
            {
                throw new ValidationException("That username is already taken.");
            }

            Db.ExecuteNonQuery(
                "INSERT INTO Users (Username, PasswordHash, FullName, Role) " +
                "VALUES (@username, @hash, @fullName, @role)",
                new SqlParameter("@username", username),
                new SqlParameter("@hash", PasswordHasher.Hash(password)),
                new SqlParameter("@fullName", (object)fullName ?? System.DBNull.Value),
                new SqlParameter("@role", string.IsNullOrWhiteSpace(role) ? "Operator" : role));
        }

        /// <summary>Returns the matching user or throws <see cref="AuthenticationException"/>.</summary>
        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new AuthenticationException("Please enter both username and password.");
            }

            DataTable table = Db.GetDataTable(
                "SELECT UserID, Username, FullName, Role, CreatedAt, PasswordHash FROM Users WHERE Username = @username",
                new SqlParameter("@username", username));

            if (table.Rows.Count == 0)
            {
                throw new AuthenticationException("Username or password is incorrect.");
            }

            string storedHash = table.Rows[0]["PasswordHash"].ToString();
            if (!PasswordHasher.Verify(password, storedHash))
            {
                throw new AuthenticationException("Username or password is incorrect.");
            }

            return Map(table.Rows[0]);
        }

        /// <summary>Verifies the current password and stores the new one (hashed).</summary>
        public void ChangePassword(int userId, string currentPassword, string newPassword)
        {
            DataTable table = Db.GetDataTable(
                "SELECT PasswordHash FROM Users WHERE UserID = @id",
                new SqlParameter("@id", userId));

            if (table.Rows.Count == 0)
            {
                throw new ValidationException("User account not found.");
            }
            if (!PasswordHasher.Verify(currentPassword, table.Rows[0]["PasswordHash"].ToString()))
            {
                throw new ValidationException("Current password is incorrect.");
            }
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                throw new ValidationException("New password must be at least 6 characters long.");
            }

            Db.ExecuteNonQuery(
                "UPDATE Users SET PasswordHash = @hash WHERE UserID = @id",
                new SqlParameter("@hash", PasswordHasher.Hash(newPassword)),
                new SqlParameter("@id", userId));
        }

        private static User Map(DataRow row)
        {
            return new User
            {
                UserID = (int)row["UserID"],
                Username = row["Username"].ToString(),
                FullName = row["FullName"] == System.DBNull.Value ? null : row["FullName"].ToString(),
                Role = row["Role"].ToString(),
                CreatedAt = row["CreatedAt"] == System.DBNull.Value
                    ? System.DateTime.MinValue
                    : (System.DateTime)row["CreatedAt"]
            };
        }
    }
}
