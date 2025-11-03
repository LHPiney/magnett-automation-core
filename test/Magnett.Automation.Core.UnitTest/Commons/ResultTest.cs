using System;
using Xunit;

using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.UnitTest.Commons;

public class ResultTest
{
    [Fact]
    public void Success_WhenInvoke_ReturnSuccessResult()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.ErrorMessage);
        Assert.Null(result.Exception);
    }

    [Fact]
    public void Failure_WhenInvokeWithMessage_ReturnFailureResult()
    {
        const string errorMessage = "Test error message";
        var result = Result.Failure(errorMessage);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.Null(result.Exception);
    }

    [Fact]
    public void Failure_WhenInvokeWithMessageAndException_ReturnFailureResult()
    {
        const string errorMessage = "Test error message";
        var exception = new InvalidOperationException("Inner exception");
        var result = Result.Failure(errorMessage, exception);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.Equal(exception, result.Exception);
    }

    [Fact]
    public void ImplicitOperator_WhenException_ConvertToFailureResult()
    {
        var exception = new InvalidOperationException("Test exception");
        Result result = exception;

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(exception.Message, result.ErrorMessage);
        Assert.Equal(exception, result.Exception);
    }
}

public class ResultGenericTest
{
    [Fact]
    public void Success_WhenInvokeWithValue_ReturnSuccessResult()
    {
        const int value = 42;
        var result = Result<int>.Success(value);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(value, result.Value);
        Assert.Null(result.ErrorMessage);
        Assert.Null(result.Exception);
    }

    [Fact]
    public void Failure_WhenInvokeWithMessage_ReturnFailureResult()
    {
        const string errorMessage = "Test error message";
        var result = Result<int>.Failure(errorMessage);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(default(int), result.Value);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.Null(result.Exception);
    }

    [Fact]
    public void Failure_WhenInvokeWithMessageAndException_ReturnFailureResult()
    {
        const string errorMessage = "Test error message";
        var exception = new InvalidOperationException("Inner exception");
        var result = Result<int>.Failure(errorMessage, exception);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(default(int), result.Value);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.Equal(exception, result.Exception);
    }

    [Fact]
    public void ImplicitOperator_WhenValue_ConvertToSuccessResult()
    {
        const string value = "Test value";
        Result<string> result = value;

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(value, result.Value);
        Assert.Null(result.ErrorMessage);
        Assert.Null(result.Exception);
    }

    [Fact]
    public void ImplicitOperator_WhenException_ConvertToFailureResult()
    {
        var exception = new InvalidOperationException("Test exception");
        Result<int> result = exception;

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(default(int), result.Value);
        Assert.Equal(exception.Message, result.ErrorMessage);
        Assert.Equal(exception, result.Exception);
    }

    [Fact]
    public void ImplicitOperator_WhenNonGenericSuccessResult_ThrowException()
    {
        var nonGenericResult = Result.Success();
        
        Assert.Throws<InvalidOperationException>(() =>
        {
            Result<int> _ = nonGenericResult;
        });
    }

    [Fact]
    public void ImplicitOperator_WhenNonGenericFailureResult_ConvertToFailureResult()
    {
        const string errorMessage = "Test error";
        var exception = new InvalidOperationException("Inner exception");
        var nonGenericResult = Result.Failure(errorMessage, exception);
        
        Result<int> result = nonGenericResult;

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(default(int), result.Value);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.Equal(exception, result.Exception);
    }

    [Fact]
    public void ImplicitOperator_WhenNonGenericFailureResultWithoutException_ConvertToFailureResult()
    {
        const string errorMessage = "Test error";
        var nonGenericResult = Result.Failure(errorMessage);
        
        Result<string> result = nonGenericResult;

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(default(string), result.Value);
        Assert.Equal(errorMessage, result.ErrorMessage);
        Assert.Null(result.Exception);
    }

    [Fact]
    public void Success_WhenInvokeWithNullValue_ReturnSuccessResultWithNull()
    {
        var result = Result<string>.Success(null);

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Value);
        Assert.Null(result.ErrorMessage);
        Assert.Null(result.Exception);
    }

    [Fact]
    public void Failure_WhenInvokeWithNullMessage_ReturnFailureResultWithNullMessage()
    {
        var result = Result<int>.Failure(null);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Null(result.ErrorMessage);
        Assert.Null(result.Exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(42)]
    [InlineData(-100)]
    public void Success_WhenInvokeWithDifferentIntegerValues_ReturnCorrectValue(int value)
    {
        var result = Result<int>.Success(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("test")]
    [InlineData("Test with spaces")]
    public void Success_WhenInvokeWithDifferentStringValues_ReturnCorrectValue(string value)
    {
        var result = Result<string>.Success(value);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }
}

