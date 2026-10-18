using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Paddock.BuildingBlocks.Results;
using Paddock.BuildingBlocks.Web;

namespace Paddock.UnitTests.BuildingBlocks.Web;

public sealed class ResultHttpExtensionsTests
{
    [Fact]
    public void A_success_is_passed_to_the_success_callback()
    {
        Result<int> result = 42;

        var httpResult = result.Match(TypedResults.Ok);

        var ok = Assert.IsType<Ok<int>>(httpResult);
        Assert.Equal(42, ok.Value);
    }

    [Theory]
    [InlineData(ErrorType.Validation, StatusCodes.Status400BadRequest)]
    [InlineData(ErrorType.Forbidden, StatusCodes.Status403Forbidden)]
    [InlineData(ErrorType.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorType.Conflict, StatusCodes.Status409Conflict)]
    public void Each_error_type_maps_to_its_status_code(ErrorType type, int expectedStatusCode)
    {
        Result<int> result = new Error("test.code", "Description.", type);

        var httpResult = result.Match(TypedResults.Ok);

        var problem = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal(expectedStatusCode, problem.StatusCode);
    }

    [Fact]
    public void A_failure_carries_its_error_code_in_the_problem_details()
    {
        Result result = Error.Conflict("booking.sold_out", "No place left for this track day.");

        var httpResult = result.Match(TypedResults.NoContent);

        var problem = Assert.IsType<ProblemHttpResult>(httpResult);
        Assert.Equal("booking.sold_out", problem.ProblemDetails.Extensions["code"]);
        Assert.Equal("No place left for this track day.", problem.ProblemDetails.Detail);
    }

    [Fact]
    public void Every_error_type_has_a_status_code()
    {
        Assert.All(Enum.GetValues<ErrorType>(), type =>
        {
            Result result = new Error("test.code", "Description.", type);

            var httpResult = result.Match(TypedResults.NoContent);

            Assert.IsType<ProblemHttpResult>(httpResult);
        });
    }
}
