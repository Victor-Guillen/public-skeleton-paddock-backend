namespace Paddock.BuildingBlocks.Pagination;

// Réponse paginée de toute liste de l'API : { items, nextCursor, hasMore }.
public sealed record CursorPage<T>(IReadOnlyList<T> Items, string? NextCursor, bool HasMore);
