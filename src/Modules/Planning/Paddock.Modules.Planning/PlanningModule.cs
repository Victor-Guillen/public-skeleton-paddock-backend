using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Paddock.Modules.Planning;

// Point d'entrée public du module, appelé par Paddock.Api : enregistre ses services et mappe ses endpoints.
// Vide tant que le module n'est pas implémenté.
public static class PlanningModule
{
    public static IServiceCollection AddPlanningModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapPlanningModule(this IEndpointRouteBuilder endpoints) => endpoints;
}
