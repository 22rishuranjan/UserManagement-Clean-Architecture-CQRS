
namespace UserManagement.Application.Common.Exceptions;

public class OtpTimeoutException : Exception
{
    /// <summary>
    /// Initializes a new instance of the NotFoundException class with a specified name of the queried object and its key.
    /// </summary>
    /// <param name="objectName">Name of the queried object.</param>
    /// <param name="key">The value by which the object is queried.</param>
    public OtpTimeoutException(string key)
        : base($"OTP Timeout. Generate another OTP, Key: {key}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the NotFoundException class with a specified name of the queried object, its key,
    /// and the exception that is the cause of this exception.
    /// </summary>
    /// <param name="objectName">Name of the queried object.</param>
    /// <param name="key">The value by which the object is queried.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public OtpTimeoutException(string key, Exception innerException)
        : base($"OTP Timeout. Generate another OTP, Key: {key}", innerException)
    {
    }
}
