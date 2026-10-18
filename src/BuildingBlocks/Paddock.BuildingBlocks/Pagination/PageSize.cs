namespace Paddock.BuildingBlocks.Pagination;

// Taille des pages : 20 par défaut, 50 au maximum.
public static class PageSize
{
    public const int Default = 20;
    public const int Max = 50;

    // Tolérant : un limit hors bornes est ramené dans [1, Max] plutôt que refusé.
    public static int Normalize(int? limit) =>
        limit is null ? Default : Math.Clamp(limit.Value, 1, Max);
}
