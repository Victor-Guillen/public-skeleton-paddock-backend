using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Paddock.IntegrationTests;

// Démarre la vraie API (Program.cs, pipeline complet) en mémoire : pas de port réseau, un HttpClient branché
// directement sur le serveur de test. Partagée par toute la collection « Api » : démarrée une seule fois.
// À venir : PostgreSQL via Testcontainers et un TestAuthHandler (utilisateur simulé par en-têtes).
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        // Valeur par défaut de WebApplicationFactory, rendue explicite : /health et OpenAPI n'existent qu'en Development.
        builder.UseEnvironment("Development");
}
