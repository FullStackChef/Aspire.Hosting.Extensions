using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Azure;
using Azure.Provisioning;
using Azure.Provisioning.Expressions;
using Azure.Provisioning.Primitives;

namespace Aspire.Hosting;

internal class CustomResourceNamePropertyResolver(IReadOnlyList<ParameterResource> parameters,
     Func<IReadOnlyDictionary<string, ProvisioningParameter>, string, BicepValue<string>, string, BicepInterpolatedStringHandler> handlerFunction) :
    DynamicResourceNamePropertyResolver
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
            name.Kind == BicepValueKind.Expression && name.Expression is CustomResourceNameExpression customResourceName)
        {
            string separator = requirements.ValidCharacters.HasFlag(ResourceNameCharacters.Hyphen) ? "-" :
                               requirements.ValidCharacters.HasFlag(ResourceNameCharacters.Underscore) ? "_" :
                               requirements.ValidCharacters.HasFlag(ResourceNameCharacters.Period) ? "." : string.Empty;


            Dictionary<string, ProvisioningParameter> provisioning = parameters.ToDictionary(p => p.Name,
                                                                                             p => p.AsProvisioningParameter(infrastructure));

            var interpolationHandler = handlerFunction(provisioning, separator, customResourceName.Abbreviation, customResourceName.Value);

            return BicepFunction.Interpolate(interpolationHandler);
        }

        return base.ResolveName(options, resource, requirements);
    }
}
