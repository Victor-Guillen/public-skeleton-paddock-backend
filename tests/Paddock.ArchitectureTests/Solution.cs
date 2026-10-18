using System.Reflection;
using Paddock.BuildingBlocks.Results;

namespace Paddock.ArchitectureTests;

// Les assemblies inspectées par les règles d'architecture.
// Les modules sont découverts dans le dossier de sortie des tests : ils y sont copiés parce que Paddock.Api, référencé
// par ce projet, référence chaque module. Un module que l'API ne référence pas serait donc invisible : c'est ce que
// vérifie CoverageTests.
internal static class Solution
{
    public const string ModulesNamespace = "Paddock.Modules";
    public const string ContractsSuffix = ".Contracts";

    public static readonly Assembly Api = typeof(Program).Assembly;

    public static readonly Assembly BuildingBlocks = typeof(Result).Assembly;

    // Paddock.Modules.<M>.dll : l'implémentation de chaque module.
    public static readonly IReadOnlyList<Assembly> ModuleImplementations = LoadModuleAssemblies()
        .Where(assembly => !IsContracts(assembly))
        .ToList();

    // Paddock.Modules.<M>.Contracts.dll : l'API publique de chaque module.
    public static readonly IReadOnlyList<Assembly> ModuleContracts = LoadModuleAssemblies()
        .Where(IsContracts)
        .ToList();

    public static bool HasModules => ModuleImplementations.Count > 0;

    // Racine du dépôt : premier dossier parent qui contient Paddock.slnx.
    public static DirectoryInfo RepositoryRoot { get; } = FindRepositoryRoot();

    public static string ModuleName(Assembly assembly) =>
        assembly.GetName().Name!.Split('.')[2]; // Paddock.Modules.<M>[.Contracts] → <M>

    private static bool IsContracts(Assembly assembly) =>
        assembly.GetName().Name!.EndsWith(ContractsSuffix, StringComparison.Ordinal);

    private static IEnumerable<Assembly> LoadModuleAssemblies() =>
        Directory.GetFiles(AppContext.BaseDirectory, $"{ModulesNamespace}.*.dll")
            .Select(path => Assembly.Load(Path.GetFileNameWithoutExtension(path)));

    private static DirectoryInfo FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Paddock.slnx")))
        {
            directory = directory.Parent;
        }

        return directory ?? throw new InvalidOperationException("Paddock.slnx not found above the test output directory.");
    }
}
