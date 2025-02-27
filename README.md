# Distributed Application Builder Extensions for Custom Resource Naming

This project provides an extension method that integrates custom resource naming conventions into your distributed application provisioning pipeline. With this approach, you can configure multiple parameters that contribute to the final resource names and supply a custom interpolation handler to generate names that adhere to your standards.

---

## Overview

The extension method adds a set of parameters to your builder and registers a custom resource name property resolver. The resolver constructs the final resource name by combining a resource abbreviation, one or more dynamic parameters, and a custom identifier. The name is then interpolated using a delegate provided by the consumer, ensuring that naming follows a consistent pattern based on allowed characters (e.g., hyphen, underscore, or period).

### Key Features

- **Multiple Parameter Support:**  
  Define a collection of parameter names (e.g., "stage", "environment") that are injected into your builder and used during name resolution.

- **Custom Interpolation Handler:**  
  Provide a delegate to control how the final resource name is interpolated. This handler receives the dynamic parameters, a chosen separator, the resource abbreviation, and a custom identifier to build the complete name.

- **Consistent Naming Conventions:**  
  Ensures all resource names are generated following a consistent format, such as:  
  `<resourceAbbreviation><separator><parameter1><separator><parameter2><separator><identifier>`

---

## Example Usage

Below is a sample snippet demonstrating how to integrate the extension method into your provisioning pipeline:

```csharp
var builder = DistributedApplicationBuilder.CreateBuilder(args);


// Configure the custom naming convention by specifying the parameter names ("stage" and "territory")
// and providing a lambda function that defines how the final resource name should be built.
// The lambda receives the dictionary of provisioning parameters, a separator string,
// the resource abbreviation, and a custom identifier, then returns the interpolated name.
builder.WithCustomNamingConvention(["stage", "territory"], (parameters, separator, abbrieviation, identifier) =>
                $"{abbrieviation}{separator}{parameters["territory"]}{separator}{parameters["stage"]}{separator}{identifier}");



builder.AddAzureCosmosDB("cosmosdb")
       .AddNameProperties(resources => resources.OfType<CosmosDBAccount>().FirstOrDefault(), "cosmos", "playground")
       .AddCosmosDatabase("shop")
       .AddContainer("customers", "/partitionKey");
```

### How It Works

1. **Parameter Injection:**  
   The extension method accepts a collection of parameter names. For each provided name, a corresponding parameter is added to the builder. These parameters are later used by the naming resolver to construct the resource name.

2. **Custom Resource Name Resolver:**  
   The method registers a custom resource name property resolver (`CustomResourceNamePropertyResolver`) at the start of the provisioning resolver chain. This resolver:
   - Checks for a custom resource name expression on the resource.
   - Uses the provided parameters and the custom interpolation handler to build the final resource name.
   - Selects an appropriate separator (hyphen, underscore, or period) based on the resource’s allowed characters.

3. **Resource Name Interpolation:**  
   The handler delegate is invoked with:
   - A dictionary of provisioning parameters.
   - The chosen separator.
   - The resource abbreviation (from the custom resource name expression).
   - The custom identifier.
   
   The delegate then constructs a `BicepInterpolatedStringHandler` that defines the final resource name.

---

## Getting Started

### Prerequisites

- .NET SDK (as required by your project)
- References to:
  - `Aspire.Hosting.ApplicationModel`
  - `Azure.Provisioning`
  - `Azure.Provisioning.Expressions`
  - `Microsoft.Extensions.DependencyInjection`
- Other dependencies as needed by your project

### Installation

1. **Clone or Integrate:**  
   Add this project to your solution or incorporate the provided files into your existing codebase.

2. **Reference the Extension:**  
   Ensure your project references the `DistributedApplicationBuilderExtensions` class to enable the custom naming extension.

3. **Configure the Builder:**  
   Update your application startup code to use the `WithCustomNamingConvention` extension method with the appropriate parameter names and handler delegate.

### Running the Application

After configuring your builder, build and run your application. The custom naming resolver will automatically generate resource names based on the provided parameters and custom logic during the provisioning process.

---

## Customization

- **Parameter Names:**  
  Modify the list of parameter names to suit your organizational requirements.

- **Interpolation Logic:**  
  Update the handler delegate to control how the final resource name is constructed. This flexibility allows you to incorporate additional parameters or alter the formatting as needed.

- **Naming Requirements:**  
  The resolver dynamically selects a separator based on the allowed characters for the resource. Adjust this logic within the resolver if your naming rules change.

---
