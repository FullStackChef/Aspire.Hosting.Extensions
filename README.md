# Custom Distributed Application Builder with Custom Resource Naming

This project extends the standard distributed application builder to support custom resource naming conventions when provisioning Azure resources. By integrating custom naming logic, you can enforce consistent naming patterns (for example, embedding deployment stages) across your infrastructure.


## Overview

This solution provides a set of components that work together to **Customize resource names:** Apply a naming convention that concatenates a resource abbreviation, a stage identifier, and a custom value.


The key components are:

- **AzureResourceExtensions:**  
  Provides an extension method (`WithCustomNamingConvention`) to apply custom naming conventions on a provisionable resource.  

- **CustomDistributedApplicationBuilder:**  
  A specialized builder that extends the default `DistributedApplicationBuilder` and injects a stage parameter. It also registers the custom resource name property resolver.  

- **CustomResourceNameExpression:**  
  Encapsulates the custom naming expression by combining a resource abbreviation with a user-defined identifier.  

- **CustomResourceNamePropertyResolver:**  
  Implements the logic to resolve and interpolate the final resource name. It checks for allowed characters to determine a suitable separator and uses a stage parameter to build a name like:  
  `<resourceAbbreviation><separator><stage><separator><identifier>`  

- **CustomResourceNameProvisioningOutput:**  
  Wraps the output of the naming resolution process as a provisioning output, making the resolved name available for further consumption.  

---

## Example Usage

Below is a sample snippet demonstrating how to use the builder and apply a custom naming convention to an Azure Cosmos DB resource:

```csharp
var builder = CustomDistributedApplicationBuilder.CreateBuilder(args);

builder.AddAzureCosmosDB("cosmosdb")
       .ConfigureInfrastructure(infr =>
       {
           if (infr.GetProvisionableResources().OfType<CosmosDBAccount>().SingleOrDefault() is CosmosDBAccount cosmosDBAccount)
           {
               cosmosDBAccount.WithCustomNamingConvention("cosmos", "playground");
           };
       })
       .AddCosmosDatabase("shop")
       .AddContainer("customers", "/partitionKey");
```

### What Happens Behind the Scenes

- **Stage Parameter Injection:**  
  The `CustomDistributedApplicationBuilder` injects a stage parameter during construction, ensuring that all resources are tagged with the correct deployment stage.

- **Custom Naming Resolution:**  
  The extension method `WithCustomNamingConvention` creates a `CustomResourceNameExpression` which is later resolved by the `CustomResourceNamePropertyResolver`. This resolver:
  - Checks if the resource has a custom naming expression.
  - Determines the valid separator (e.g., hyphen, underscore, period) based on the resource’s naming constraints.
  - Constructs a final name by interpolating the resource abbreviation, the stage, and the custom identifier.

- **Provisioning Output:**  
  If applicable, the resolved name is wrapped in a `CustomResourceNameProvisioningOutput` for consumption in downstream provisioning steps.

---

## Getting Started

### Prerequisites

- .NET SDK (as required by your project)
- Dependencies on `Aspire.Hosting`, `Azure.Provisioning`, and related libraries.

### Installation

1. Clone or add this project to your solution.
2. Ensure that your project references the necessary Azure provisioning packages.
3. Integrate the custom builder in your application startup by replacing or extending your existing `DistributedApplicationBuilder`.

### Running the Application

After configuring your builder as shown in the example usage, build and run your application. The custom naming resolver will automatically format resource names based on the provided conventions.

---

## Customization

- **Custom Abbreviations and Identifiers:**  
  Modify the parameters passed to `WithCustomNamingConvention` to suit your naming standards.

- **Naming Separators:**  
  The resolver dynamically chooses a separator based on allowed characters. Adjust the logic in `CustomResourceNamePropertyResolver` if you require a different behavior.

- **Stage Parameter:**  
  The `stage` parameter is injected via the builder. You can configure it to match your deployment environments (e.g., development, staging, production).
