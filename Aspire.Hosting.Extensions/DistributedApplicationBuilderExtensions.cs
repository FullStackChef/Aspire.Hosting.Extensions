using Aspire.Hosting.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace Aspire.Hosting;

public static class DistributedApplicationBuilderExtensions
{
    public static IDistributedApplicationBuilder WithCustomNamingConvention(this IDistributedApplicationBuilder builder)
    {
        builder.AddParameter("stage");
        builder.Services.Configure<AzureProvisioningOptions>(options =>
            options.ProvisioningBuildOptions.InfrastructureResolvers.Insert(0,
            new CustomResourceNamePropertyResolver(builder)));
        return builder;
    }
}
