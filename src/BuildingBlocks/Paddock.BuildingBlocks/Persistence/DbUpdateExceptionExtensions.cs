using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Paddock.BuildingBlocks.Persistence;

// « Jamais vérifier puis écrire » : on insère, et si une contrainte d'unicité refuse,
// le handler transforme l'exception en erreur métier.
//   catch (DbUpdateException e) when (e.IsUniqueViolation(Constraints.UniqueRaceNumber)) { return Errors.RaceNumberTaken; }
public static class DbUpdateExceptionExtensions
{
    // constraintName : nom explicite de la contrainte (constante du module). null = n'importe quelle contrainte d'unicité.
    public static bool IsUniqueViolation(this DbUpdateException exception, string? constraintName = null) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } postgres
        && (constraintName is null || postgres.ConstraintName == constraintName);
}
