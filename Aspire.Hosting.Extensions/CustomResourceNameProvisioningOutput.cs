using Azure.Provisioning;
using Azure.Provisioning.Primitives;

namespace Aspire.Hosting;

internal class CustomResourceNameProvisioningOutput(string bicepIdentifier, Type type, ProvisionableResource resource) : ProvisioningOutput(bicepIdentifier, type)
{
    internal ProvisionableResource Resource => resource;
}
