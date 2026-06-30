var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("webhooks");

var queue = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume()
    .WithManagementPlugin();

builder.AddProject<Projects.Webhooks_Api>("webhooks-api")
    .WithReference(database)
    .WithReference(queue)
    .WaitFor(database)
    .WaitFor(queue);

builder.Build().Run();
