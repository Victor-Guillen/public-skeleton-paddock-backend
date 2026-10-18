using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Paddock.BuildingBlocks.Web;

namespace Paddock.UnitTests.BuildingBlocks.Web;

public sealed class ValidationFilterTests
{
    // Ce que renvoie le faux handler : permet de vérifier que c'est bien lui qui a répondu.
    private static readonly object _handlerResult = new();

    private static readonly TestRequest _validRequest = new("Jane", "jane@example.com", new TestVehicle("AB-123-CD"));

    private static readonly TestRequest _invalidRequest = new("", "not-an-email", new TestVehicle(""));

    [Fact]
    public async Task A_valid_request_reaches_the_handler()
    {
        var response = await InvokeFilterAsync(_validRequest);

        Assert.Same(_handlerResult, response);
    }

    [Fact]
    public async Task An_invalid_request_never_reaches_the_handler()
    {
        var handlerCalled = false;

        await InvokeFilterAsync(_invalidRequest, _ =>
        {
            handlerCalled = true;
            return ValueTask.FromResult<object?>(_handlerResult);
        });

        Assert.False(handlerCalled);
    }

    [Fact]
    public async Task An_invalid_request_returns_a_validation_problem_with_its_code()
    {
        var response = await InvokeFilterAsync(_invalidRequest);

        var problem = Assert.IsType<ValidationProblem>(response);
        Assert.Equal(StatusCodes.Status400BadRequest, problem.StatusCode);
        Assert.Equal("common.validation_failed", problem.ProblemDetails.Extensions["code"]);
    }

    [Fact]
    public async Task Validation_errors_use_camel_case_field_names()
    {
        var response = await InvokeFilterAsync(_invalidRequest);

        var problem = Assert.IsType<ValidationProblem>(response);
        Assert.Equal(
            ["emailAddress", "firstName", "vehicle.plate"],
            problem.ProblemDetails.Errors.Keys.Order());
    }

    // Appelle le filtre comme le ferait ASP.NET Core : contexte HTTP en mémoire, arguments de l'endpoint, puis next.
    private static async Task<object?> InvokeFilterAsync(TestRequest request, EndpointFilterDelegate? next = null)
    {
        var filter = new ValidationFilter<TestRequest>(new TestRequestValidator());
        var context = EndpointFilterInvocationContext.Create(new DefaultHttpContext(), request);

        return await filter.InvokeAsync(context, next ?? (_ => ValueTask.FromResult<object?>(_handlerResult)));
    }

    // Requête et validateur factices, propres à ces tests (noms composés et propriété imbriquée pour le camelCase).
    private sealed record TestRequest(string FirstName, string EmailAddress, TestVehicle Vehicle);

    private sealed record TestVehicle(string Plate);

    private sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.EmailAddress).EmailAddress();
            RuleFor(x => x.Vehicle.Plate).NotEmpty();
        }
    }
}
