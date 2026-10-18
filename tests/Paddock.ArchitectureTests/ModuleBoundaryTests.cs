using NetArchTest.Rules;

namespace Paddock.ArchitectureTests;

public sealed class ModuleBoundaryTests
{
    // Frontières : un module ne référence que les .Contracts des autres modules.
    // Vérifié sur les références d'assemblies (le compilateur ne garde que celles réellement utilisées).
    [Fact]
    public void Modules_only_reference_the_contracts_of_other_modules()
    {
        ArchAssert.SkipWhenNoModule();

        var violations =
            from assembly in Solution.ModuleImplementations.Concat(Solution.ModuleContracts)
            from reference in assembly.GetReferencedAssemblies()
            let referenced = reference.Name!
            where referenced.StartsWith($"{Solution.ModulesNamespace}.", StringComparison.Ordinal)
                && !referenced.EndsWith(Solution.ContractsSuffix, StringComparison.Ordinal)
                && referenced != $"{Solution.ModulesNamespace}.{Solution.ModuleName(assembly)}"
            select $"{assembly.GetName().Name} → {referenced}";

        Assert.Empty(violations);
    }

    // Domaine pur : le dossier Domain/ ne dépend d'aucun framework d'infrastructure.
    // Il peut utiliser Results et Pricing du socle, pas ses parties web ou persistance.
    [Fact]
    public void Module_domains_do_not_depend_on_infrastructure_frameworks()
    {
        ArchAssert.SkipWhenNoModule();

        var result = Types.InAssemblies(Solution.ModuleImplementations)
            .That()
            .ResideInNamespaceContaining(".Domain")
            .ShouldNot()
            .HaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "Npgsql",
                "Wolverine",
                "Stripe",
                "Paddock.BuildingBlocks.Web",
                "Paddock.BuildingBlocks.Persistence")
            .GetResult();

        ArchAssert.Passes(result);
    }

    // Le DbContext d'un module est public pour Wolverine, pas pour l'hôte.
    [Fact]
    public void The_api_does_not_depend_on_module_db_contexts()
    {
        ArchAssert.SkipWhenNoModule();

        var dbContexts = Types.InAssemblies(Solution.ModuleImplementations)
            .That()
            .HaveNameEndingWith("DbContext")
            .GetTypes()
            .Select(type => type.FullName)
            .ToArray();
        Assert.SkipWhen(dbContexts.Length == 0, "No module DbContext yet.");

        var result = Types.InAssembly(Solution.Api)
            .ShouldNot()
            .HaveDependencyOnAny(dbContexts)
            .GetResult();

        ArchAssert.Passes(result);
    }
}
