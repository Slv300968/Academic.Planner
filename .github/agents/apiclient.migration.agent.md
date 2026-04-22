# ApiClient Migration Agent

This document describes the steps to migrate services from `Web/Services` folder to a new `ApiClient` project with authentication support.

## Overview

This migration:
- Creates a new `ApiClient` project with `IHttpClientFactory` pattern
- Adds token-based API authentication using `LoginApiHandler` and `TokenApiStateProvider`
- Configures JWT Bearer authentication in the `Api` project
- Creates an `AuthController` for token generation
- Migrates all service classes (except `ResxLocalizer.cs`) from `Web/Services` to `ApiClient`
- Moves `ResxLocalizer.cs` to `Web/Resources`
- Updates `Web` project to reference `ApiClient` and use the new security pattern

---

## Step 0: Configure JWT Authentication in Api Project

Before setting up the ApiClient, you must configure the Api project to support JWT authentication.

### 0.1 Update `Models/MRCAppSettings.cs`

Add the `JwtKey` property:

```csharp
namespace Models;

public class MRCAppSettings
{
	public string ApplicationKey { get; set; }
	public string ClientKey { get; set; }
	public string Environment { get; set; }
	public string Location { get; set; }
	public string LogUrl { get; set; }
	public string UtilitiesApi { get; set; }
	public string UtilitiesApiNF { get; set; }
	public string JwtKey { get; set; }  // <-- Add this
	// ... other properties
}
```

### 0.2 Update `Api/appsettings.json`

Add the `JwtKey` setting in `MRCAppSettings`:

```json
{
	"ConnectionStrings": {
		"YourDatabase": "your-connection-string"
	},
	"MRCAppSettings": {
		"ApplicationKey": "MRC.YourApp.Name",
		"ClientKey": "YourClient",
		"Environment": "DEV",
		"Location": "USA",
		"LogUrl": "http://localhost:44500",
		"UtilitiesApi": "https://devdti.lcred.net/utilities/api/",
		"UtilitiesApiNF": "https://devdti.lcred.net/utilities/apinf/",
		"JwtKey": "your-secure-jwt-key-here"
	},
	"AllowedHosts": "*"
}
```

### 0.3 Update `Api/GlobalUsings.cs`

Add JWT-related using statements:

```csharp
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.IdentityModel.Tokens;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Text;
// ... other existing usings
```

### 0.4 Update `Api/Program.cs`

Add JWT Bearer authentication configuration:

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfigurationSection mrcAppSettingsSection = builder.Configuration.GetSection("MRCAppSettings");
MRCAppSettings mrcAppSettings = mrcAppSettingsSection.Get<MRCAppSettings>();  // Move this to the top

builder.Services.Configure<MRCAppSettings>(mrcAppSettingsSection);
builder.Services.AddControllers().AddJsonOptions(x => { x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; x.JsonSerializerOptions.PropertyNameCaseInsensitive = true; });
// ... other service registrations

// Add JWT Authentication (add before Serilog logger setup)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = mrcAppSettings.ApplicationKey,
		ValidAudience = mrcAppSettings.ApplicationKey,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mrcAppSettings.JwtKey))
	};
});

// ... Serilog logger setup

WebApplication app = builder.Build();
// ... middleware

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();  // <-- Add this BEFORE UseAuthorization
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### 0.5 Create `Api/Controllers/AuthController.cs`

