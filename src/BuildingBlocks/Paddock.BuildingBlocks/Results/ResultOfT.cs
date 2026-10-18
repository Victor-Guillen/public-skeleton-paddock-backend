namespace Paddock.BuildingBlocks.Results;

public sealed class Result<TValue> : Result
{
    // Assigné seulement en cas de succès ; garde sa valeur par défaut en cas d'échec.
    private readonly TValue? _value;

    private Result(TValue value) : base(null)
    {
        _value = value;
    }

    private Result(Error error) : base(error)
    {
    }

    // Lire la valeur d'un échec est une erreur de programmation (IsFailure non testé) : exception.
    // Le message cite le code d'erreur, jamais la valeur (risque de donnée personnelle dans les logs).
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"Cannot read the value of a failed result (error '{Error.Code}').");

    // Fabrique explicite : indispensable quand TValue est une interface
    // (C# interdit les conversions implicites depuis une interface).
    public static Result<TValue> Success(TValue value) => new(value);

    // « new » masque volontairement Result.Failure, qui renverrait un Result non générique.
    public static new Result<TValue> Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(error);
    }

    public static implicit operator Result<TValue>(TValue value) => Success(value);

    public static implicit operator Result<TValue>(Error error) => Failure(error);
}
