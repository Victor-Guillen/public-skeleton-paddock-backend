using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Paddock.Modules.Booking;

// Point d'entrée public du module, appelé par Paddock.Api : enregistre ses services et mappe ses endpoints.
// Vide tant que le module n'est pas implémenté.
public static class BookingModule
{
    public static IServiceCollection AddBookingModule(this IServiceCollection services) => services;

    public static IEndpointRouteBuilder MapBookingModule(this IEndpointRouteBuilder endpoints) => endpoints;
}
