using System.Data.SqlClient;
using WareHouseApp.Security;

namespace WareHouseApp.Data
{
    /// <summary>
    /// Creates the required tables on first run and seeds a default admin account.
    /// Idempotent: safe to call every time the application starts.
    /// </summary>
    public static class DatabaseInitializer
    {
        public static void Initialize()
        {
            var db = DatabaseManager.Instance;

            db.ExecuteNonQuery(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Users' AND xtype = 'U')
                CREATE TABLE Users (
                    UserID INT IDENTITY(1,1) PRIMARY KEY,
                    Username NVARCHAR(50) NOT NULL UNIQUE,
                    PasswordHash NVARCHAR(128) NOT NULL,
                    FullName NVARCHAR(100) NULL,
                    Role NVARCHAR(20) NOT NULL,
                    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
                );");

            db.ExecuteNonQuery(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Materials' AND xtype = 'U')
                CREATE TABLE Materials (
                    MaterialID INT IDENTITY(1,1) PRIMARY KEY,
                    MaterialName NVARCHAR(100) NOT NULL,
                    Category NVARCHAR(50) NULL,
                    Quantity INT NOT NULL DEFAULT 0,
                    UnitPrice DECIMAL(18,2) NOT NULL DEFAULT 0
                );");

            db.ExecuteNonQuery(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Customers' AND xtype = 'U')
                CREATE TABLE Customers (
                    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    Email NVARCHAR(100) NULL,
                    Phone NVARCHAR(30) NULL,
                    Address NVARCHAR(200) NULL
                );");

            db.ExecuteNonQuery(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Employees' AND xtype = 'U')
                CREATE TABLE Employees (
                    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    Role NVARCHAR(50) NULL,
                    Email NVARCHAR(100) NULL,
                    Phone NVARCHAR(30) NULL,
                    Salary DECIMAL(18,2) NOT NULL DEFAULT 0
                );");

            db.ExecuteNonQuery(@"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'StockMovements' AND xtype = 'U')
                CREATE TABLE StockMovements (
                    MovementID INT IDENTITY(1,1) PRIMARY KEY,
                    MaterialID INT NOT NULL,
                    MovementType NVARCHAR(3) NOT NULL,
                    Quantity INT NOT NULL,
                    MovedBy NVARCHAR(50) NULL,
                    MovedAt DATETIME NOT NULL DEFAULT GETDATE()
                );");

            SeedAdmin(db);
        }

        private static void SeedAdmin(DatabaseManager db)
        {
            var count = (int)db.ExecuteScalar(
                "SELECT COUNT(*) FROM Users WHERE Username = @username",
                new SqlParameter("@username", "admin"));

            if (count == 0)
            {
                db.ExecuteNonQuery(
                    "INSERT INTO Users (Username, PasswordHash, FullName, Role) " +
                    "VALUES (@username, @hash, @fullName, @role)",
                    new SqlParameter("@username", "admin"),
                    new SqlParameter("@hash", PasswordHasher.Hash("admin123")),
                    new SqlParameter("@fullName", "Default Administrator"),
                    new SqlParameter("@role", "Admin"));
            }
        }
    }
}
