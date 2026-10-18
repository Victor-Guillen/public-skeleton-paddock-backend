using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Paddock.Modules.Billing;

// Point d'entrée public du module, appelé par Paddock.Api : enregistre ses services et mappe ses endpoints.
// Vide tant que le module n'est pas implémenté.
public static class BillingModule
{
    public static IServiceCollection AddBillingModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapBillingModule(this IEndpointRouteBuilder endpoints) => endpoints;
}
