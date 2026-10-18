using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Paddock.BuildingBlocks.Web;

// Dernier rempart : toute exception non rattrapée devient une réponse ProblemDetails, sans fuite d'information interne.
// Enregistré dans l'API par AddExceptionHandler<GlobalExceptionHandler>() + app.UseExceptionHandler().
public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    // Code non standard (popularisé par nginx) : le client a fermé la connexion avant la réponse.
    public const int ClientClosedRequestStatusCode = 499;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Requête annulée par le client (navigateur fermé…) : ce n'est pas un bug, donc pas de log d'erreur.
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            httpContext.Response.StatusCode = ClientClosedRequestStatusCode;
            return true;
        }

        // Depuis .NET 10, le middleware ne journalise plus une exception traitée par un IExceptionHandler :
        // sans ce log, les 500 seraient invisibles. Ce n'est donc pas un doublon.
        logger.LogError(exception, "Unhandled exception");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new() { Title = "An unexpected error occurred." },
        });
    }
}
