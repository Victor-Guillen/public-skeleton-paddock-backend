using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Paddock.BuildingBlocks.Web;

namespace Paddock.UnitTests.BuildingBlocks.Web;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task An_unexpected_exception_becomes_a_500_problem()
    {
        var (handler, httpContext, _) = CreateSut();

        var handled = await handler.TryHandleAsync(
            httpContext, new InvalidOperationException("Boom."), TestContext.Current.CancellationToken);

        var body = await ReadBodyAsync(httpContext);
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
        Assert.Contains("\"status\":500", body);
    }

    [Fact]
    public async Task An_unexpected_exception_is_logged_as_an_error()
    {
        var (handler, httpContext, logger) = CreateSut();

        await handler.TryHandleAsync(
            httpContext, new InvalidOperationException("Boom."), TestContext.Current.CancellationToken);

        Assert.Contains(LogLevel.Error, logger.Levels);
    }

    [Fact]
    public async Task The_exception_message_is_never_exposed()
    {
        var (handler, httpContext, _) = CreateSut();

        await handler.TryHandleAsync(
            httpContext,
            new InvalidOperationException("Host=db;Password=secret"),
            TestContext.Current.CancellationToken);

        var body = await ReadBodyAsync(httpContext);
        Assert.NotEmpty(body);
        Assert.DoesNotContain("secret", body);
    }

    [Fact]
    public async Task A_request_aborted_by_the_client_is_not_reported_as_an_error()
    {
        var (handler, httpContext, logger) = CreateSut();
        httpContext.RequestAborted = new CancellationToken(canceled: true);

        var handled = await handler.TryHandleAsync(
            httpContext, new OperationCanceledException(), TestContext.Current.CancellationToken);

        Assert.True(handled);
        Assert.Equal(GlobalExceptionHandler.ClientClosedRequestStatusCode, httpContext.Response.StatusCode);
        Assert.DoesNotContain(LogLevel.Error, logger.Levels);
    }

    // SUT = « system under test » : l'objet testé, avec ce dont il a besoin.
    private static (GlobalExceptionHandler Handler, DefaultHttpContext HttpContext, RecordingLogger<GlobalExceptionHandler> Logger) CreateSut()
    {
        // Vrai IProblemDetailsService du framework, comme dans l'API.
        // AddOptions() : dans l'API, WebApplication.CreateBuilder l'enregistre d'office ; ici, il faut le faire soi-même.
        var services = new ServiceCollection();
        services.AddOptions();
        services.AddProblemDetails();
        var provider = services.BuildServiceProvider();

        // Contexte HTTP en mémoire : pas de serveur, le corps de la réponse est écrit dans un MemoryStream.
        var httpContext = new DefaultHttpContext { RequestServices = provider };
        httpContext.Response.Body = new MemoryStream();

        var logger = new RecordingLogger<GlobalExceptionHandler>();
        var handler = new GlobalExceptionHandler(provider.GetRequiredService<IProblemDetailsService>(), logger);

        return (handler, httpContext, logger);
    }

    private static async Task<string> ReadBodyAsync(HttpContext httpContext)
    {
        httpContext.Response.Body.Position = 0;
        using var reader = new StreamReader(httpContext.Response.Body);
        return await reader.ReadToEndAsync(TestContext.Current.CancellationToken);
    }

    // Logger minimal qui retient le niveau de chaque log, pour vérifier qu'une requête annulée n'est pas une Error.
    private sealed class RecordingLogger<T> : ILogger<T>
    {
        public List<LogLevel> Levels { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter) => Levels.Add(logLevel);
    }
}
