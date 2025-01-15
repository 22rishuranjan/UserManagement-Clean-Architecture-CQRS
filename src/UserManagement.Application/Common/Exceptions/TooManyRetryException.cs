
namespace UserManagement.Application.Common.Exceptions;

public class TooManyRetryException : Exception
{
    /// <summary>
    /// Initializes a new instance of the NotFoundException class with a specified name of the queried object and its key.
    /// </summary>
    /// <param name="objectName">Name of the queried object.</param>
    /// <param name="key">The value by which the object is queried.</param>
    public TooManyRetryException(string key)
        : base($"Too Many Otp Retries, Email: {key}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the NotFoundException class with a specified name of the queried object, its key,
    /// and the exception that is the cause of this exception.
    /// </summary>
    /// <param name="objectName">Name of the queried object.</param>
    /// <param name="key">The value by which the object is queried.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TooManyRetryException(string key, Exception innerException)
        : base($"Too Many Otp Retries, Email: {key}", innerException)
    {
    }
}
