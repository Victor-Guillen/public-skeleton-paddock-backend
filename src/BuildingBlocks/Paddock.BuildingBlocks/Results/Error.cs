namespace Paddock.BuildingBlocks.Results;

public sealed record Error
{
    // Constructeur explicite plutôt que syntaxe positionnelle : il valide le code, et les propriétés en { get; }
    // (sans init) empêchent de contourner cette validation avec une expression « with ».
    public Error(string code, string description, ErrorType type)
    {
        // Le code (<module>.<cas>) est un contrat avec les fronts : il ne peut pas être vide.
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        Code = code;
        Description = description;
        Type = type;
    }

    public string Code { get; }

    public string Description { get; }

    public ErrorType Type { get; }

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Forbidden(string code, string description) =>
        new(code, description, ErrorType.Forbidden);
}
