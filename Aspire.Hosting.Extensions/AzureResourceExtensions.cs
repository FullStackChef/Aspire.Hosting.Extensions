using Aspire.Hosting.ApplicationModel;
using Azure.Provisioning;
using Azure.Provisioning.Primitives;

namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for configuring Azure resources with custom naming conventions.
/// </summary>
public static class AzureResourceExtensions
{
    /// <summary>
    /// Applies a custom naming convention to the specified provisionable resource by setting its name
    /// using a custom resource name expression.
    /// </summary>
    /// <param name="resource">The provisionable resource to configure.</param>
    /// <param name="abbreviation">The abbreviation used as a prefix in the resource name.</param>
    /// <param name="identifier">The identifier appended to the resource name.</param>
    /// <remarks>
    /// This method creates a <see cref="CustomResourceNameExpression"/> using the provided abbreviation and identifier.
    /// If the resource contains a provisionable property for its name, the method sets this property using the custom expression.
    /// </remarks>
    public static void SetNameProperties(this ProvisionableResource resource, string abbreviation, string identifier)
    {
        var expression = new CustomResourceNameExpression(abbreviation, identifier);
        if (resource.ProvisionableProperties.TryGetValue(nameof(Resource.Name), out IBicepValue? name))
        {
            resource.SetProvisioningProperty(name, new BicepValue<string>(expression));
        }
    }
}
