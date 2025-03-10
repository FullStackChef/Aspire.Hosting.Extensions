using Aspire.Hosting;
using Azure.Provisioning.CosmosDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;


var builder = DistributedApplication.CreateBuilder(args);

builder.AddCustomNamingConvention(["stage", "territory"], (parameters, separator, abbrieviation, identifier) =>
                $"{abbrieviation}{separator}{parameters["territory"]}{separator}{parameters["stage"]}{separator}{identifier}");


builder.AddAzureCosmosDB("cosmosdb")
       .AddNameProperties(resources => resources.OfType<CosmosDBAccount>().FirstOrDefault(), "cosmos", "playground")
       .AddCosmosDatabase("shop")
       .AddContainer("customers", "/partitionKey");

var apiService = builder.AddProject<Projects.AspirePlayground_ApiService>("apiservice");

//builder.AddWebApplication("simpleapi", static builder => builder.AddServiceDefaults())
//       .WithRouteEndpoints(static app => app.MapGet("/hello-world", () => "Hi there"));


builder.AddAzureSqlServer("sqlserver").RunAsContainer().AddDatabase("database", "mydb");

Packages.Aspire_Hosting_Azure_Sql

builder.AddProject<Projects.AspirePlayground_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();

