namespace TaskApp.Application.Common;

/// <summary>
/// Categorizes why a <see cref="ServiceResult{T}"/> failed, so callers (controllers)
/// can map it to the right HTTP status without string-matching error messages.
/// </summary>
public enum ServiceResultErrorType
{
    /// <summary>No failure — the result is successful.</summary>
    None,

    /// <summary>Input failed validation (maps to 400).</summary>
    Validation,

    /// <summary>The requested resource does not exist (maps to 404).</summary>
    NotFound
}

/// <summary>
/// Lightweight outcome wrapper for Application-layer operations that can fail
/// (validation, not-found). Lets services signal failure without throwing
/// exceptions for expected, common outcomes — a full Result library (e.g.
/// FluentResults) would be disproportionate for the handful of call sites here.
/// </summary>
/// <typeparam name="T">Type of the value returned on success.</typeparam>
public class ServiceResult<T>
{
    #region Properties

    /// <summary>Whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>The resulting value, set only when <see cref="IsSuccess"/> is true.</summary>
    public T? Value { get; }

    /// <summary>Human-readable failure reasons, empty when <see cref="IsSuccess"/> is true.</summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>Why the operation failed, used to pick the right HTTP status. <see cref="ServiceResultErrorType.None"/> on success.</summary>
    public ServiceResultErrorType ErrorType { get; }

    #endregion

    #region Constructor

    private ServiceResult(bool isSuccess, T? value, IReadOnlyList<string> errors, ServiceResultErrorType errorType)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
        ErrorType = errorType;
    }

    #endregion

    #region Factories

    /// <summary>Creates a successful result carrying <paramref name="value"/>.</summary>
    public static ServiceResult<T> Success(T value) =>
        new(true, value, Array.Empty<string>(), ServiceResultErrorType.None);

    /// <summary>Creates a failed result for invalid input, carrying the given <paramref name="errors"/>.</summary>
    public static ServiceResult<T> ValidationFailure(IEnumerable<string> errors) =>
        new(false, default, errors.ToList(), ServiceResultErrorType.Validation);

    /// <summary>Creates a failed result for a missing resource.</summary>
    public static ServiceResult<T> NotFound(string error) =>
        new(false, default, new[] { error }, ServiceResultErrorType.NotFound);

    #endregion
}

/// <summary>
/// Non-generic counterpart to <see cref="ServiceResult{T}"/>, for operations that
/// don't return a value on success (e.g. delete, which maps to 204 No Content).
/// </summary>
public class ServiceResult
{
    #region Properties

    /// <summary>Whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Human-readable failure reasons, empty when <see cref="IsSuccess"/> is true.</summary>
    public IReadOnlyList<string> Errors { get; }

    /// <summary>Why the operation failed, used to pick the right HTTP status. <see cref="ServiceResultErrorType.None"/> on success.</summary>
    public ServiceResultErrorType ErrorType { get; }

    #endregion

    #region Constructor

    private ServiceResult(bool isSuccess, IReadOnlyList<string> errors, ServiceResultErrorType errorType)
    {
        IsSuccess = isSuccess;
        Errors = errors;
        ErrorType = errorType;
    }

    #endregion

    #region Factories

    /// <summary>Creates a successful result.</summary>
    public static ServiceResult Success() => new(true, Array.Empty<string>(), ServiceResultErrorType.None);

    /// <summary>Creates a failed result for a missing resource.</summary>
    public static ServiceResult NotFound(string error) => new(false, new[] { error }, ServiceResultErrorType.NotFound);

    #endregion
}
