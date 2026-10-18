using NetArchTest.Rules;

namespace Paddock.ArchitectureTests;

// Encapsulation : dans l'implémentation d'un module, tout est internal sauf <M>Module,
// les consommateurs *Consumer et le *DbContext (publics pour Wolverine).
public sealed class ModuleEncapsulationTests
{
    [Fact]
    public void Module_implementation_types_are_internal_except_the_allowed_ones()
    {
        ArchAssert.SkipWhenNoModule();

        var result = Types.InAssemblies(Solution.ModuleImplementations)
            .That()
            .ArePublic()
            .And()
            .AreNotNested()
            .And()
            .DoNotHaveNameEndingWith("Module", "Consumer", "DbContext")
            // Migrations EF Core générées publiques : leur visibilité sera tranchée avec la première migration.
            .And()
            .DoNotResideInNamespaceContaining(".Migrations")
            .Should()
            .NotBePublic()
            .GetResult();

        ArchAssert.Passes(result);
    }

    // Wolverine ne découvre que les classes publiques : un consommateur internal serait ignoré sans erreur.
    [Fact]
    public void Consumers_are_public_static_classes()
    {
        ArchAssert.SkipWhenNoModule();

        var result = Types.InAssemblies(Solution.ModuleImplementations)
            .That()
            .HaveNameEndingWith("Consumer")
            .Should()
            .BePublic()
            .And()
            .BeStatic()
            .GetResult();

        ArchAssert.Passes(result);
    }

    // Wolverine découvre aussi les classes publiques en *Handler : un handler de cas d'usage public
    // deviendrait un handler de messages par erreur.
    [Fact]
    public void Use_case_handlers_are_not_public()
    {
        ArchAssert.SkipWhenNoModule();

        var result = Types.InAssemblies(Solution.ModuleImplementations)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .NotBePublic()
            .GetResult();

        ArchAssert.Passes(result);
    }
}