```csharp
namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(ILogger<AuthController> logger, IOptions<MRCAppSettings> mrcAppSettings, UserDL userDL) : ControllerBase
{
	private readonly MRCAppSettings _mrcAppSettings = mrcAppSettings.Value;

	[AllowAnonymous]
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] string userPrincipalName)
	{
		logger.LogInformation("Login(userPrincipalName: {UserPrincipalName})", userPrincipalName);
		User user = await AuthenticateUser(userPrincipalName);
		if (user is null)
			return Unauthorized();

		string token = GenerateJSONWebToken(user);
		return Ok(token);
	}

	private async Task<User> AuthenticateUser(string userPrincipalName)
	{
		User user = await userDL.SelectUser_UserPrincipalName(userPrincipalName);
		return user;
	}

	private string GenerateJSONWebToken(User user)
	{
		string jwtKey = _mrcAppSettings.JwtKey;
		string jwtIssuer = _mrcAppSettings.ApplicationKey;
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
		var claims = new[] {
			new Claim(ClaimTypes.Sid, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Name),
			new Claim(ClaimTypes.Upn, user.UserPrincipalName)
		};
		var JwtSecurityToken = new JwtSecurityToken(jwtIssuer, jwtIssuer, claims, expires: DateTime.UtcNow.AddHours(24), signingCredentials: credentials);
		return new JwtSecurityTokenHandler().WriteToken(JwtSecurityToken);
	}
}
```

> **Note:** Ensure `UserDL` has a `SelectUser_UserPrincipalName` method that returns a `User` object.

---

## Step 1: Create ApiClient Project Structure

Create a new folder `ApiClient` at the solution root level.

### 1.1 Create `ApiClient/ApiClient.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk">

	<PropertyGroup>
		<TargetFramework>net10.0</TargetFramework>
		<ImplicitUsings>enable</ImplicitUsings>
		<Nullable>disable</Nullable>
		<PlatformTarget>x64</PlatformTarget>
	</PropertyGroup>

	<ItemGroup>
		<PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="10.0.0" />
		<PackageReference Include="Microsoft.Extensions.Http" Version="10.0.0" />
		<PackageReference Include="Microsoft.Extensions.Options" Version="10.0.0" />
		<PackageReference Include="MRC.DTI.Dev.Utilities.Business" Version="1.1.30" />
	</ItemGroup>

	<ItemGroup>
		<ProjectReference Include="..\Models\Models.csproj" />
	</ItemGroup>

</Project>
```

> **Note:** Adjust the `TargetFramework` and package versions to match your solution.

### 1.2 Create `ApiClient/GlobalUsings.cs`

```csharp
global using Models;
global using MRC.DTI.Dev.Utilities.Business;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Text;
global using System.Text.Json;
```

### 1.3 Create `ApiClient/HelperApiClient.cs`

```csharp
namespace ApiClient;

public static class HelperApiClient
{
	public static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
}
```

---

## Step 2: Create Authentication Infrastructure

### 2.1 Create `ApiClient/TokenApiStateProvider.cs`

```csharp
namespace ApiClient;

public class TokenApiStateProvider
{
	public string Token { get; set; } // API authentication token for subsequent calls
	public string UserPrincipalName { get; set; } // Logged-in user from Azure AD
	public string Name { get; set; } // User display name (optional)
}
```

### 2.2 Create `ApiClient/LoginApiService.cs`

```csharp
namespace ApiClient;

public interface ILoginApiService
{
	Task<string> AuthenticateAsync();
}

public class LoginApiService(IHttpClientFactory httpClientFactory, TokenApiStateProvider tokenApiStateProvider) : ILoginApiService
{
	public async Task<string> AuthenticateAsync()
	{
		try
		{
			string token = RetrieveCachedToken();
			if (!string.IsNullOrWhiteSpace(token))
				return token;
			HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientNameAnonymous);
			HttpResponseMessage result = await httpClient.PostAsync("auth/login", GenerateBody(tokenApiStateProvider.UserPrincipalName));
			result.EnsureSuccessStatusCode();
			token = await result.Content.ReadAsStringAsync();
			SetCacheToken(token);
			return token;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private static StringContent GenerateBody(string userPrincipalName)
	{
		string body = JsonSerializer.Serialize(userPrincipalName, HelperApiClient.JsonSerializerOptions);
		return new StringContent(body, Encoding.UTF8, "application/json");
	}

	private void SetCacheToken(string token)
	{
		tokenApiStateProvider.Token = token;
	}

	private string RetrieveCachedToken()
	{
		return tokenApiStateProvider.Token;
	}
}
```

### 2.3 Create `ApiClient/LoginApiHandler.cs`

```csharp
namespace ApiClient;

public class LoginApiHandler(ILoginApiService loginApiService) : DelegatingHandler
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		string token = await loginApiService.AuthenticateAsync();
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		return await base.SendAsync(request, cancellationToken);
	}
}
```

---

## Step 3: Migrate Service Files

For each service file in `Web/Services` (except `ResxLocalizer.cs`):

### 3.1 Service Migration Pattern

**Before (Web/Services pattern):**
```csharp
namespace Web;

