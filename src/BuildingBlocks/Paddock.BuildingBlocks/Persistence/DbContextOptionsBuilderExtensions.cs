using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Paddock.BuildingBlocks.Persistence;

// Réglages communs à tous les DbContext de module. La façon de les brancher (UseNpgsql direct ou
// intégration client Aspire) sera choisie avec le premier DbContext.
public static class DbContextOptionsBuilderExtensions
{
    // Tables, colonnes, clés et index en snake_case (TrackDay.StartsAt → track_days.starts_at).
    // Générique pour conserver le type du builder (DbContextOptionsBuilder<TContext>) dans une chaîne d'appels.
    public static TBuilder UseModuleConventions<TBuilder>(this TBuilder options)
        where TBuilder : DbContextOptionsBuilder
    {
        options.UseSnakeCaseNamingConvention();
        return options;
    }

    // Table d'historique des migrations dans le schéma du module : chaque module migre indépendamment.
    public static NpgsqlDbContextOptionsBuilder UseModuleMigrationsHistory(
        this NpgsqlDbContextOptionsBuilder npgsql,
        string schema) =>
        npgsql.MigrationsHistoryTable(HistoryRepository.DefaultTableName, schema);
}
