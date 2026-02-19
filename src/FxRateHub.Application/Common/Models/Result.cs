using System.Collections.Generic;
using System.Linq;

namespace FxRateHub.Application.Common.Models;

/// <summary>
/// Non-generic Result class for void operations
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result() { }

    public static Result Success()
    {
        return new Result { IsSuccess = true };
    }

    public static Result Failure(string error)
    {
        return new Result
        {
            IsSuccess = false,
            Error = error,
            Errors = new List<string> { error }
        };
    }

    public static Result Failure(List<string> errors)
    {
        return new Result
        {
            IsSuccess = false,
            Error = errors.FirstOrDefault(),
            Errors = errors
        };
    }
}

/// <summary>
/// Generic Result class for operations that return data
/// </summary>
/// <typeparam name="T">The type of data returned</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Error { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result() { }

    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Error = error,
            Errors = new List<string> { error }
        };
    }

    public static Result<T> Failure(List<string> errors)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Error = errors.FirstOrDefault(),
            Errors = errors
        };
    }
}
