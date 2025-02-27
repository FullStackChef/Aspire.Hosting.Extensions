using Azure.Provisioning.CosmosDB;


var builder = DistributedApplication.CreateBuilder(args);

builder.AddCustomNamingConvention(["stage", "territory"], (parameters, separator, abbrieviation, identifier) =>
                $"{abbrieviation}{separator}{parameters["territory"]}{separator}{parameters["stage"]}{separator}{identifier}");


builder.AddAzureCosmosDB("cosmosdb")
       .AddNameProperties(resources => resources.OfType<CosmosDBAccount>().FirstOrDefault(), "cosmos", "playground")
       .AddCosmosDatabase("shop")
       .AddContainer("customers", "/partitionKey");

var apiService = builder.AddProject<Projects.AspirePlayground_ApiService>("apiservice");

builder.AddProject<Projects.AspirePlayground_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
