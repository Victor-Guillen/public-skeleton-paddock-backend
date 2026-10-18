namespace Paddock.IntegrationTests;

// Toutes les classes marquées [Collection(ApiCollection.Name)] partagent la même instance d'ApiFactory.
[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<ApiFactory>
{
    public const string Name = "Api";
}
