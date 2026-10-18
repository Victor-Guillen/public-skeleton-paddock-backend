using Paddock.BuildingBlocks.Results;

namespace Paddock.UnitTests.BuildingBlocks.Results;

public sealed class ResultTests
{
    [Fact]
    public void Success_has_no_error()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_exposes_its_error()
    {
        var error = Error.NotFound("test.not_found", "Not found.");

        var result = Result.Failure(error);

        Assert.True(result.IsFailure);
        Assert.Same(error, result.Error);
    }

    [Fact]
    public void A_failure_cannot_be_created_without_an_error()
    {
        Assert.Throws<ArgumentNullException>(() => Result.Failure(null!));
    }

    [Fact]
    public void An_error_converts_implicitly_to_a_failed_result()
    {
        var error = Error.Conflict("test.conflict", "Conflict.");

        Result<int> result = error;

        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void A_value_converts_implicitly_to_a_successful_result()
    {
        Result<int> result = 42;

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Reading_the_value_of_a_failure_throws()
    {
        Result<int> result = Error.Conflict("test.conflict", "Conflict.");

        var exception = Assert.Throws<InvalidOperationException>(() => result.Value);

        Assert.Contains("test.conflict", exception.Message);
    }

    [Fact]
    public void Errors_with_the_same_code_description_and_type_are_equal()
    {
        var first = Error.Conflict("test.conflict", "Conflict.");
        var second = Error.Conflict("test.conflict", "Conflict.");

        Assert.NotSame(first, second);
        Assert.Equal(first, second);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void An_error_requires_a_code(string? code)
    {
        Assert.ThrowsAny<ArgumentException>(() => Error.Conflict(code!, "Conflict."));
    }

    [Fact]
    public void Each_error_factory_sets_its_type()
    {
        Assert.Equal(ErrorType.Validation, Error.Validation("test.code", "Description.").Type);
        Assert.Equal(ErrorType.NotFound, Error.NotFound("test.code", "Description.").Type);
        Assert.Equal(ErrorType.Conflict, Error.Conflict("test.code", "Description.").Type);
        Assert.Equal(ErrorType.Forbidden, Error.Forbidden("test.code", "Description.").Type);
    }
}
