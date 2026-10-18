using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Paddock.BuildingBlocks.Web;

// Valide la forme de la requête (FluentValidation) avant le handler ; les règles métier restent dans le domaine.
// Usage : group.MapPost(…).AddEndpointFilter<ValidationFilter<AddVehicleRequest>>()
// Le filtre est créé à chaque requête avec les services de la requête : injecter le validateur ici est sûr.
public sealed class ValidationFilter<TRequest>(IValidator<TRequest> validator) : IEndpointFilter
{
    // Contrat avec les fronts : ne jamais renommer.
    private const string ErrorCode = "common.validation_failed";

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (request is null)
        {
            return await next(context);
        }

        var validation = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);
        if (validation.IsValid)
        {
            return await next(context);
        }

        var errors = validation.ToDictionary().ToDictionary(
            entry => ToCamelCase(entry.Key),
            entry => entry.Value);

        return TypedResults.ValidationProblem(
            errors,
            extensions: new Dictionary<string, object?> { ["code"] = ErrorCode });
    }

    // "FirstName" → "firstName" ; "Vehicle.Plate" → "vehicle.plate" : chaque segment du chemin est converti.
    private static string ToCamelCase(string propertyPath) =>
        string.Join('.', propertyPath.Split('.').Select(JsonNamingPolicy.CamelCase.ConvertName));
}
