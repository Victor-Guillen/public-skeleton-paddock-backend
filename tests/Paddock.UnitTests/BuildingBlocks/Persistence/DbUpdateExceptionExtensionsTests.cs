using Microsoft.EntityFrameworkCore;
using Npgsql;
using Paddock.BuildingBlocks.Persistence;

namespace Paddock.UnitTests.BuildingBlocks.Persistence;

// Exceptions PostgreSQL construites à la main ; la vérification avec une vraie contrainte viendra avec les tests d'intégration sur PostgreSQL.
public sealed class DbUpdateExceptionExtensionsTests
{
    private const string Constraint = "ux_bookings_reference";

    [Fact]
    public void A_unique_violation_is_recognized()
    {
        var exception = Wrap(PostgresError(PostgresErrorCodes.UniqueViolation, Constraint));

        Assert.True(exception.IsUniqueViolation());
    }

    [Fact]
    public void A_unique_violation_is_recognized_by_its_constraint_name()
    {
        var exception = Wrap(PostgresError(PostgresErrorCodes.UniqueViolation, Constraint));

        Assert.True(exception.IsUniqueViolation(Constraint));
        Assert.False(exception.IsUniqueViolation("ux_another_constraint"));
    }

    [Fact]
    public void Another_database_error_is_not_a_unique_violation()
    {
        var exception = Wrap(PostgresError(PostgresErrorCodes.CheckViolation, "ck_inventory_capacity"));

        Assert.False(exception.IsUniqueViolation());
    }

    [Fact]
    public void An_error_that_does_not_come_from_postgres_is_not_a_unique_violation()
    {
        var exception = Wrap(new InvalidOperationException("Not a database error."));

        Assert.False(exception.IsUniqueViolation());
    }

    private static DbUpdateException Wrap(Exception inner) => new("Saving failed.", inner);

    private static PostgresException PostgresError(string sqlState, string constraintName) =>
        new("Database error.", "ERROR", "ERROR", sqlState, constraintName: constraintName);
}
