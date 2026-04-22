# Developer Guide: PIVision Strategy Development

## Overview
This repository contains the `PIDA_L1` project and a `PIVisionStrategy` template. These are class library (DLL) plugins for consollabs.

> **Note on Paths:** Throughout this guide, `C:\Setup\hub\` is used as an example installation path. Please replace this with the actual path where consollabs is installed on your machine.

---

## Creating a Project from the Template
We provide a project template named **PIVision Strategy** to help you jumpstart new strategy development.

### 1. Install the Template
Run the following command in the root of this repository:
```bash
dotnet new install ./PIVisionStrategy
```

### 2. Create a New Project
You can now create a new project using the template from the CLI, Visual Studio, or VS Code.

#### Via CLI:
```bash
dotnet new pivision-strategy -n MyNewStrategy
```

#### Via Visual Studio:
1. Open Visual Studio and select **Create a new project**.
2. Search for "PIVision Strategy" in the template list.
3. Follow the wizard to create your project.

### 3. Automated DLL Deployment
The project is configured to automatically copy the compiled DLL into the consollabs extensions folder whenever you build.

#### How it works:
The `.csproj` file contains a `PostBuild` target that manages this. **You must update the path in your .csproj file to match your local installation.**

```xml
<Target Name="PostBuild" AfterTargets="PostBuildEvent">
  <PropertyGroup>
    <!-- REPLACE 'C:\Setup\hub\' with your actual installation path -->
    <ConsollabsAppsFolder>C:\Setup\hub\extensions\PIVisionImporter\extensions\$(ProjectName)</ConsollabsAppsFolder>
  </PropertyGroup>
  <ItemGroup>
    <FilesToCopy Include="$(TargetDir)*.*" />
  </ItemGroup>
  <MakeDir Directories="$(ConsollabsAppsFolder)" />
  <Copy SourceFiles="@(FilesToCopy)" DestinationFolder="$(ConsollabsAppsFolder)" />
</Target>
```

#### Verifying Deployment:
1. Build your project in Visual Studio or VS Code.
2. Check the consollabs extensions directory (e.g., `C:\Setup\hub\extensions\PIVisionImporter\extensions\<YourProjectName>`).
3. Ensure the DLL and PDB files are present.

---

## Test Data
Sample data for testing your strategies is available in the `testdata` folder at the root of the solution. This includes:
- **sample.AML**: Asset Modeling Language file.
- **projectData.xlsx**: Excel file containing tag mappings.
- **MigrationConfig.xlsx**: Migration configuration settings.
- **Templates/**: JSON templates for symbols.

---

## Debugging the Plugin
The plugins are loaded into a host process for execution. In this environment:
1. **consollabs:** `C:\Setup\hub\consollabs.exe` is the primary application. (Example path)
2. **The Host Importer:** `consollabs.exe` launches `...\extensions\PIVisionImporter\PIVisionImporter.exe`.
3. **The Plugin:** `PIVisionImporter.exe` is the process that actually loads and executes your DLL plugin.

To debug your code, you must attach your debugger to the `PIVisionImporter.exe` process.

---

## Step-by-Step Debugging Procedure (Visual Studio)

### 1. Build Your Project
Build the `PIDA_L1` (or your new project) in **Debug** configuration. The build will automatically deploy the DLL to the consollabs folder.

### 2. Start consollabs
Manually launch consollabs:
- `C:\Setup\hub\consollabs.exe` (Example path)

### 3. Trigger the Importer
Perform the action within `consollabs.exe` that triggers the migration/import process. This will cause `consollabs.exe` to launch `PIVisionImporter.exe`.

### 4. Attach the Debugger
In Visual Studio:
1. Go to the **Debug** menu and select **Attach to Process...** (or press `Ctrl+Alt+P`).
2. In the "Available processes" list, find **PIVisionImporter.exe**.
   - *Note: If you don't see it, ensure "Show processes from all users" is checked and that you have triggered the import process in the step above.*
3. Click the **Attach** button.

### 5. Debug Your Code
1. Once attached, set breakpoints in your source code.
2. When the execution flow in `PIVisionImporter.exe` reaches your plugin's code, Visual Studio will hit your breakpoints.

---

## Finalizing: Packaging for Release
Once you have finished debugging and your strategy is ready for production:

1. **Build in Release Mode:** Switch your build configuration to **Release** and rebuild the project (`dotnet build -c Release`).
2. **Verify Release Deployment:** Ensure the Release DLLs are correctly copied to the consollabs extension folder by the build process.
3. **Verify Performance:** Always ensure the Release version behaves correctly in the consollabs environment.

---

## Pro-Tip: Automatic Launching (Optional)
If you are frequently debugging and want to automate this, you can configure the `PIDA_L1` project to launch consollabs directly when you press **F5**.

### Configuration
Create a file at `PIDA_L1\Properties\launchSettings.json` with the following content. **Ensure the executablePath matches your machine.**

```json
{
  "profiles": {
    "Debug with consollabs": {
      "commandName": "Executable",
      "executablePath": "C:\\Setup\\hub\\consollabs.exe",
      "workingDirectory": "C:\\Setup\\hub"
    }
  }
}
```

When you use this profile (by pressing F5), Visual Studio will start `consollabs.exe`. You will still need to trigger the action that launches `PIVisionImporter.exe`, and then use the **Attach to Process** method described above to attach to the child process.

---

## Step-by-Step Debugging Procedure (Visual Studio Code)

### 1. Prerequisites
- Ensure the **C# Dev Kit** extension is installed in VS Code.

### 2. Configure Launch Settings
Create or update your `.vscode/launch.json` file with an "Attach" configuration.

#### Example `.vscode/launch.json`:
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Attach (PIVisionImporter)",
      "type": "coreclr",
      "request": "attach",
      "processName": "PIVisionImporter.exe"
    }
  ]
}
```

### 3. Debugging Steps
1. **Start consollabs:** Launch `consollabs.exe` manually from your installation path.
2. **Trigger the Importer:** Perform the action in consollabs that launches `PIVisionImporter.exe`.
3. **Attach in VS Code:**
   - Go to the **Run and Debug** view (`Ctrl+Shift+D`).
   - Select **.NET Core Attach (PIVisionImporter)** from the dropdown.
   - Click the **Start Debugging** (green arrow) button.
   - If prompted, select the `PIVisionImporter.exe` process from the list.
4. **Set Breakpoints:** You can now set breakpoints in your source code. They will be hit as the plugin executes.

