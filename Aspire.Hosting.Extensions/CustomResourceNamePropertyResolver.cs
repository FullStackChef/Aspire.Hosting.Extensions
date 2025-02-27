using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Azure;
using Azure.Provisioning;
using Azure.Provisioning.Expressions;
using Azure.Provisioning.Primitives;

namespace Aspire.Hosting
{
    /// <summary>
    /// Resolves custom resource name properties by applying a naming convention based on provided parameters and a handler function.
    /// This resolver checks for custom resource name expressions and interpolates the final resource name accordingly.
    /// </summary>
    /// <param name="parameters">
    /// A list of <see cref="ParameterResource"/> objects that will be used to generate provisioning parameters.
    /// </param>
    /// <param name="handlerFunction">
    /// A function that accepts a dictionary of provisioning parameters, a separator string, a resource abbreviation as a <see cref="BicepValue{string}"/>,
    /// and a custom identifier string, and returns a <see cref="BicepInterpolatedStringHandler"/> for name interpolation.
    /// </param>
    internal class CustomResourceNamePropertyResolver(
        IReadOnlyList<ParameterResource> parameters,
        Func<IReadOnlyDictionary<string, ProvisioningParameter>, string, BicepValue<string>, string, BicepInterpolatedStringHandler> handlerFunction)
        : DynamicResourceNamePropertyResolver
    {
        /// <summary>
        /// Resolves the properties of a provisionable construct by applying custom naming logic.
        /// If the construct is a provisionable resource with an unset or custom resource name expression,
        /// this method resolves and sets the final resource name.
        /// It also handles <see cref="CustomResourceNameProvisioningOutput"/> by assigning the resolved name.
        /// </summary>
        /// <param name="construct">The provisionable construct whose properties are to be resolved.</param>
        /// <param name="options">The build options influencing the provisioning process.</param>
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

        /// <summary>
        /// Resolves the name of a provisionable resource by constructing an interpolated name using custom naming logic.
        /// The method checks for a custom resource name expression and, if found, builds the final name using the provided handler function.
        /// </summary>
        /// <param name="options">The build options used during provisioning.</param>
        /// <param name="resource">The provisionable resource whose name is to be resolved.</param>
        /// <param name="requirements">The naming requirements specifying allowed characters and formatting rules.</param>
        /// <returns>
        /// A <see cref="BicepValue{string}"/> representing the resolved resource name if successful;
        /// otherwise, the base implementation is invoked.
        /// </returns>
        public override BicepValue<string>? ResolveName(ProvisioningBuildOptions options, ProvisionableResource resource, ResourceNameRequirements requirements)
        {
            if (resource.ParentInfrastructure is AzureResourceInfrastructure infrastructure &&
                resource.ProvisionableProperties.TryGetValue(nameof(Resource.Name), out IBicepValue? name) &&
                name.Kind == BicepValueKind.Expression && name.Expression is CustomResourceNameExpression customResourceName)
            {
                string separator = requirements.ValidCharacters.HasFlag(ResourceNameCharacters.Hyphen) ? "-" :
                                   requirements.ValidCharacters.HasFlag(ResourceNameCharacters.Underscore) ? "_" :
                                   requirements.ValidCharacters.HasFlag(ResourceNameCharacters.Period) ? "." : string.Empty;

                Dictionary<string, ProvisioningParameter> provisioning = parameters.ToDictionary(
                    p => p.Name,
                    p => p.AsProvisioningParameter(infrastructure));

                var interpolationHandler = handlerFunction(provisioning, separator, customResourceName.Abbreviation, customResourceName.Value);

                return BicepFunction.Interpolate(interpolationHandler);
            }

            return base.ResolveName(options, resource, requirements);
        }
    }
}