public class ExampleService(HttpClient httpClient)
{
	private const string urlApi = "example";

	public async Task<List<Example>> GetItems()
	{
		HttpResponseMessage response = await httpClient.GetAsync($"{urlApi}/GetItems");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		Stream result = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<List<Example>>(result, Helper.JsonSerializerOptions);
	}
}
```

**After (ApiClient pattern):**
```csharp
namespace ApiClient;

public class ExampleService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "example";

	public async Task<List<Example>> GetItems()
	{
		HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/GetItems");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		Stream result = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<List<Example>>(result, HelperApiClient.JsonSerializerOptions);
	}
}
```

### 3.2 Key Changes for Each Service

| Item | Before | After |
|------|--------|-------|
| Namespace | `namespace Web;` | `namespace ApiClient;` |
| Constructor | `(HttpClient httpClient)` | `(IHttpClientFactory httpClientFactory)` |
| Field | `private const string urlApi` | `private readonly string apiName` |
| HttpClient | Direct use of `httpClient` | `httpClientFactory.CreateClient(Utilities.HttpClientName)` |
| JSON Options | `Helper.JsonSerializerOptions` | `HelperApiClient.JsonSerializerOptions` |

### 3.3 UserService Special Consideration

If `CustomUserFactory` calls a method like `SelectUser_UserPrincipalName`, ensure this method exists in `UserService`:

```csharp
public async Task<User> SelectUser_UserPrincipalName(string userPrincipalName)
{
	HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
	HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectUser_UserPrincipalName?userPrincipalName={userPrincipalName}");
	if (response.StatusCode == HttpStatusCode.NoContent)
		return null;
	if (!response.IsSuccessStatusCode)
		throw new ApplicationException(await response.Content.ReadAsStringAsync());
	Stream result = await response.Content.ReadAsStreamAsync();
	return await JsonSerializer.DeserializeAsync<User>(result, HelperApiClient.JsonSerializerOptions);
}
```

---

## Step 4: Update Solution File

Add the ApiClient project to the `.sln` file:

```
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "ApiClient", "ApiClient\ApiClient.csproj", "{GENERATE-NEW-GUID}"
EndProject
```

Add build configurations in `GlobalSection(ProjectConfigurationPlatforms)`:

```
{SAME-GUID}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
{SAME-GUID}.Debug|Any CPU.Build.0 = Debug|Any CPU
{SAME-GUID}.Release|Any CPU.ActiveCfg = Release|Any CPU
{SAME-GUID}.Release|Any CPU.Build.0 = Release|Any CPU
```

---

## Step 5: Update Web Project

### 5.1 Update `Web/Web.csproj`

Add reference to ApiClient project:

```xml
<ItemGroup>
	<ProjectReference Include="..\ApiClient\ApiClient.csproj" />
	<ProjectReference Include="..\Models\Models.csproj" />
