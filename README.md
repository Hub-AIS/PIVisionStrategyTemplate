# PIVision Strategy Development Template

This repository provides a template and examples for developing **PIVision Strategy Plugins** for the **ConsoleLabs PIVision Importer** application. These plugins allow for custom logic during the migration and symbol configuration process.

## Overview

The ConsoleLabs PIVision Importer uses these strategy plugins (DLLs) to determine how symbols should be configured and mapped to PI tags during the import process. This repository contains:

- **PIVisionStrategy Template**: A reusable `dotnet` project template to quickly bootstrap new strategy development.
- **PIDA_L1 Example**: A reference implementation of a symbol configuration strategy.
- **Test Data**: Sample assets and configuration files to validate your strategies.

---

## Repository Contents

### Projects
- **`PIVisionStrategy/`**: The core project template. It contains the boilerplate code and configuration needed to create a new plugin.
- **`PIDA_L1/`**: A concrete implementation example. It demonstrates how to:
    - Clone and modify symbol templates.
    - Map DCS tags to PI tags using a project data table.
    - Set symbol geometry (Top, Left).
    - Configure data sources for PIVision symbols.

### Folders
- **`Libs/`**: Contains shared dependencies required by the strategies (e.g., `PIAccessExtension.dll`).
- **`testdata/`**: Essential for local development and testing:
    - `sample.AML`: Asset Modeling Language file for hierarchy.
    - `projectData.xlsx`: Tag mapping table (DCS TAG -> PI TAG).
    - `MigrationConfig.xlsx`: Main configuration for the importer.
    - `Templates/`: JSON-based symbol templates used as base configurations.
- **`.vscode/`**: Contains `launch.json` for debugging support in VS Code.

---

## Getting Started

### 1. Install the Project Template
To make it easy to create new strategies, install the provided template:
```bash
dotnet new install ./PIVisionStrategy
```

### 2. Create a New Strategy Project
```bash
dotnet new pivision-strategy -n MyCustomStrategy
```

---

## Development Workflow

### Building and Deployment
The projects are configured with a **Post-Build event** that automatically deploys the compiled DLL to the ConsoleLabs extensions folder.

> **Crucial:** You must update the `<ConsollabsAppsFolder>` path in your `.csproj` file to match your local installation of ConsoleLabs.

```xml
<PropertyGroup>
  <ConsollabsAppsFolder>C:\Path\To\ConsoleLabs\extensions\PIVisionImporter\extensions\$(ProjectName)</ConsollabsAppsFolder>
</PropertyGroup>
```

### Debugging
Since these are plugins, they must be debugged by attaching to the host process:

1. Launch **ConsoleLabs**.
2. Trigger the PIVision Import process (this starts `PIVisionImporter.exe`).
3. In your IDE (Visual Studio or VS Code), **Attach to Process** and select `PIVisionImporter.exe`.
4. Set breakpoints in your `contract.cs` file.

For detailed step-by-step instructions, refer to the [Developer Guide](DeveloperGuide.md).

---

## Technical Details

Strategies must implement the `ISymbolConfigurator` interface:
```csharp
public JObject Configure(JObject template, SymbolInputData input, DataTable projectDataTable)
```
- **`template`**: The base JSON configuration for the symbol.
- **`input`**: Contains instance-specific data (name, position, properties from AML).
- **`projectDataTable`**: The mapping data loaded from `projectData.xlsx`.
