using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Paddock.BuildingBlocks.Persistence;

namespace Paddock.UnitTests.BuildingBlocks.Persistence;

// Le modèle EF Core (le schéma calculé) se construit sans se connecter à la base : on vérifie les noms produits.
public sealed class ModelConventionsTests
{
    [Fact]
    public void Table_and_column_names_are_snake_case()
    {
        var entityType = BuildModel().FindEntityType(typeof(TestBooking))!;

        Assert.Equal("test_bookings", entityType.GetTableName());
        Assert.Equal("starts_at", entityType.FindProperty(nameof(TestBooking.StartsAt))!.GetColumnName());
    }

    [Fact]
    public void Enums_are_stored_as_snake_case_text()
    {
        var status = BuildModel().FindEntityType(typeof(TestBooking))!.FindProperty(nameof(TestBooking.Status))!;

        Assert.Equal("text", status.GetColumnType());
        Assert.Equal("pending_payment", status.GetValueConverter()!.ConvertToProvider(TestStatus.PendingPayment));
    }

    [Fact]
    public void Nullable_enums_are_stored_as_snake_case_text_too()
    {
        var previous = BuildModel().FindEntityType(typeof(TestBooking))!.FindProperty(nameof(TestBooking.PreviousStatus))!;

        Assert.Equal("text", previous.GetColumnType());
        Assert.NotNull(previous.GetValueConverter());
    }

    // Modèle « design-time » : la version complète du modèle, avec toutes ses annotations relationnelles.
    private static IModel BuildModel()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseNpgsql("Host=unused")
            .UseModuleConventions()
            .Options;

        using var dbContext = new TestDbContext(options);
        return dbContext.GetService<IDesignTimeModel>().Model;
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<TestBooking> TestBookings => Set<TestBooking>();

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder) =>
            configurationBuilder.Conventions.Add(_ => new SnakeCaseEnumConvention());
    }

    private sealed class TestBooking
    {
        public Guid Id { get; set; }

        public DateTimeOffset StartsAt { get; set; }

        public TestStatus Status { get; set; }

        public TestStatus? PreviousStatus { get; set; }
    }

    private enum TestStatus
    {
        Draft,
        PendingPayment,
    }
}