</ItemGroup>
```

### 5.2 Update `Web/GlobalUsings.cs`

Add ApiClient namespace at the top:

```csharp
global using ApiClient;
```

If moving ResxLocalizer to Resources, also add:

```csharp
global using Web.Resources;
```

### 5.3 Update `Web/Program.cs`

Replace the old service registration pattern:

**Before:**
```csharp
WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
string urlApi = Utilities.GetApiUrl_BlazorWebProject(builder.HostEnvironment.BaseAddress);
builder.RootComponents.Add<App>("#app");
builder.Services.AddTelerikBlazor();
builder.Services.AddSingleton<ITelerikStringLocalizer, ResxLocalizer>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient<ReportService>(client => client.BaseAddress = new Uri(urlApi));
builder.Services.AddHttpClient<UserService>("User", client => client.BaseAddress = new Uri(urlApi));
// ... other typed HttpClient registrations

builder.Services.AddSingleton<StateProvider>();
```

**After:**
```csharp
WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
string apiUrl = Utilities.GetApiUrl_BlazorWebProject(builder.HostEnvironment.BaseAddress);

builder.RootComponents.Add<App>("#app");
builder.Services.AddTelerikBlazor();
builder.Services.AddSingleton<ITelerikStringLocalizer, ResxLocalizer>();

// Authentication infrastructure
builder.Services.AddSingleton<TokenApiStateProvider>();
builder.Services.AddScoped<ILoginApiService, LoginApiService>();
builder.Services.AddScoped<LoginApiHandler>();
builder.Services.AddHttpClient(Utilities.HttpClientNameAnonymous, client => client.BaseAddress = new Uri(apiUrl));
builder.Services.AddHttpClient(Utilities.HttpClientName, client => client.BaseAddress = new Uri(apiUrl)).AddHttpMessageHandler<LoginApiHandler>();

// State and services
builder.Services.AddSingleton<StateProvider>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<UserService>();
// ... register all other services as Scoped
```

### 5.4 Update `Web/Shared/CustomUserFactory.cs`

**Before:**
```csharp
namespace Web.Shared;

public class CustomUserFactory(IAccessTokenProviderAccessor accessor, IHttpClientFactory clientFactory, StateProvider stateProvider) : AccountClaimsPrincipalFactory<CustomUserAccount>(accessor)
{
	public async override ValueTask<ClaimsPrincipal> CreateUserAsync(CustomUserAccount account, RemoteAuthenticationUserOptions options)
	{
		// ... authentication logic
		User user = await SelectUser_UserPrincipalName(userPrincipalName);
		// ...
	}

	public async Task<User> SelectUser_UserPrincipalName(string userPrincipalName)
	{
		HttpClient client = clientFactory.CreateClient("User");
		// ... HTTP call logic
	}
}
```

**After:**
```csharp
namespace Web.Shared;

public class CustomUserFactory(IAccessTokenProviderAccessor accessor, UserService userService, StateProvider stateProvider, TokenApiStateProvider tokenApiStateProvider) : AccountClaimsPrincipalFactory<CustomUserAccount>(accessor)
{
	public async override ValueTask<ClaimsPrincipal> CreateUserAsync(CustomUserAccount account, RemoteAuthenticationUserOptions options)
	{
		ClaimsPrincipal initialUser = await base.CreateUserAsync(account, options);
		if (!initialUser.Identity.IsAuthenticated)
			return initialUser;

		ClaimsIdentity userIdentity = (ClaimsIdentity)initialUser.Identity;
		string userPrincipalName = initialUser.FindFirst("preferred_username").Value;
		tokenApiStateProvider.UserPrincipalName = userPrincipalName;
		User user = await SelectUser_UserPrincipalName(userPrincipalName);
		if (user != null)
		{
			// Set state provider properties
			stateProvider.UserId = user.Id;
			stateProvider.UserPrincipalName = userPrincipalName;
			// ... other properties
			if (!string.IsNullOrEmpty(user.RoleKey))
				userIdentity.AddClaim(new Claim("RoleKey", user.RoleKey));
		}
		return initialUser;
	}

