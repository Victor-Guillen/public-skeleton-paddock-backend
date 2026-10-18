using System.Buffers.Text;
using System.Text.Json;
using Paddock.BuildingBlocks.Results;

namespace Paddock.BuildingBlocks.Pagination;

// Curseur opaque pour le client : la position (clés de tri du dernier élément renvoyé) en JSON, encodée en Base64 URL.
// Il n'est pas signé : le filtre multi-organisateurs s'applique de toute façon à la requête.
public static class Cursor
{
    public static readonly Error Invalid =
        Error.Validation("common.invalid_cursor", "The pagination cursor is invalid.");

    // Strict au décodage : une propriété manquante ou un null inattendu rend le curseur invalide,
    // au lieu de produire une position avec des valeurs par défaut (Guid.Empty…).
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        RespectRequiredConstructorParameters = true,
        RespectNullableAnnotations = true,
    };

    public static string Encode<TPosition>(TPosition position) =>
        Base64Url.EncodeToString(JsonSerializer.SerializeToUtf8Bytes(position, _jsonOptions));

    // Le curseur vient du client : une valeur illisible est un cas prévu (400), pas une exception (500).
    public static Result<TPosition> Decode<TPosition>(string cursor)
    {
        try
        {
            var position = JsonSerializer.Deserialize<TPosition>(Base64Url.DecodeFromChars(cursor), _jsonOptions);
            if (position is null)
            {
                return Invalid;
            }

            return position;
        }
        catch (Exception exception) when (exception is FormatException or JsonException)
        {
            return Invalid;
        }
    }
}
