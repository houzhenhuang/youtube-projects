var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("db")
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("notes-db");

builder.AddProject<Projects.Notes_Api>("notes-api")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
