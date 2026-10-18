using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Paddock.Modules.Notifications;

// Point d'entrée public du module, appelé par Paddock.Api : enregistre ses services et mappe ses endpoints.
// Vide tant que le module n'est pas implémenté.
public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapNotificationsModule(this IEndpointRouteBuilder endpoints) => endpoints;
}
