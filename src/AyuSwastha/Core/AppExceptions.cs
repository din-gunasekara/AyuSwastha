using System;

namespace AyuSwastha.Core
{
    /// <summary>Base type for all domain-level errors raised by AyuSwastha.</summary>
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }
        public AppException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>Thrown when user input fails a business/validation rule.</summary>
    public class ValidationException : AppException
    {
        public ValidationException(string message) : base(message) { }
    }

    /// <summary>Thrown when authentication fails (bad credentials, inactive user).</summary>
    public class AuthenticationException : AppException
    {
        public AuthenticationException(string message) : base(message) { }
    }

    /// <summary>Thrown when a data-access operation fails.</summary>
    public class DataAccessException : AppException
    {
        public DataAccessException(string message) : base(message) { }
        public DataAccessException(string message, Exception inner) : base(message, inner) { }
    }
}
