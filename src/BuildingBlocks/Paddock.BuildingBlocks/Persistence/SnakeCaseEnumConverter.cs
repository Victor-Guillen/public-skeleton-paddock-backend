using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Paddock.BuildingBlocks.Persistence;

// Stocke un enum en texte snake_case (BookingStatus.PendingPayment ↔ 'pending_payment') : lisible en SQL,
// et réordonner ou insérer une valeur dans l'enum ne corrompt pas les données existantes.
public sealed class SnakeCaseEnumConverter<TEnum>() : ValueConverter<TEnum, string>(
    value => ToSnakeCase(value),
    text => FromSnakeCase(text))
    where TEnum : struct, Enum
{
    // Calculés une fois par type d'enum (champ statique d'une classe générique : un jeu par TEnum).
    private static readonly Dictionary<TEnum, string> _names = Enum.GetValues<TEnum>()
        .ToDictionary(value => value, value => JsonNamingPolicy.SnakeCaseLower.ConvertName(value.ToString()));

    private static readonly Dictionary<string, TEnum> _values = _names
        .ToDictionary(entry => entry.Value, entry => entry.Key);

    private static string ToSnakeCase(TEnum value) =>
        _names.TryGetValue(value, out var name)
            ? name
            : throw new InvalidOperationException($"'{value}' is not a defined value of {typeof(TEnum).Name}.");

    // Une valeur inconnue en base est une donnée corrompue ou une migration oubliée : imprévu, donc exception.
    private static TEnum FromSnakeCase(string text) =>
        _values.TryGetValue(text, out var value)
            ? value
            : throw new InvalidOperationException($"'{text}' is not a stored value of {typeof(TEnum).Name}.");
}
