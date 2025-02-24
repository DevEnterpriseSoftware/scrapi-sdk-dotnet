using System.Net;

namespace DevEnterprise.Scrapi.Sdk;

/// <summary>
/// Represents errors that may occur when making calls to the API.
/// </summary>
/// <seealso cref="Exception"/>
public class ScrapiException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ScrapiException"/> class.
  /// </summary>
  public ScrapiException()
  {
    StatusCode = HttpStatusCode.InternalServerError;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ScrapiException"/> class with a specified error message.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  public ScrapiException(string? message)
    : base(message)
  {
    StatusCode = HttpStatusCode.InternalServerError;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ScrapiException"/> class with a specified error
  /// message and a reference to the inner exception that is the cause of this exception.
  /// </summary>
  /// <param name="message">The error message that explains the reason for the exception.</param>
  /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
  public ScrapiException(string? message, Exception? innerException)
    : base(message, innerException)
  {
    StatusCode = HttpStatusCode.InternalServerError;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ScrapiException"/> class with a specified HTTP status code.
  /// </summary>
  /// <param name="statusCode">The status code returned by the API.</param>
  public ScrapiException(HttpStatusCode? statusCode)
  {
    StatusCode = statusCode ?? HttpStatusCode.InternalServerError;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ScrapiException"/> class with a specified HTTP status code and error message.
  /// </summary>
  /// <param name="statusCode">The status code returned by the API.</param>
  /// <param name="message">The message.</param>
  public ScrapiException(HttpStatusCode? statusCode, string? message)
    : base(message)
  {
    StatusCode = statusCode ?? HttpStatusCode.InternalServerError;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ScrapiException"/> class  with a specified HTTP status code,
  /// error message and a reference to the inner exception that is the cause of this exception.
  /// </summary>
  /// <param name="statusCode">The status code returned by the API.</param>
  /// <param name="message">The error message that explains the reason for the exception.</param>
  /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
  public ScrapiException(HttpStatusCode? statusCode, string? message, Exception? innerException)
    : base(message, innerException)
  {
    StatusCode = statusCode ?? HttpStatusCode.InternalServerError;
  }

  /// <summary>
  /// Gets or sets the status code.
  /// </summary>
  /// <value>
  /// The status code.
  /// </value>
  public HttpStatusCode StatusCode { get; }
}
