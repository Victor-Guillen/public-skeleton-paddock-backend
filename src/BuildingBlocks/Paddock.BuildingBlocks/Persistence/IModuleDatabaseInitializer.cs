namespace Paddock.BuildingBlocks.Persistence;

// Implémenté par chaque module, appelé par Paddock.MigrationService avant le démarrage de l'API.
public interface IModuleDatabaseInitializer
{
    // Nom du module, pour les logs et les traces (ex. "Booking").
    string Module { get; }

    // Applique les migrations EF Core du module dans son schéma.
    Task MigrateAsync(CancellationToken cancellationToken);

    // Données de démonstration, appelé en développement seulement. Doit être idempotent.
    Task SeedDevelopmentDataAsync(CancellationToken cancellationToken);
}
