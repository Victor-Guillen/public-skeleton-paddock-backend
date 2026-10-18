using NetArchTest.Rules;

namespace Paddock.ArchitectureTests;

public sealed class BuildingBlocksTests
{
    // Le socle est technique et partagé : il ne connaît ni les modules ni les hôtes.
    [Fact]
    public void Building_blocks_do_not_depend_on_modules_or_hosts()
    {
        var result = Types.InAssembly(Solution.BuildingBlocks)
            .ShouldNot()
            .HaveDependencyOnAny(Solution.ModulesNamespace, "Paddock.Api", "Paddock.AppHost", "Paddock.MigrationService")
            .GetResult();

        ArchAssert.Passes(result);
    }
}
