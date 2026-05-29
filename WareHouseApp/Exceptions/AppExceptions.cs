using System;

namespace WareHouseApp.Exceptions
{
    /// <summary>Base type for all application specific exceptions.</summary>
    public class WarehouseException : Exception
    {
        public WarehouseException(string message) : base(message) { }
        public WarehouseException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>Thrown when a login attempt fails.</summary>
    public class AuthenticationException : WarehouseException
    {
        public AuthenticationException(string message) : base(message) { }
    }

    /// <summary>Thrown when user supplied data fails validation.</summary>
    public class ValidationException : WarehouseException
    {
        public ValidationException(string message) : base(message) { }
    }

    /// <summary>Thrown when a data access operation fails.</summary>
    public class DataAccessException : WarehouseException
    {
        public DataAccessException(string message, Exception inner) : base(message, inner) { }
    }
}
