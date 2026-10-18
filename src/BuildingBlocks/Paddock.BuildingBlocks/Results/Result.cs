using System.Diagnostics.CodeAnalysis;

namespace Paddock.BuildingBlocks.Results;

public class Result
{
    // private protected : seules les fabriques ci-dessous et Result<TValue> (même assembly) créent un Result.
    // Aucun autre projet ne peut en hériter. Succès = pas d'erreur (null) ; échec = une erreur.
    private protected Result(Error? error)
    {
        Error = error;
    }

    public Error? Error { get; }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    [MemberNotNullWhen(true, nameof(Error))]
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new(null); // = new Result(null);

    public static Result Failure(Error error)
    {
        // Avec la nullabilité activée, null ne devrait pas arriver ici, mais un appelant peut forcer avec « null! ».
        // Un échec sans erreur serait un succès déguisé.
        ArgumentNullException.ThrowIfNull(error);
        return new(error); // = new Result(error);
    }

    public static implicit operator Result(Error error) => Failure(error);
}
