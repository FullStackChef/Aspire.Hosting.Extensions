using Azure.Provisioning;
using Azure.Provisioning.Primitives;

namespace Aspire.Hosting;

/// <summary>
/// Represents the provisioning output for a custom resource name.
/// This output encapsulates the provisionable resource along with its resolved naming.
/// </summary>
/// <param name="bicepIdentifier">The identifier used in the Bicep template for this output.</param>
/// <param name="type">The type of the output value.</param>
/// <param name="resource">The provisionable resource associated with this naming output.</param>
internal class CustomResourceNameProvisioningOutput(string bicepIdentifier, Type type, ProvisionableResource resource)
    : ProvisioningOutput(bicepIdentifier, type)
{
    /// <summary>
    /// Gets the provisionable resource associated with this custom resource name output.
    /// </summary>
    internal ProvisionableResource Resource => resource;
}
