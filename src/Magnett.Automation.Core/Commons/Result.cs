namespace Magnett.Automation.Core.Commons;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? ErrorMessage { get; }
    public Exception? Exception { get; }

    protected Result(bool isSuccess, string? errorMessage = null, Exception? exception = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    public static Result Success()
    {
        return new Result(true);
    }

    public static Result Failure(string errorMessage)
    {
        return new Result(false, errorMessage);
    }

    public static Result Failure(string errorMessage, Exception exception)
    {
        return new Result(false, errorMessage, exception);
    }

    public static implicit operator Result(Exception exception)
    {
        return new Result(false, exception.Message, exception);
    }
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public Exception? Exception { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(string errorMessage, Exception? exception = null)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
        Exception = exception;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Failure(string errorMessage)
    {
        return new Result<T>(errorMessage);
    }

    public static Result<T> Failure(string errorMessage, Exception exception)
    {
        return new Result<T>(errorMessage, exception);
    }

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(Exception exception)
    {
        return new Result<T>(exception.Message, exception);
    }

    public static implicit operator Result<T>(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot convert a non-generic success Result to a generic Result<T>");
        }
        
        return new Result<T>(result.ErrorMessage ?? "Unknown error", result.Exception);
    }
}
