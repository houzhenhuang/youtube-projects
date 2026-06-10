var builder = DistributedApplication.CreateBuilder(args);


var postgres = builder.AddPostgres("contentplatform-db")
    .WithPgAdmin();

var rabbitmq = builder.AddRabbitMQ("contentplatform-mq", builder.AddParameter("username", "guest"), builder.AddParameter("password", "guest"))
    .WithManagementPlugin();


var reportingApi = builder.AddProject<Projects.ContentPlatform_Reporting_Api>("contentplatform-reporting-api")
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WaitFor(postgres)
    .WaitFor(rabbitmq);

var api = builder.AddProject<Projects.ContentPlatform_Api>("contentplatform-api")
    .WithHttpsEndpoint(port: 5001)
    .WithHttpEndpoint(port: 5000)
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WithReference(reportingApi)
    .WaitFor(postgres)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.ContentPlatform_Presentation>("contentplatform-presentation")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
