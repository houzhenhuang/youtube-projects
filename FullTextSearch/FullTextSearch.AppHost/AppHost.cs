var builder = DistributedApplication.CreateBuilder(args);

var blogsDb = builder.AddPostgres("blogs-db")
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("blogs");

builder.AddProject<Projects.Blogs_Api>("blogs-api")
    .WithReference(blogsDb)
    .WaitFor(blogsDb);

builder.Build().Run();
