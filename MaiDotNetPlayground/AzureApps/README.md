# Azure Apps examples
Azure enabled apps.



## Azure Function Project structure
A .NET project for Azure Functions using the isolated worker model is basically a .NET console app project that targets a supported .NET runtime. The following are the basic files required in any .NET isolated project:

- C# project file (.csproj) that defines the project and dependencies.
- Program.cs file that's the entry point for the app.
- Any code files defining your functions.
- host.json file that defines configuration shared by functions in your project.
- local.settings.json file that defines environment variables used by your project when run locally on your machine.

## NuGet Packages

### Azure Basic packages
- Azure.Identity
- Microsoft.Azure.AppConfiguration.Functions.Worker  
  - Microsoft.Azure.AppConfiguration.AspNetCore is designed for ASP.NET Core applications, while Microsoft.Azure.AppConfiguration.Functions.Worker is intended for use with Azure Functions (specifically, the .NET Isolated worker model).

### Azure Function Core packages
[Azure Function Documentation](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=hostbuilder%2Cwindows)

The following packages are required to run your .NET functions in an isolated worker process:

- Microsoft.Azure.Functions.Worker
- Microsoft.Azure.Functions.Worker.Sdk

### Other Packags for Azure Functions

- Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore

