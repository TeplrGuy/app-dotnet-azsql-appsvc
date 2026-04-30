# Package Version Contract: .NET 10 Migration

**Feature**: 001-net6-to-net10-migration
**Date**: 2026-04-27
**Purpose**: Defines the exact package versions and removals for each project.

## Contract: ContosoUniversity.API.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <UserSecretsId>008cb285-7653-4d63-afcc-02830f4a6eb2</UserSecretsId>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Azure.Extensions.AspNetCore.Configuration.Secrets" Version="1.3.2" />
    <PackageReference Include="Azure.Identity" Version="1.13.2" />
    <PackageReference Include="Bogus" Version="35.6.1" />
    <PackageReference Include="Microsoft.ApplicationInsights.AspNetCore" Version="2.22.0" />
    <PackageReference Include="Microsoft.Data.SqlClient" Version="6.0.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.Extensions.Azure" Version="1.8.0" />
    <PackageReference Include="Swashbuckle.AspNetCore.SwaggerUI" Version="7.2.0" />
  </ItemGroup>

</Project>
```

### Removed Packages (API)

| Package | Reason |
|---------|--------|
| Microsoft.AspNetCore.Mvc.NewtonsoftJson | Replaced by built-in System.Text.Json |
| Microsoft.AspNetCore.Razor.Language | Part of shared framework |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | Dev-time only; not needed at build |
| NSwag.AspNetCore | Replaced by built-in OpenAPI + SwaggerUI |
| System.Runtime.Extensions | Built into runtime |

---

## Contract: ContosoUniversity.WebApplication.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <UserSecretsId>ba41875f-c956-4fad-87e5-4b75230e53bc</UserSecretsId>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.ApplicationInsights.AspNetCore" Version="2.22.0" />
  </ItemGroup>

</Project>
```

### Removed Packages (WebApplication)

| Package | Reason |
|---------|--------|
| Microsoft.AspNetCore.Razor.Language | Part of shared framework |
| Microsoft.Extensions.DependencyInjection | Part of shared framework |
| Microsoft.Extensions.DependencyInjection.Abstractions | Part of shared framework |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | Dev-time only |
| Newtonsoft.Json | Replaced by built-in System.Text.Json |

---

## Contract: ContosoUniversity.Test.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="MSTest.TestAdapter" Version="3.7.0" />
    <PackageReference Include="MSTest.TestFramework" Version="3.7.0" />
    <PackageReference Include="coverlet.collector" Version="6.0.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

</Project>
```

---

## Contract: ContosoUniversity.CodedUITest.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.12.0" />
    <PackageReference Include="MSTest.TestAdapter" Version="3.7.0" />
    <PackageReference Include="MSTest.TestFramework" Version="3.7.0" />
    <PackageReference Include="Selenium.WebDriver" Version="4.27.0" />
  </ItemGroup>

</Project>
```

### Removed Packages (CodedUITest)

| Package | Reason |
|---------|--------|
| Selenium.WebDriver.ChromeDriver | Selenium.Manager (built into WebDriver 4.6+) handles driver downloads automatically |

---

## Contract: global.json (both locations)

```json
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestFeature"
  }
}
```

---

## Contract: Dockerfile Base Images

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
```

---

## Contract: Bicep (infra/resources.bicep)

```bicep
siteConfig: {
  netFrameworkVersion: 'v10.0'
  // ...existing properties unchanged
}
```

---

## Contract: CI/CD Pipeline

```yaml
env:
  DOTNET_VERSION: '10.0.x'
```