	public async Task<User> SelectUser_UserPrincipalName(string userPrincipalName)
	{
		return await userService.SelectUser_UserPrincipalName(userPrincipalName);
	}
}
```

---

## Step 6: Move ResxLocalizer

### 6.1 Create `Web/Resources/ResxLocalizer.cs`

```csharp
namespace Web.Resources;

public class ResxLocalizer : ITelerikStringLocalizer
{
	public string this[string name]
	{
		get
		{
			return GetStringFromResource(name);
		}
	}

	public static string GetStringFromResource(string key)
	{
		return TelerikMessages.ResourceManager.GetString(key, TelerikMessages.Culture);
	}
}
```

### 6.2 Delete Old Services Folder

After all services are migrated and ResxLocalizer is moved, delete the `Web/Services` folder.

```powershell
Remove-Item -Path "Web\Services" -Recurse -Force
```

---

## Step 7: Verify Build

```powershell
dotnet build YourSolution.sln
```

Ensure all projects compile successfully.

---

## Checklist

### Api Project (JWT Authentication)
- [ ] Updated `Models/MRCAppSettings.cs` with `JwtKey` property
- [ ] Updated `Api/appsettings.json` with `JwtKey` setting
- [ ] Updated `Api/GlobalUsings.cs` with JWT-related usings
- [ ] Updated `Api/Program.cs` with JWT Bearer authentication
- [ ] Added `app.UseAuthentication()` before `app.UseAuthorization()`
- [ ] Created `Api/Controllers/AuthController.cs`

### ApiClient Project
- [ ] Created `ApiClient` folder at solution root
- [ ] Created `ApiClient/ApiClient.csproj`
- [ ] Created `ApiClient/GlobalUsings.cs`
- [ ] Created `ApiClient/HelperApiClient.cs`
- [ ] Created `ApiClient/TokenApiStateProvider.cs`
- [ ] Created `ApiClient/LoginApiService.cs`
- [ ] Created `ApiClient/LoginApiHandler.cs`
- [ ] Migrated all service files (except ResxLocalizer) to ApiClient
- [ ] Updated service files to use `IHttpClientFactory` pattern
- [ ] Added `SelectUser_UserPrincipalName` method to UserService (if needed)
- [ ] Added ApiClient project to solution file

### Web Project
- [ ] Updated `Web/Web.csproj` with ApiClient reference
- [ ] Updated `Web/GlobalUsings.cs` with ApiClient namespace
- [ ] Updated `Web/Program.cs` with new security pattern
- [ ] Updated `Web/Shared/CustomUserFactory.cs`
- [ ] Moved `ResxLocalizer.cs` to `Web/Resources`
- [ ] Deleted old `Web/Services` folder

### Final Verification
- [ ] Verified successful build

---

## Files Created/Modified Summary

### Modified Files (Api project)
- `Models/MRCAppSettings.cs` - Added `JwtKey` property
- `Api/appsettings.json` - Added `JwtKey` setting
- `Api/GlobalUsings.cs` - Added JWT-related usings
- `Api/Program.cs` - Added JWT Bearer authentication configuration

### New Files (Api project)
- `Api/Controllers/AuthController.cs` - JWT token generation endpoint

### New Files (ApiClient project)
- `ApiClient/ApiClient.csproj`
- `ApiClient/GlobalUsings.cs`
- `ApiClient/HelperApiClient.cs`
- `ApiClient/TokenApiStateProvider.cs`
- `ApiClient/LoginApiService.cs`
- `ApiClient/LoginApiHandler.cs`
- `ApiClient/*Service.cs` (migrated services)

### Modified Files (Web project)
- `Web/Web.csproj` - Added ApiClient reference
- `Web/GlobalUsings.cs` - Added ApiClient namespace
- `Web/Program.cs` - New security pattern and service registration
- `Web/Shared/CustomUserFactory.cs` - Use UserService and TokenApiStateProvider
- `Web/Resources/ResxLocalizer.cs` - Moved from Services

### Deleted
- `Web/Services/` folder and all contents
