using Aspire.Hosting.Azure;
using Azure.Provisioning;
using Azure.Provisioning.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods to add custom resource naming conventions to the <see cref="IDistributedApplicationBuilder"/> .
/// </summary>
public static class DistributedApplicationBuilderExtensions
{
    /// <summary>
    /// Configures the distributed application builder to use a custom naming convention for resource provisioning.
    /// This method adds the specified parameters to the builder and registers a custom resource name property resolver.
    /// </summary>
    /// <param name="builder">The distributed application builder to extend.</param>
    /// <param name="parametersNames">
    /// A collection of parameter names that will be added to the builder. Each parameter is used to generate provisioning parameters
    /// for custom resource naming.
    /// </param>
    /// <param name="handler">
    /// A delegate that accepts a dictionary of provisioning parameters, a separator string, a resource abbreviation as a <see cref="BicepValue{string}"/>,
    /// and a custom identifier string, and returns a <see cref="BicepInterpolatedStringHandler"/> for interpolating the final resource name.
    /// </param>
    /// <returns>
    /// The original <see cref="IDistributedApplicationBuilder"/> instance with the custom naming convention configured.
    /// </returns>
    public static IDistributedApplicationBuilder AddCustomNamingConvention(
        this IDistributedApplicationBuilder builder,
        IEnumerable<string> parametersNames,
        Func<IReadOnlyDictionary<string, ProvisioningParameter>, string, BicepValue<string>, string, BicepInterpolatedStringHandler> handler)
    {
        var parameters = parametersNames.Select(p => builder.AddParameter(p).Resource).ToList();

        builder.Services.Configure<AzureProvisioningOptions>(options =>
            options.ProvisioningBuildOptions.InfrastructureResolvers.Insert(0,
            new CustomResourceNamePropertyResolver(parameters, handler)));

        return builder;
    }
}
