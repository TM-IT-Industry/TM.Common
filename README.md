# TM.Common.Models

A collection of common models and base entities for TM applications.

## 📦 NuGet Package

This project is available as a NuGet package:

# BUILD PACKAGE LOCALLY
- `dotnet build -c Release`
- `dotnet pack -c Release`


# BUILD PACKAGE IN NUGET.ORG
- cmd

 `dotnet nuget push "C:\Users\Ken Napura\source\repos\TM.Common.Models\TM.Common.EntityFrameworkCore\bin\Release\TM.Common.EntityFrameworkCore.1.0.0.nupkg" --api-key %Nuget_APIKey% --source https://api.nuget.org/v3/index.json`

- powershell

 `dotnet nuget push "C:\Users\Ken Napura\source\repos\TM.Common.Models\TM.Common.EntityFrameworkCore\bin\Release\TM.Common.EntityFrameworkCore.1.0.0.nupkg" --api-key $env:Nuget_APIKey --source https://nuget.org`
