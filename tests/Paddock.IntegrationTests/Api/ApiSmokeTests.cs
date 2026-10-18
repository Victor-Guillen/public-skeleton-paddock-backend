using System.Net;

namespace Paddock.IntegrationTests.Api;

// Tests de fumée : l'API démarre en test, avec son vrai pipeline, et répond.
[Collection(ApiCollection.Name)]
public sealed class ApiSmokeTests(ApiFactory factory)
{
    [Fact]
    public async Task The_api_reports_healthy_in_development()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task The_openapi_document_is_served_in_development()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("\"openapi\"", await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}
