using Azure.Provisioning;
using Azure.Provisioning.Expressions;

namespace Aspire.Hosting;

internal class CustomResourceNameExpression(BicepValue<string> resourceAbbreviation, string value) : StringLiteralExpression(value)
{
    public BicepValue<string> ResourceAbbreviation => resourceAbbreviation;
}
