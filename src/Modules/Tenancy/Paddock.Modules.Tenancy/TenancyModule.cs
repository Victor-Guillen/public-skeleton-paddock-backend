using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Paddock.Modules.Tenancy;

// Point d'entrée public du module, appelé par Paddock.Api : enregistre ses services et mappe ses endpoints.
// Vide tant que le module n'est pas implémenté.
public static class TenancyModule
{
    public static IServiceCollection AddTenancyModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapTenancyModule(this IEndpointRouteBuilder endpoints) => endpoints;
}
