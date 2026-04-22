---
applyTo: ApiClient/**
---

# ApiClient Project Rules and Standards

## Overview
The **ApiClient** project is the HTTP client service layer for the MRC ATMex Administration system. All service classes follow strict naming conventions, code style standards, and architectural patterns to ensure consistency and maintainability across the solution. This project acts as the communication bridge between the Blazor Web application and the Api project.

---

## Table of Contents
1. [File Structure and Namespaces](#file-structure-and-namespaces)
2. [Class Naming and Organization](#class-naming-and-organization)
3. [Primary Constructors and Dependency Injection](#primary-constructors-and-dependency-injection)
4. [Code Style Guidelines](#code-style-guidelines)
5. [HTTP Client Patterns](#http-client-patterns)
6. [Method Naming Conventions](#method-naming-conventions)
7. [Error Handling](#error-handling)
8. [Using Statements](#using-statements)
9. [Serialization](#serialization)
10. [Best Practices](#best-practices)
11. [Reference Examples](#reference-examples)

---

## File Structure and Namespaces

### File-Scoped Namespaces (Required)
All ApiClient project files must use **file-scoped namespaces** ending with a semicolon.

**✅ CORRECT:**
```csharp
namespace ApiClient;

public class SocietyService(IHttpClientFactory httpClientFactory)
{
	// Implementation
}
```

**❌ WRONG - Block-scoped namespace:**
```csharp
namespace ApiClient
{
	public class SocietyService(IHttpClientFactory httpClientFactory)
	{
		// Implementation
	}
}
```

### Namespace Convention
- All classes in the ApiClient project use the **single file-scoped namespace**: `namespace ApiClient;`
- Do NOT use sub-namespaces (e.g., `namespace ApiClient.Services;` is incorrect)

---

## Class Naming and Organization

### Naming Convention: *Entity* + "Service"
Service classes **must** follow the naming pattern: `[EntityName]Service`

**Convention:**
- `SocietyService` - Service for Society entity
- `AssetService` - Service for Asset entity
- `UserService` - Service for User entity
- `LoginApiService` - Service for authentication (special case)
- `TokenApiStateProvider` - State provider for tokens (special case)
- `LoginApiHandler` - HTTP handler for authentication (special case)

**✅ CORRECT:**
```csharp
namespace ApiClient;

public class SocietyService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "Society";

	public async Task<Society> SelectSociety(int id) { ... }
}
```

**❌ WRONG - Incorrect naming patterns:**
```csharp
public class SocietyClient { ... }      // Wrong suffix
public class SocietyDL { ... }          // Wrong suffix (that's for Data layer)
public class Society_Service { ... }    // Wrong pattern
```

### File Organization
- **One public class per file** - Each service class should be in its own file
- **File name matches class name** - `SocietyService.cs` contains `SocietyService` class
- **Private helper classes only** - Additional classes in a file must be private
- **One concern per class** - Each service class handles one entity type

---

## Primary Constructors and Dependency Injection

### Required: Primary Constructor Pattern
All service classes **must** use primary constructors for dependency injection.

**✅ CORRECT - Primary Constructor:**
```csharp
namespace ApiClient;

public class SocietyService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "Society";

	public async Task<Society> SelectSociety(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
		// Implementation
	}
}
```

**❌ WRONG - Traditional Constructor:**
```csharp
public class SocietyService
{
	private readonly IHttpClientFactory _httpClientFactory;

	public SocietyService(IHttpClientFactory httpClientFactory)
	{
		_httpClientFactory = httpClientFactory;
	}
}
```

### Dependency Injection Parameters
- **Required:** `IHttpClientFactory httpClientFactory` - Always the first (and typically only) parameter
- **Naming:** Use camelCase for the parameter name
- **Access:** The injected parameter is automatically available throughout the class

**✅ CORRECT:**
```csharp
public class AssetService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "Asset";

	public async Task<Asset> SelectAsset(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
		// Implementation
	}
}
```

---

## Code Style Guidelines

### Indentation
- **Use tabs** for indentation, NOT spaces
- Configure your editor to use tabs for the ApiClient project

**✅ CORRECT:**
```csharp
public class SocietyService(IHttpClientFactory httpClientFactory)
{
→	private readonly string apiName = "Society";

→	public async Task<Society> SelectSociety(int id)
→	{
→	→	HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
→	→	HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectSociety?id={id}");
→	→	// ...
→	}
}
```

### Type Declarations
- **Never use `var`** - Always use explicit type names

**✅ CORRECT:**
```csharp
HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectSociety?id={id}");
Stream result = await response.Content.ReadAsStreamAsync();
Society society = await JsonSerializer.DeserializeAsync<Society>(result, Helper.JsonSerializerOptions);
```

**❌ WRONG - Using `var`:**
```csharp
var httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
var response = await httpClient.GetAsync($"{apiName}/SelectSociety?id={id}");
var result = await response.Content.ReadAsStreamAsync();
```

### Object Instantiation
- Use **target-typed new** expression: `new()` when the type is obvious from context

**✅ CORRECT:**
```csharp
ReportCommercialParams reportCommercialParams = new() { GovernmentKeys = governmentKeys };
MultipartFormDataContent multipartFormDataContent = [];
```

**❌ WRONG - Redundant type specification:**
```csharp
ReportCommercialParams reportCommercialParams = new ReportCommercialParams() { GovernmentKeys = governmentKeys };
```

### One-Line If Statements
- **No braces** for single-line if statements

**✅ CORRECT:**
```csharp
if (!response.IsSuccessStatusCode)
	throw new ApplicationException(await response.Content.ReadAsStringAsync());

if (response.StatusCode == HttpStatusCode.NoContent)
	return null;
```

**❌ WRONG - Unnecessary braces:**
```csharp
if (!response.IsSuccessStatusCode)
{
	throw new ApplicationException(await response.Content.ReadAsStringAsync());
}
```

### Naming Conventions - General
- **No abbreviations** except standard acronyms (API, HTTP, JWT)
- Use PascalCase for class names, method names, properties
- Use camelCase for local variables and parameters

**✅ CORRECT:**
```csharp
public async Task<Society> SelectSociety(int id) { ... }
public async Task<List<Society>> SelectSocieties() { ... }
HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
```

**❌ WRONG - Abbreviations:**
```csharp
public async Task<Society> SelSoc(int i) { ... }
HttpClient hc = httpClientFactory.CreateClient(Utilities.HttpClientName);
```

### Null Checking
- Use `is null` or `is not null` for null checks

**✅ CORRECT:**
```csharp
if (userId is null || userId == "0")
	return true;
```

**❌ WRONG:**
```csharp
if (userId == null || userId == "0")
	return true;
```

### API Name Field
- Every service class must have a private readonly `apiName` field
- The value matches the controller name in the Api project

**✅ CORRECT:**
```csharp
public class SocietyService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "Society";
	// ...
}
```

---

## HTTP Client Patterns

### Creating HttpClient
Always use `IHttpClientFactory` to create `HttpClient` instances with the correct named client.

**✅ CORRECT:**
```csharp
HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
```

**❌ WRONG - Creating HttpClient directly:**
```csharp
HttpClient httpClient = new();
```

### GET Requests
Use for Select operations. Parameters are passed via querystring.

**✅ CORRECT:**
```csharp
public async Task<Society> SelectSociety(int id)
{
	HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
	HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectSociety?id={id}");
	if (!response.IsSuccessStatusCode)
		throw new ApplicationException(await response.Content.ReadAsStringAsync());
	if (response.StatusCode == HttpStatusCode.NoContent)
		return null;
	Stream result = await response.Content.ReadAsStreamAsync();
	return await JsonSerializer.DeserializeAsync<Society>(result, Helper.JsonSerializerOptions);
}
```

### POST Requests
Use for Insert operations. Entity is serialized in the request body with `PostAsJsonAsync`.

**✅ CORRECT:**
```csharp
public async Task<Society> InsertSociety(Society item)
{
	HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
	HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{apiName}/InsertSociety", item);
	if (!response.IsSuccessStatusCode)
		throw new ApplicationException(await response.Content.ReadAsStringAsync());
	if (response.StatusCode == HttpStatusCode.NoContent)
		return null;
	Stream result = await response.Content.ReadAsStreamAsync();
	return await JsonSerializer.DeserializeAsync<Society>(result, Helper.JsonSerializerOptions);
}
```

### PUT Requests
Use for Update operations. Entity is serialized in the request body with `PutAsJsonAsync`.

**✅ CORRECT:**
```csharp
public async Task<Society> UpdateSociety(Society item)
{
	HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
	HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{apiName}/UpdateSociety", item);
	if (!response.IsSuccessStatusCode)
		throw new ApplicationException(await response.Content.ReadAsStringAsync());
	if (response.StatusCode == HttpStatusCode.NoContent)
		return null;
	Stream result = await response.Content.ReadAsStreamAsync();
	return await JsonSerializer.DeserializeAsync<Society>(result, Helper.JsonSerializerOptions);
}
```

### DELETE Requests
Use for Delete operations. ID is passed via querystring.

**✅ CORRECT:**
```csharp
public async Task<int> DeleteSociety(int id)
{
	HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
	HttpResponseMessage response = await httpClient.DeleteAsync($"{apiName}/DeleteSociety?id={id}");
	if (!response.IsSuccessStatusCode)
		throw new ApplicationException(await response.Content.ReadAsStringAsync());
	if (response.StatusCode == HttpStatusCode.NoContent)
		return 0;
	Stream result = await response.Content.ReadAsStreamAsync();
	return await JsonSerializer.DeserializeAsync<int>(result, Helper.JsonSerializerOptions);
}
```

### File Upload (Multipart)
Use `MultipartFormDataContent` for file uploads.

**✅ CORRECT:**
```csharp
public async Task<List<ErrorMessage>> ImportAssets(AssetDocument assetDocument)
{
	HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
	httpClient.Timeout = TimeSpan.FromMinutes(30);
	MultipartFormDataContent multipartFormDataContent = [];
	multipartFormDataContent.Add(new StringContent(assetDocument.FileName), nameof(AssetDocument.FileName));
	multipartFormDataContent.Add(new StringContent(assetDocument.FileExtension), nameof(AssetDocument.FileExtension));
	multipartFormDataContent.Add(new StringContent(assetDocument.FileSize.ToString()), nameof(AssetDocument.FileSize));
	multipartFormDataContent.Add(new ByteArrayContent(assetDocument.FileBytes), nameof(AssetDocument.FileBytes), assetDocument.FileName);

	HttpResponseMessage response = await httpClient.PostAsync($"{apiName}/ImportAssets", multipartFormDataContent);
	if (!response.IsSuccessStatusCode)
		throw new ApplicationException(await response.Content.ReadAsStringAsync());
	Stream result = await response.Content.ReadAsStreamAsync();
	return await JsonSerializer.DeserializeAsync<List<ErrorMessage>>(result, Helper.JsonSerializerOptions);
}
```

### Async All the Way
- **Always use async/await** for all HTTP operations
- Never use `.Result` or `.Wait()` - these can cause deadlocks

**✅ CORRECT:**
```csharp
public async Task<List<Society>> SelectSocieties()
{
	HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
	HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectSocieties");
	// ...
}
```

**❌ WRONG - Synchronous operations:**
```csharp
public List<Society> SelectSocieties()
{
	HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
	HttpResponseMessage response = httpClient.GetAsync($"{apiName}/SelectSocieties").Result;  // Blocking call
	// ...
}
```

---

## Method Naming Conventions

### Standard CRUD Method Names
Service methods follow a strict naming convention matching the Api controller methods:

| Operation | HTTP Verb | Method Name | Example |
|-----------|-----------|-------------|---------|
| **SELECT** (single) | GET | `Select[Entity]` | `SelectSociety(int id)` |
| **SELECT** (multiple) | GET | `Select[Entities]` | `SelectSocieties()` |
| **INSERT** | POST | `Insert[Entity]` | `InsertSociety(Society item)` |
| **UPDATE** | PUT | `Update[Entity]` | `UpdateSociety(Society item)` |
| **DELETE** | DELETE | `Delete[Entity]` | `DeleteSociety(int id)` |

### Pluralization Rule
- Use **singular entity name** for single record operations
- Use **plural entity name** for collection operations

**✅ CORRECT:**
```csharp
public async Task<Society> SelectSociety(int id) { ... }           // Single record
public async Task<List<Society>> SelectSocieties() { ... }          // Multiple records
public async Task<List<Society>> SelectSocieties_Type(string societyTypeKey) { ... }
public async Task<Society> InsertSociety(Society item) { ... }      // Insert single
public async Task<Society> UpdateSociety(Society item) { ... }      // Update single
public async Task<int> DeleteSociety(int id) { ... }                // Delete single
```

**❌ WRONG - Using Get/Create/Add:**
```csharp
public async Task<Society> GetSociety(int id) { ... }              // Use "Select" not "Get"
public async Task<Society> CreateSociety(Society item) { ... }     // Use "Insert" not "Create"
public async Task<Society> AddSociety(Society item) { ... }        // Use "Insert" not "Add"
```

### Method Parameters
- Use descriptive parameter names: `int id`, `int societyId`, `string societyTypeKey`
- Never use single-letter names

**✅ CORRECT:**
```csharp
public async Task<Society> SelectSociety(int id) { ... }
public async Task<List<Society>> SelectSocieties_Type(string societyTypeKey) { ... }
public async Task<List<Society>> SelectSocieties_Type_Group(string societyTypeKey, int groupId) { ... }
```

---

## Error Handling

### Check Response Status
Always check `IsSuccessStatusCode` and throw `ApplicationException` on failure.

**✅ CORRECT:**
```csharp
HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectSociety?id={id}");
if (!response.IsSuccessStatusCode)
	throw new ApplicationException(await response.Content.ReadAsStringAsync());
```

### Handle NoContent Response
Check for `HttpStatusCode.NoContent` and return appropriate default values.

**✅ CORRECT:**
```csharp
if (response.StatusCode == HttpStatusCode.NoContent)
	return null;  // For reference types

if (response.StatusCode == HttpStatusCode.NoContent)
	return 0;     // For int

if (response.StatusCode == HttpStatusCode.NoContent)
	return false; // For bool
```

### Standard Response Handling Pattern
Every method should follow this pattern:

```csharp
public async Task<Society> SelectSociety(int id)
{
	HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
	HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectSociety?id={id}");
	
	// 1. Check for errors
	if (!response.IsSuccessStatusCode)
		throw new ApplicationException(await response.Content.ReadAsStringAsync());
	
	// 2. Handle empty response
	if (response.StatusCode == HttpStatusCode.NoContent)
		return null;
	
	// 3. Deserialize and return
	Stream result = await response.Content.ReadAsStreamAsync();
	return await JsonSerializer.DeserializeAsync<Society>(result, Helper.JsonSerializerOptions);
}
```

---

## Using Statements

### GlobalUsings.cs Convention (Critical)
**Never use local `using` statements in ApiClient project files.** All using statements must be in `GlobalUsings.cs`.

`System.Net.Http.Json` must be included so `PostAsJsonAsync` and `PutAsJsonAsync` are available in service classes.

**✅ CORRECT - ApiClient/GlobalUsings.cs:**
```csharp
global using Models;
global using MRC.DTI.Dev.Utilities.Business;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Text;
global using System.Text.Json;
global using System.Net.Http.Json;
global using System.IdentityModel.Tokens.Jwt;
```

**ApiClient/SocietyService.cs - No using statements:**
```csharp
namespace ApiClient;

public class SocietyService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "Society";
	// Implementation
}
```

**❌ WRONG - Using statements in individual files:**
```csharp
using System.Text.Json;
using Models;

namespace ApiClient;

public class SocietyService(IHttpClientFactory httpClientFactory) { ... }
```

---

## Serialization

### Use Helper.JsonSerializerOptions
Always use the shared `JsonSerializerOptions` from `Helper` class for consistency.

**✅ CORRECT:**
```csharp
Stream result = await response.Content.ReadAsStreamAsync();
return await JsonSerializer.DeserializeAsync<Society>(result, Helper.JsonSerializerOptions);
```

**❌ WRONG - Creating new options:**
```csharp
JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
return await JsonSerializer.DeserializeAsync<Society>(result, options);
```

### Serialize Request Body
Use `PostAsJsonAsync` and `PutAsJsonAsync` instead of manually creating `StringContent` for standard insert and update requests.

**✅ CORRECT:**
```csharp
HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{apiName}/InsertSociety", item);
HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{apiName}/UpdateSociety", item);
```

---

## Best Practices

### 1. Keep Service Classes Focused
- Each service class handles **one entity type** only
- Exception: Special services like `LoginApiService`, `ReportService`

### 2. Match Api Controller Methods
- Service methods should mirror the Api controller methods exactly
- Same method names, same parameters

### 3. Use Appropriate Timeouts
- For long-running operations (file imports), set custom timeouts

**✅ CORRECT:**
```csharp
HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
httpClient.Timeout = TimeSpan.FromMinutes(30);  // For file imports
```

### 4. Use Named HTTP Clients
- Use `Utilities.HttpClientName` for authenticated requests
- Use `Utilities.HttpClientNameAnonymous` for authentication endpoints

---

## Reference Examples

### Complete SocietyService Example
```csharp
namespace ApiClient;

public class SocietyService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "Society";

	public async Task<Society> SelectSociety(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectSociety?id={id}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		if (response.StatusCode == HttpStatusCode.NoContent)
			return null;
		Stream result = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Society>(result, Helper.JsonSerializerOptions);
	}

	public async Task<List<Society>> SelectSocieties()
	{
		HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectSocieties");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		Stream result = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<List<Society>>(result, Helper.JsonSerializerOptions);
	}

	public async Task<Society> InsertSociety(Society item)
	{
		HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
		HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{apiName}/InsertSociety", item);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		if (response.StatusCode == HttpStatusCode.NoContent)
			return null;
		Stream result = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Society>(result, Helper.JsonSerializerOptions);
	}

	public async Task<Society> UpdateSociety(Society item)
	{
		HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
		HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{apiName}/UpdateSociety", item);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		if (response.StatusCode == HttpStatusCode.NoContent)
			return null;
		Stream result = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<Society>(result, Helper.JsonSerializerOptions);
	}

	public async Task<int> DeleteSociety(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
		HttpResponseMessage response = await httpClient.DeleteAsync($"{apiName}/DeleteSociety?id={id}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		if (response.StatusCode == HttpStatusCode.NoContent)
			return 0;
		Stream result = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<int>(result, Helper.JsonSerializerOptions);
	}
}
```

### Complete GlobalUsings.cs Example
```csharp
global using Models;
global using MRC.DTI.Dev.Utilities.Business;
global using System.Net;
global using System.Net.Http.Headers;
global using System.Text;
global using System.Text.Json;
global using System.Net.Http.Json;
global using System.IdentityModel.Tokens.Jwt;
```

---

## Verification Checklist

Before committing ApiClient layer code, verify:

- [ ] File uses file-scoped namespace (`namespace ApiClient;`)
- [ ] Class name follows pattern: `[Entity]Service`
- [ ] Primary constructor used for DI: `public class SocietyService(IHttpClientFactory httpClientFactory)`
- [ ] Has `private readonly string apiName` field matching controller name
- [ ] All methods are `async` and return `Task<T>` or `Task`
- [ ] Uses `httpClientFactory.CreateClient(Utilities.HttpClientName)`
- [ ] Checks `response.IsSuccessStatusCode` and throws `ApplicationException` on failure
- [ ] Handles `HttpStatusCode.NoContent` appropriately
- [ ] Uses `Helper.JsonSerializerOptions` for deserialization
- [ ] Method names follow convention: `Select`, `Insert`, `Update`, `Delete`
- [ ] Collections use plural: `SelectSocieties()`, not `SelectSociety()` for lists
- [ ] No `var` used - all types are explicit
- [ ] Indentation uses tabs, not spaces
- [ ] No local `using` statements - all in `GlobalUsings.cs`
- [ ] No abbreviations in names (except standard acronyms)

---

## Related Documentation
- [AI Coding Guide](../copilot-instructions.md)
- [Api Project Rules and Standards](./api.instructions.md)
- [Data Project Rules and Standards](./data.instructions.md)
- [Models Project Rules and Standards](./models.instructions.md)
