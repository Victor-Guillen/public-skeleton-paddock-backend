namespace Paddock.ArchitectureTests;

public sealed class CoverageTests
{
    // Un module absent du dossier de sortie (non référencé par Paddock.Api) ne serait vérifié par aucune règle,
    // en silence. Ce test compare les projets présents sous src/Modules aux assemblies réellement inspectées.
    [Fact]
    public void Every_module_project_is_covered_by_the_architecture_tests()
    {
        var modulesDirectory = Path.Combine(Solution.RepositoryRoot.FullName, "src", "Modules");
        var moduleProjects = Directory.Exists(modulesDirectory)
            ? Directory.GetFiles(modulesDirectory, "*.csproj", SearchOption.AllDirectories)
                .Select(Path.GetFileNameWithoutExtension)
            : [];

        var inspected = Solution.ModuleImplementations.Concat(Solution.ModuleContracts)
            .Select(assembly => assembly.GetName().Name)
            .ToHashSet();

        var missing = moduleProjects.Where(project => !inspected.Contains(project));

        Assert.Empty(missing);
    }
}
