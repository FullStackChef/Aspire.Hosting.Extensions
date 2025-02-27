using Azure.Provisioning;
using Azure.Provisioning.Expressions;

namespace Aspire.Hosting
{
    /// <summary>
    /// Represents a custom resource name expression that extends a string literal expression.
    /// It combines a resource abbreviation with an identifier to facilitate custom naming conventions.
    /// </summary>
    /// <param name="resourceAbbreviation">
    /// A <see cref="BicepValue{T}"/> representing the abbreviation for the resource.
    /// </param>
    /// <param name="identifier">
    /// The string identifier that will be used as part of the resource name.
    /// </param>
    internal class CustomResourceNameExpression(BicepValue<string> resourceAbbreviation, string identifier)
        : StringLiteralExpression(identifier)
    {
        /// <summary>
        /// Gets the resource abbreviation used in the custom resource name expression.
        /// </summary>
        public BicepValue<string> Abbreviation => resourceAbbreviation;
    }
}
