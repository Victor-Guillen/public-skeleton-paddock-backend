var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImageTag("18")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

var database = postgres.AddDatabase("paddock");

var keycloakAdmin = builder.AddParameter("keycloak-admin");
var keycloakAdminPassword = builder.AddParameter("keycloak-admin-password", secret: true);

var keycloak = builder.AddKeycloak("keycloak", 8080, keycloakAdmin, keycloakAdminPassword)
    .WithRealmImport("../../infra/keycloak/realms");

builder.AddProject<Projects.Paddock_Api>("api")
    .WithReference(database)
    .WaitFor(database)
    .WaitFor(keycloak);

builder.Build().Run();
