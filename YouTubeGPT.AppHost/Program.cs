using Microsoft.Extensions.Hosting;
using YouTubeGPT.AppHost;
using YouTubeGPT.Shared;

var builder = DistributedApplication.CreateBuilder(args);

var ai = builder.ExecutionContext.IsPublishMode ?
    builder.AddAzureOpenAI(ServiceNames.OpenAI)
        .AddDeployment(new(builder.Configuration["Azure:AI:ChatDeploymentName"] ?? "gpt-4", "gpt-4", "1106"))
        .AddDeployment(new(builder.Configuration["Azure:AI:EmbeddingDeploymentName"] ?? "text-embedding-ada-003-small", "text-embedding-ada-003-small", "3")) :
    builder.AddConnectionString(ServiceNames.OpenAI);

var pgContainer = builder
    .AddPostgres("postgres")
    .WithPgAdmin();

if (builder.Environment.IsDevelopment())
{
    pgContainer = pgContainer
        .WithImage("pgvector/pgvector")
        .WithImageTag("pg16")
        .WithBindMount("./database", "/docker-entrypoint-initdb.d")
        //.WithBindMount("./data/postgres", "/var/lib/postgresql/data")
        ;
}

var vectorDB = pgContainer.AddDatabase(ServiceNames.VectorDB);
var metadataDB = pgContainer.AddDatabase(ServiceNames.MetadataDB);

builder.AddProject<Projects.YouTubeGPT_Ingestion>("youtubegpt-ingestion")
    .WithReference(ai)
    .WithReference(vectorDB)
    .WithReference(metadataDB)
    .WithConfiguration("Azure:AI:EmbeddingDeploymentName");

// We only want to launch the DB migrator when we're not in publish mode
// otherwise we'll skip it and run DB migrations as part of the CI/CD pipeline.
if (!builder.ExecutionContext.IsPublishMode)
{
    builder.AddProject<Projects.YouTubeGPT_DatabaseMigrator>("youtubegpt-databasemigrator")
        .WithReference(metadataDB);
}

builder.AddProject<Projects.YouTubeGPT_Client>("youtubegpt-client")
    .WithReference(ai)
    .WithReference(metadataDB)
    .WithReference(vectorDB)
    .WithConfiguration("Azure:AI:EmbeddingDeploymentName")
    .WithConfiguration("Azure:AI:ChatDeploymentName");

builder.Build().Run();
