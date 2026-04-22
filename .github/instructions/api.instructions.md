---
applyTo: Api/**
---

# Api Project Rules and Standards

## Overview
The **Api** project is the ASP.NET Core Web API layer. All controllers follow strict naming conventions, code style standards, and architectural patterns to ensure consistency and maintainability across the solution.

---

## Table of Contents
1. [File Structure and Namespaces](#file-structure-and-namespaces)
2. [Class Naming and Organization](#class-naming-and-organization)
3. [Primary Constructors and Dependency Injection](#primary-constructors-and-dependency-injection)
4. [Code Style Guidelines](#code-style-guidelines)
5. [Controller Patterns](#controller-patterns)
6. [Method Naming Conventions](#method-naming-conventions)
7. [Error Handling](#error-handling)
8. [Using Statements](#using-statements)
9. [Logging](#logging)
10. [Best Practices](#best-practices)
11. [Reference Examples](#reference-examples)

---

## File Structure and Namespaces

### File-Scoped Namespaces (Required)
All Api project files must use **file-scoped namespaces** ending with a semicolon.

**✅ CORRECT:**
```csharp
namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AssetController(ILogger<AssetController> logger, AssetDL assetDL) : ControllerBase
{
	// Implementation
}
```

**❌ WRONG - Block-scoped namespace:**
```csharp
namespace Api.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AssetController(ILogger<AssetController> logger, AssetDL assetDL) : ControllerBase
	{
		// Implementation
	}
}
```

### Namespace Convention
- Controllers use: `namespace Api.Controllers;`
- Middleware uses: `namespace Api.Middleware;`
- Root-level files use: `namespace Api;`

---

## Class Naming and Organization

### Naming Convention: *Entity* + "Controller"
Controller classes **must** follow the naming pattern: `[EntityName]Controller`

**Convention:**
- `AssetController` - Controller for Asset entity
- `SocietyController` - Controller for Society entity
- `UserController` - Controller for User entity
- `AuthController` - Controller for authentication (special case)

**✅ CORRECT:**
```csharp
namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AssetController(ILogger<AssetController> logger, AssetDL assetDL) : ControllerBase
{
	[HttpGet("SelectAsset")]
	public async Task<ActionResult> SelectAsset(int id) { ... }
}
```

**❌ WRONG - Incorrect naming patterns:**
```csharp
public class AssetApi { ... }           // Wrong suffix
public class AssetsController { ... }   // Wrong plural
public class Asset_Controller { ... }   // Wrong pattern
```

### File Organization
- **One controller per file** - Each controller should be in its own file
- **File name matches class name** - `AssetController.cs` contains `AssetController` class
- **Controllers folder** - All controllers must be in the `Controllers/` folder

---

## Primary Constructors and Dependency Injection

### Required: Primary Constructor Pattern
All controller classes **must** use primary constructors for dependency injection.

**✅ CORRECT - Primary Constructor:**
```csharp
namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class SocietyController(ILogger<SocietyController> logger, SocietyDL societyDL) : ControllerBase
{
	[HttpGet("SelectSociety")]
	public async Task<ActionResult> SelectSociety(int id)
	{
		logger.LogInformation("SelectSociety(id: {@Id})", id);
		Society item = await societyDL.SelectSociety(id);
		return Ok(item);
	}
}
```

**❌ WRONG - Traditional Constructor:**
```csharp
public class SocietyController : ControllerBase
{
	private readonly ILogger<SocietyController> _logger;
	private readonly SocietyDL _societyDL;

	public SocietyController(ILogger<SocietyController> logger, SocietyDL societyDL)
	{
		_logger = logger;
		_societyDL = societyDL;
	}
}
```

### Dependency Injection Parameters
- **Use parameters directly** - Do NOT create local variables for injected objects
- **Exception:** Extract `IOptions<T>.Value` into a readonly field when needed
- **Naming:** Use camelCase for parameter names matching the type (e.g., `assetDL` for `AssetDL`)

**✅ CORRECT:**
```csharp
public class AssetController(ILogger<AssetController> logger, IOptions<MRCAppSettings> mrcAppSettings, AssetDL assetDL) : ControllerBase
{
	private readonly MRCAppSettings _mrcAppSettings = mrcAppSettings.Value;  // Exception: extract Value

	[HttpGet("SelectAsset")]
	public async Task<ActionResult> SelectAsset(int id)
	{
		logger.LogInformation("SelectAsset(id: {Id})", id);  // Use parameter directly
		Asset item = await assetDL.SelectAsset(id);          // Use parameter directly
		return Ok(item);
	}
}
```

**❌ WRONG - Creating local variables:**
```csharp
public class AssetController(ILogger<AssetController> logger, AssetDL assetDL) : ControllerBase
{
	private readonly ILogger<AssetController> _logger = logger;  // Wrong!
	private readonly AssetDL _assetDL = assetDL;                 // Wrong!
}
```

---

## Code Style Guidelines

### Indentation
- **Use tabs** for indentation, NOT spaces
- Configure your editor to use tabs for the Api project

**✅ CORRECT:**
```csharp
public class AssetController(ILogger<AssetController> logger, AssetDL assetDL) : ControllerBase
{
→	[HttpGet("SelectAsset")]
→	public async Task<ActionResult> SelectAsset(int id)
→	{
→	→	logger.LogInformation("SelectAsset(id: {Id})", id);
→	→	Asset item = await assetDL.SelectAsset(id);
→	→	return Ok(item);
→	}
}
```

### Type Declarations
- **Never use `var`** - Always use explicit type names

**✅ CORRECT:**
```csharp
Asset item = await assetDL.SelectAsset(id);
List<Society> items = await societyDL.SelectSocieties();
int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));
```

**❌ WRONG - Using `var`:**
```csharp
var item = await assetDL.SelectAsset(id);
var items = await societyDL.SelectSocieties();
```

### One-Line If Statements
- **No braces** for single-line if statements

**✅ CORRECT:**
```csharp
if (item is null)
	return Ok(null);

if (item.Notary.Name == Helper.NOTARY_NEWNAME)
	item.Notary.Name = string.Empty;
```

**❌ WRONG - Unnecessary braces:**
```csharp
if (item is null)
{
	return Ok(null);
}
```

### Naming Conventions - General
- **No abbreviations** except standard acronyms (DL, API, JWT)
- Use PascalCase for method names and properties
- Use camelCase for local variables and parameters
- Variable names match class names in camelCase: `societyDL` for `SocietyDL`, `asset` for `Asset`

**✅ CORRECT:**
```csharp
public async Task<ActionResult> SelectAsset(int id) { ... }
public async Task<ActionResult> InsertSociety([FromBody] Society item) { ... }
Society society = await societyDL.SelectSociety(id);
```

**❌ WRONG - Abbreviations:**
```csharp
public async Task<ActionResult> SelAsset(int i) { ... }
Society soc = await societyDL.SelectSociety(id);
```

### Null Checking
- Use `is null` or `is not null` for null checks

**✅ CORRECT:**
```csharp
if (item is null)
	return Unauthorized();
```

**❌ WRONG:**
```csharp
if (item == null)
	return Unauthorized();
```

---

## Controller Patterns

### Controller Attributes
All controllers must have the following attributes:

**✅ CORRECT:**
```csharp
[Route("[controller]")]
[ApiController]
public class AssetController(ILogger<AssetController> logger, AssetDL assetDL) : ControllerBase
```

**❌ WRONG - Using api/ prefix:**
```csharp
[Route("api/[controller]")]  // Wrong! Do not use api/ prefix
[ApiController]
public class AssetController : ControllerBase
```

### HTTP Method Decorators
HTTP decorators include **only the method name**. Parameters are passed via querystring, NOT in the route.

**✅ CORRECT:**
```csharp
[HttpGet("SelectAsset")]
public async Task<ActionResult> SelectAsset(int id) { ... }

[HttpPost("InsertAsset")]
public async Task<ActionResult> InsertAsset([FromBody] Asset item) { ... }

[HttpPut("UpdateAsset")]
public async Task<ActionResult> UpdateAsset([FromBody] Asset item) { ... }

[HttpDelete("DeleteAsset")]
public async Task<ActionResult> DeleteAsset(int id) { ... }
```

**❌ WRONG - Parameters in route:**
```csharp
[HttpGet("SelectAsset/{id}")]        // Wrong! No route parameters
public async Task<ActionResult> SelectAsset(int id) { ... }

[HttpDelete("DeleteAsset/{id}")]     // Wrong! No route parameters
public async Task<ActionResult> DeleteAsset(int id) { ... }
```

### Return Values
- **Return `Ok()`** with the result for successful operations
- **Returning null is valid** - Do NOT return `NotFound()`
- Use `Unauthorized()` only for authentication failures

**✅ CORRECT:**
```csharp
[HttpGet("SelectAsset")]
public async Task<ActionResult> SelectAsset(int id)
{
	Asset item = await assetDL.SelectAsset(id);
	return Ok(item);  // Even if item is null
}
```

**❌ WRONG - Returning NotFound:**
```csharp
[HttpGet("SelectAsset")]
public async Task<ActionResult> SelectAsset(int id)
{
	Asset item = await assetDL.SelectAsset(id);
	if (item is null)
		return NotFound();  // Wrong! Return Ok(null) instead
	return Ok(item);
}
```

### Async All the Way
- **Always use async/await** for controller actions
- Return `Task<ActionResult>` or `Task<IActionResult>`

**✅ CORRECT:**
```csharp
[HttpGet("SelectSocieties")]
public async Task<ActionResult> SelectSocieties()
{
	List<Society> items = await societyDL.SelectSocieties();
	return Ok(items);
}
```

---

## Method Naming Conventions

### Standard CRUD Method Names
Controller methods follow a strict naming convention using verb prefixes:

| Operation | HTTP Verb | Method Name | Example |
|-----------|-----------|-------------|---------|
| **SELECT** (single) | GET | `Select[Entity]` | `SelectAsset(int id)` |
| **SELECT** (multiple) | GET | `Select[Entities]` | `SelectAssets()` |
| **INSERT** | POST | `Insert[Entity]` | `InsertAsset([FromBody] Asset item)` |
| **UPDATE** | PUT | `Update[Entity]` | `UpdateAsset([FromBody] Asset item)` |
| **DELETE** | DELETE | `Delete[Entity]` | `DeleteAsset(int id)` |

**✅ CORRECT:**
```csharp
[HttpGet("SelectSociety")]
public async Task<ActionResult> SelectSociety(int id) { ... }

[HttpGet("SelectSocieties")]
public async Task<ActionResult> SelectSocieties() { ... }

[HttpPost("InsertSociety")]
public async Task<ActionResult> InsertSociety([FromBody] Society item) { ... }

[HttpPut("UpdateSociety")]
public async Task<ActionResult> UpdateSociety([FromBody] Society item) { ... }

[HttpDelete("DeleteSociety")]
public async Task<ActionResult> DeleteSociety(int id) { ... }
```

**❌ WRONG - Using Get/Create/Add:**
```csharp
[HttpGet("GetSociety")]              // Wrong! Use "Select"
public async Task<ActionResult> GetSociety(int id) { ... }

[HttpPost("CreateSociety")]          // Wrong! Use "Insert"
public async Task<ActionResult> CreateSociety([FromBody] Society item) { ... }

[HttpPost("AddSociety")]             // Wrong! Use "Insert"
public async Task<ActionResult> AddSociety([FromBody] Society item) { ... }
```

---

## Error Handling

### ApplicationException for Business Errors
Use `ApplicationException` for business rule violations. The middleware returns HTTP 400.

**✅ CORRECT:**
```csharp
[HttpPost("InsertAsset")]
public async Task<ActionResult> InsertAsset([FromBody] Asset item)
{
	if (item is null)
		throw new ApplicationException("Asset cannot be null.");
	
	item = await assetDL.InsertAsset(item);
	return Ok(item);
}
```

### CustomExceptionHandlingMiddleware
- `ApplicationException` → HTTP 400 (Bad Request)
- Other exceptions → HTTP 500 (Internal Server Error) with error ID
- Error ID is logged for troubleshooting

### Let Exceptions Propagate
Do NOT catch exceptions in controllers unless you have specific recovery logic.

**❌ WRONG - Catching all exceptions:**
```csharp
[HttpGet("SelectAsset")]
public async Task<ActionResult> SelectAsset(int id)
{
	try
	{
		Asset item = await assetDL.SelectAsset(id);
		return Ok(item);
	}
	catch (Exception ex)
	{
		return BadRequest(ex.Message);  // Wrong! Let middleware handle it
	}
}
```

---

## Using Statements

### GlobalUsings.cs Convention (Critical)
**Never use local `using` statements in Api project files.** All using statements must be in `GlobalUsings.cs`.

**✅ CORRECT - Api/GlobalUsings.cs:**
```csharp
global using Api.Middleware;
global using Data;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Models;
global using Serilog;
global using System.IdentityModel.Tokens.Jwt;
global using System.Net;
global using System.Security.Claims;
global using System.Text;
global using System.Text.Json.Serialization;
```

**Api/Controllers/AssetController.cs - No using statements:**
```csharp
namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AssetController(ILogger<AssetController> logger, AssetDL assetDL) : ControllerBase
{
	// Implementation
}
```

**❌ WRONG - Using statements in individual files:**
```csharp
using Microsoft.AspNetCore.Mvc;
using Data;

namespace Api.Controllers;

public class AssetController : ControllerBase { ... }
```

---

## Logging

### Structured Logging Pattern
Use `ILogger<T>` with structured logging for all controller actions.

**✅ CORRECT:**
```csharp
[HttpGet("SelectAsset")]
public async Task<ActionResult> SelectAsset(int id)
{
	logger.LogInformation("SelectAsset(id: {Id})", id);
	Asset item = await assetDL.SelectAsset(id);
	return Ok(item);
}

[HttpPost("InsertAsset")]
public async Task<ActionResult> InsertAsset([FromBody] Asset item)
{
	logger.LogInformation("InsertAsset(item: {@Item})", item);
	item = await assetDL.InsertAsset(item);
	return Ok(item);
}
```

### Logging Format
- Use method name followed by parameters: `"MethodName(param1: {Param1}, param2: {Param2})"`
- Use `{@Object}` for complex objects to log all properties
- Use `{Value}` for simple values

---

## Best Practices

### 1. Keep Controllers Focused
- Each controller handles **one entity type**
- Inject only the DL classes needed for that entity

### 2. Use [FromBody] for Complex Types
- POST and PUT methods receive entities via `[FromBody]`
- GET and DELETE use querystring parameters

**✅ CORRECT:**
```csharp
[HttpPost("InsertSociety")]
public async Task<ActionResult> InsertSociety([FromBody] Society item) { ... }

[HttpGet("SelectSociety")]
public async Task<ActionResult> SelectSociety(int id) { ... }
```

### 3. Extract User Claims When Needed
```csharp
int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Sid));
string userPrincipalName = User.FindFirstValue(ClaimTypes.Upn);
```

### 4. Set Audit Fields in Controller
```csharp
[HttpPost("InsertAsset")]
public async Task<ActionResult> InsertAsset([FromBody] Asset item)
{
	item.Created = DateTime.UtcNow;
	item.CreatedBy = User.FindFirstValue(ClaimTypes.Upn);
	item = await assetDL.InsertAsset(item);
	return Ok(item);
}
```

---

## Reference Examples

### Complete SocietyController Example
```csharp
namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class SocietyController(ILogger<SocietyController> logger, SocietyDL societyDL) : ControllerBase
{
	[HttpGet("SelectSociety")]
	public async Task<ActionResult> SelectSociety(int id)
	{
		logger.LogInformation("SelectSociety(id: {@Id})", id);
		Society item = await societyDL.SelectSociety(id);
		return Ok(item);
	}

	[HttpGet("SelectSocieties")]
	public async Task<ActionResult> SelectSocieties()
	{
		logger.LogInformation("SelectSocieties()");
		List<Society> items = await societyDL.SelectSocieties();
		return Ok(items);
	}

	[HttpPost("InsertSociety")]
	public async Task<ActionResult> InsertSociety([FromBody] Society item)
	{
		logger.LogInformation("InsertSociety(item: {@Item})", item);
		item = await societyDL.InsertSociety(item);
		return Ok(item);
	}

	[HttpPut("UpdateSociety")]
	public async Task<ActionResult> UpdateSociety([FromBody] Society item)
	{
		logger.LogInformation("UpdateSociety(item: {@Item})", item);
		item = await societyDL.UpdateSociety(item);
		return Ok(item);
	}

	[HttpDelete("DeleteSociety")]
	public async Task<ActionResult> DeleteSociety(int id)
	{
		logger.LogInformation("DeleteSociety(id: {@Id})", id);
		int result = await societyDL.DeleteSociety(id);
		return Ok(result);
	}
}
```

### Complete GlobalUsings.cs Example
```csharp
global using Api.Middleware;
global using Data;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;
global using Microsoft.IdentityModel.Tokens;
global using Models;
global using MRC.DTI.Dev.Utilities.Business;
global using Serilog;
global using Serilog.Events;
global using System.IdentityModel.Tokens.Jwt;
global using System.Net;
global using System.Security.Claims;
global using System.Text;
global using System.Text.Json.Serialization;
global using System.Net.Mime;
```

---

## Verification Checklist

Before committing Api layer code, verify:

- [ ] File uses file-scoped namespace (`namespace Api.Controllers;`)
- [ ] Class name follows pattern: `[Entity]Controller`
- [ ] Primary constructor used for DI
- [ ] No local variables for injected objects (except `IOptions<T>.Value`)
- [ ] Controller has `[Route("[controller]")]` (not `api/[controller]`)
- [ ] Controller has `[ApiController]` attribute
- [ ] HTTP decorators have method names only, no route parameters
- [ ] Method names follow convention: `Select`, `Insert`, `Update`, `Delete`
- [ ] All actions are `async` and return `Task<ActionResult>`
- [ ] Return `Ok()` for all results (including null)
- [ ] No `var` used - all types are explicit
- [ ] Indentation uses tabs, not spaces
- [ ] No local `using` statements - all in `GlobalUsings.cs`
- [ ] Logging uses structured format with `ILogger<T>`
- [ ] No abbreviations in names (except standard acronyms)
- [ ] Null checking uses `is null` / `is not null`

---

## Related Documentation
- [AI Coding Guide](../copilot-instructions.md)
- [Data Project Rules and Standards](./data.instructions.md)
- [Models Project Rules and Standards](./models.instructions.md)
