using Aspire.Hosting.ApplicationModel;
using Azure.Provisioning;
using Azure.Provisioning.Primitives;

namespace Aspire.Hosting;

public static class AzureResourceExtensions
{

    public static void WithCustomNamingConvention(this ProvisionableResource resource, string abbreviation, string identifier)
    {
        var expression = new CustomResourceNameExpression(abbreviation, identifier);
        if (resource.ProvisionableProperties.TryGetValue(nameof(Resource.Name), out IBicepValue? name))
        {
            resource.SetProvisioningProperty(name, new BicepValue<string>(expression));
        }
    }
}
