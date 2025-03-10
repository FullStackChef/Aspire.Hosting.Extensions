using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.AspNetCore.Builder;
using System.Linq.Expressions;

namespace Aspire.Hosting;
public static class DistributedApplicationBuilderExtensions
{
    public static IResourceBuilder<Resource> AddWebApplication(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name,
        Expression<Action<WebApplicationBuilder>> configureApplication)
    {
        builder.Services.TryAddLifecycleHook<WebApplicationBuilderAppLifecycle>();
        var resource = new WebApplicationResource(name, configureApplication);
        return builder.AddResource(resource).WithHttpEndpoint(targetPort: 25551);
    }

    public static IResourceBuilder<Resource> WithRouteEndpoints(
        this IResourceBuilder<Resource> builder,
        Expression<Action<WebApplication>> configureApplication)
    {
        var annotation = new WithRouteEndpointsAnnotation(configureApplication);
        return builder.WithAnnotation(annotation).WithInitialState(new CustomResourceSnapshot
        {
            Properties = [],
            ResourceType = "WebApplication",
            State = KnownResourceStates.NotStarted
        });
    }
}

public class WebApplicationResource(string name, Expression<Action<WebApplicationBuilder>> configureApplicationBuilder) :
    Resource(name), IResourceWithEndpoints
{
    public Expression<Action<WebApplicationBuilder>> ConfigureApplication { get; } = configureApplicationBuilder;

}
public class WithRouteEndpointsAnnotation(Expression<Action<WebApplication>> configureApplication) : IResourceAnnotation
{
    public Expression<Action<WebApplication>> ConfigureApplication { get; } = configureApplication;
}
public class WebApplicationBuilderAppLifecycle(ResourceNotificationService resourceNotificationService) : IDistributedApplicationLifecycleHook
{
    public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {


        // either generate the web application on the fly or perhaps generate an in-memory project
        var applications = appModel.Resources.OfType<WebApplicationResource>();

        List<Task> apps = [];
        // generate on the fly
        foreach (var application in applications)
        {
            var state = application.TryGetLastAnnotation<ResourceSnapshotAnnotation>(out var snapshot);

            application.TryGetAnnotationsOfType<WithRouteEndpointsAnnotation>(out var annotations);

            //var webApplicationBuilder = WebApplication.CreateBuilder();
            //webApplicationBuilder.WebHost.UseUrls("http://localhost:25551");
            var configuration = application.ConfigureApplication.Body.ToString();

            Console.WriteLine(configuration);
            //var app = webApplicationBuilder.Build();

            //foreach (var annotation in annotations ?? [])
            //{
            //    annotation.ConfigureApplication(app);
            //}


            resourceNotificationService.PublishUpdateAsync(application,
                (snapshot) => snapshot with
                {
                    State = KnownResourceStates.Running
                });

        }


        // dotnet new webapi -n {name}
        // dotnet add package <PackageName> [--version <Version>]
        // write program.cs based on configure application
        // Add project resource to appModel based on existing api
        return Task.CompletedTask;
    }

}
