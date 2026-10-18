namespace Paddock.BuildingBlocks.Pagination;

public static class CursorPage
{
    // fetched : le résultat d'une requête limitée à limit + 1 lignes (Take(limit + 1)).
    // La ligne en trop, si elle existe, prouve qu'il y a une page suivante ; elle n'est pas renvoyée.
    // positionOf : les clés de tri d'un élément, qui deviennent le curseur de la page suivante.
    public static CursorPage<T> From<T, TPosition>(IReadOnlyList<T> fetched, int limit, Func<T, TPosition> positionOf)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit);

        var hasMore = fetched.Count > limit;
        IReadOnlyList<T> items = hasMore ? [.. fetched.Take(limit)] : fetched;
        var nextCursor = hasMore ? Cursor.Encode(positionOf(items[^1])) : null;

        return new CursorPage<T>(items, nextCursor, hasMore);
    }
}
