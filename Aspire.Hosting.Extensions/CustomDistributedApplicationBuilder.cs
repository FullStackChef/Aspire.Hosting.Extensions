using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace Aspire.Hosting;

public class CustomDistributedApplicationBuilder : DistributedApplicationBuilder
{
    internal readonly ParameterResource stage;
    public static IDistributedApplicationBuilder CreateBuilder(string[] args) => new CustomDistributedApplicationBuilder(args);
    private CustomDistributedApplicationBuilder(string[] args) : base(args)
    {
        stage = this.AddParameter(nameof(stage)).Resource;
        Services.Configure<AzureProvisioningOptions>(options =>
            options.ProvisioningBuildOptions.InfrastructureResolvers.Insert(0,
            new CustomResourceNamePropertyResolver(this)));
    }

}
