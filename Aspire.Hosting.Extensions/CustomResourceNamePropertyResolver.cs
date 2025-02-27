using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Azure;
using Azure.Provisioning;
using Azure.Provisioning.Expressions;
using Azure.Provisioning.Primitives;

namespace Aspire.Hosting;

internal class CustomResourceNamePropertyResolver(IDistributedApplicationBuilder builder) : DynamicResourceNamePropertyResolver
{
    public override void ResolveProperties(ProvisionableConstruct construct, ProvisioningBuildOptions options)
    {
        if (construct is ProvisionableResource resource
            && resource.ProvisionableProperties.TryGetValue(nameof(Resource.Name), out IBicepValue? name)
            && (name.Kind == BicepValueKind.Expression && name.Expression is CustomResourceNameExpression || name.Kind == BicepValueKind.Unset)
            && !name.IsOutput)
        {

            if (ResolveName(options, resource, resource.GetResourceNameRequirements()) is BicepValue<string> resolved)
            {
                construct.SetProvisioningProperty(name, resolved);
            }
        }
        if (construct is CustomResourceNameProvisioningOutput output)
        {
            if (ResolveName(options, output.Resource, output.Resource.GetResourceNameRequirements()) is BicepValue<string> resolved)
            {
                output.Value = resolved;
            }
        }
    }
    public override BicepValue<string>? ResolveName(ProvisioningBuildOptions options, ProvisionableResource resource, ResourceNameRequirements requirements)
    {

        if (resource.ParentInfrastructure is AzureResourceInfrastructure infrastructure &&
            resource.ProvisionableProperties.TryGetValue(nameof(Resource.Name), out IBicepValue? name) &&
            builder.Resources.FirstOrDefault(r => r.Name == "stage") is ParameterResource stageParam &&
            name.Kind == BicepValueKind.Expression && name.Expression is CustomResourceNameExpression customResourceName)
        {
            string separator = requirements.ValidCharacters.HasFlag(ResourceNameCharacters.Hyphen) ? "-" :
                               requirements.ValidCharacters.HasFlag(ResourceNameCharacters.Underscore) ? "_" :
                               requirements.ValidCharacters.HasFlag(ResourceNameCharacters.Period) ? "." : string.Empty;

            var resourceType = customResourceName.ResourceAbbreviation;
            var stage = stageParam.AsProvisioningParameter(infrastructure);

            return BicepFunction.Interpolate($"{resourceType}{separator}{stage}{separator}{customResourceName.Value}");
        }
        return base.ResolveName(options, resource, requirements);
    }
}
