using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Azure;
using Azure.Provisioning;
using Azure.Provisioning.Primitives;

namespace Aspire.Hosting
{
    /// <summary>
    /// Provides extension methods for configuring Azure resources with custom naming conventions.
    /// </summary>
    public static class AzureResourceExtensions
    {
        /// <summary>
        /// Applies a custom naming convention to the first provisionable resource selected by the provided <paramref name="resourceSelector"/>.
        /// The resource's name is set using a custom resource name expression that incorporates the specified abbreviation and identifier.
        /// </summary>
        /// <typeparam name="TAzureResource">
        /// The type of the Azure provisioning resource for which the naming convention will be applied.
        /// </typeparam>
        /// <param name="builder">
        /// The resource builder used to configure Azure provisioning resources.
        /// </param>
        /// <param name="resourceSelector">
        /// A function that selects a specific provisionable resource from a collection of provisionable resources.
        /// </param>
        /// <param name="abbreviation">
        /// The abbreviation used as a prefix in the resource name.
        /// </param>
        /// <param name="identifier">
        /// The identifier appended to the resource name.
        /// </param>
        /// <returns>
        /// The same <see cref="IResourceBuilder{TAzureResource}"/> instance with the custom naming convention applied.
        /// </returns>
        /// <remarks>
        /// This method configures the infrastructure by searching for the first resource that matches the criteria defined by the 
        /// <paramref name="resourceSelector"/>. If a matching resource is found, a <see cref="CustomResourceNameExpression"/> is created 
        /// using the provided abbreviation and identifier. The method then updates the resource's name property with this custom expression.
        /// </remarks>
        public static IResourceBuilder<TAzureResource> AddNameProperties<TAzureResource>(
            this IResourceBuilder<TAzureResource> builder,
            Func<IEnumerable<Provisionable>, ProvisionableResource?> resourceSelector,
            string abbreviation,
            string identifier)
            where TAzureResource : AzureProvisioningResource
        {
            // Configure the infrastructure for the current resource builder.
            builder.ConfigureInfrastructure(infrastructure =>
            {
                // Use the provided resource selector function to find a matching provisionable resource.
                if (resourceSelector(infrastructure.GetProvisionableResources()) is ProvisionableResource resource)
                {
                    // Create a custom resource name expression using the provided abbreviation and identifier.
                    var expression = new CustomResourceNameExpression(abbreviation, identifier);

                    // If the resource has a provisionable property for its name, set it using the custom expression.
                    if (resource.ProvisionableProperties.TryGetValue(nameof(Resource.Name), out IBicepValue? name))
                    {
                        resource.SetProvisioningProperty(name, new BicepValue<string>(expression));
                    }
                }
            });

            // Return the original builder instance to allow for fluent chaining.
            return builder;
        }
    }
}
