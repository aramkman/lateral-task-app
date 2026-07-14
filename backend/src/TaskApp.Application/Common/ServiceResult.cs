namespace TaskApp.Application.Common;

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

    #endregion

    #region Constructor

    private ServiceResult(bool isSuccess, T? value, IReadOnlyList<string> errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
    }

    #endregion

    #region Factories

    /// <summary>Creates a successful result carrying <paramref name="value"/>.</summary>
    public static ServiceResult<T> Success(T value) => new(true, value, Array.Empty<string>());

    /// <summary>Creates a failed result carrying the given <paramref name="errors"/>.</summary>
    public static ServiceResult<T> Failure(IEnumerable<string> errors) => new(false, default, errors.ToList());

    #endregion
}
