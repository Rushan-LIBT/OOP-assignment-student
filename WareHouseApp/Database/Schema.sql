-- Warehouse Management System - database schema
-- The application also creates these tables automatically at startup
-- (see Data/DatabaseInitializer.cs). This script is provided so the schema
-- can be inspected or recreated manually if needed.

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Users' AND xtype = 'U')
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(128) NOT NULL,
    FullName NVARCHAR(100) NULL,
    Role NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Materials' AND xtype = 'U')
CREATE TABLE Materials (
    MaterialID INT IDENTITY(1,1) PRIMARY KEY,
    MaterialName NVARCHAR(100) NOT NULL,
    Category NVARCHAR(50) NULL,
    Quantity INT NOT NULL DEFAULT 0,
    UnitPrice DECIMAL(18,2) NOT NULL DEFAULT 0
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Customers' AND xtype = 'U')
CREATE TABLE Customers (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NULL,
    Phone NVARCHAR(30) NULL,
    Address NVARCHAR(200) NULL
);

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Employees' AND xtype = 'U')
CREATE TABLE Employees (
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Role NVARCHAR(50) NULL,
    Email NVARCHAR(100) NULL,
    Phone NVARCHAR(30) NULL,
    Salary DECIMAL(18,2) NOT NULL DEFAULT 0
);

-- Default administrator account.
-- Username: admin   Password: admin123
-- (The PasswordHash below is the SHA-256 hash of "admin123".)
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
INSERT INTO Users (Username, PasswordHash, FullName, Role)
VALUES ('admin',
        '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9',
        'Default Administrator',
        'Admin');
