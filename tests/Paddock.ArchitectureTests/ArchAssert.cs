// Alias : xUnit (importé globalement par le .csproj) a aussi un type TestResult.
using ArchTestResult = NetArchTest.Rules.TestResult;

namespace Paddock.ArchitectureTests;

internal static class ArchAssert
{
    // Échec lisible : la liste des types fautifs, avec l'explication de NetArchTest quand elle existe.
    public static void Passes(ArchTestResult result)
    {
        var failures = (result.FailingTypes ?? []).Select(type => $"- {type.FullName} {type.Explanation}".TrimEnd());

        Assert.True(result.IsSuccessful, $"Architecture rule broken by:{Environment.NewLine}{string.Join(Environment.NewLine, failures)}");
    }

    // Les règles de modules n'ont rien à vérifier tant qu'aucun module n'existe : le test est marqué « ignoré »
    // (et non réussi), pour que ce vide reste visible dans le résultat de dotnet test.
    public static void SkipWhenNoModule() =>
        Assert.SkipUnless(Solution.HasModules, "No module yet.");
}
