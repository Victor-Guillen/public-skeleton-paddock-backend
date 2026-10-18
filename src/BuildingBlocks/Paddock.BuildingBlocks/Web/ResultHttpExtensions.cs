using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Paddock.BuildingBlocks.Results;

namespace Paddock.BuildingBlocks.Web;

// Méthodes d'extension : la classe doit être static (et non générique).
public static class ResultHttpExtensions
{
    // Usage : (await handler.HandleAsync(id, ct)).Match(TypedResults.Ok)
    public static IResult Match<TValue>(this Result<TValue> result, Func<TValue, IResult> onSuccess) =>
        result.IsSuccess ? onSuccess(result.Value) : ToProblem(result.Error);

    // Usage : result.Match(TypedResults.NoContent)
    public static IResult Match(this Result result, Func<IResult> onSuccess) =>
        result.IsSuccess ? onSuccess() : ToProblem(result.Error);

    private static ProblemHttpResult ToProblem(Error error) =>
        TypedResults.Problem(
            statusCode: ToStatusCode(error.Type),
            detail: error.Description,
            extensions: new Dictionary<string, object?> { ["code"] = error.Code }
        );

    private static int ToStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => throw new UnreachableException($"ErrorType '{type}' has no HTTP status code mapping."),
    };
}
