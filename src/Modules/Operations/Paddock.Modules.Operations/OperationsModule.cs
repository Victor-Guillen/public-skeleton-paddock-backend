using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Paddock.Modules.Operations;

// Point d'entrée public du module, appelé par Paddock.Api : enregistre ses services et mappe ses endpoints.
// Vide tant que le module n'est pas implémenté.
public static class OperationsModule
{
    public static IServiceCollection AddOperationsModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapOperationsModule(this IEndpointRouteBuilder endpoints) => endpoints;
}
