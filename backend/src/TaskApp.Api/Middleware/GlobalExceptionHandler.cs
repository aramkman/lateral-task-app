using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TaskApp.Api.Middleware;

/// <summary>
/// Catches any exception that escapes controller actions (bugs, infrastructure
/// failures) and turns it into a 500 ProblemDetails response instead of leaking a
/// stack trace or an unhandled-exception page. Expected failures (validation, not
/// found) never reach this — they're handled explicitly in <c>TasksController</c>
/// via <see cref="ProblemDetails"/> responses returned directly from the action.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    #region Fields

    private readonly ILogger<GlobalExceptionHandler> _logger;

    #endregion

    #region Constructor

    /// <summary>Creates the handler with its logger, injected via DI.</summary>
    /// <param name="logger">Logger used to record the unhandled exception.</param>
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    #endregion

    #region IExceptionHandler

    /// <summary>
    /// Logs the exception and writes a generic 500 ProblemDetails response.
    /// The exception's own message is never exposed to the client — only logged
    /// server-side — to avoid leaking internal details.
    /// </summary>
    /// <param name="httpContext">Context of the request that triggered the exception.</param>
    /// <param name="exception">The unhandled exception.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>Always true — this handler always produces a response.</returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Unhandled exception processing {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    #endregion
}
