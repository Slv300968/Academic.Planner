---
applyTo: Data/**
---

# Data Project Rules and Standards

## Overview
The **Data** project is the Entity Framework Core data access layer. All classes follow strict naming conventions, code style standards, and architectural patterns to ensure consistency and maintainability across the solution.

---

## Table of Contents
1. [File Structure and Namespaces](#file-structure-and-namespaces)
2. [Class Naming and Organization](#class-naming-and-organization)
3. [Primary Constructors and Dependency Injection](#primary-constructors-and-dependency-injection)
4. [Code Style Guidelines](#code-style-guidelines)
5. [EF Core Query Patterns](#ef-core-query-patterns)
6. [Method Naming Conventions](#method-naming-conventions)
7. [Error Handling](#error-handling)
8. [Using Statements](#using-statements)
9. [Entity Relationships](#entity-relationships)
10. [Best Practices](#best-practices)
11. [Reference Examples](#reference-examples)

---

## File Structure and Namespaces

### File-Scoped Namespaces (Required)
All Data project files must use **file-scoped namespaces** ending with a semicolon. This is the modern C# convention and improves readability.

**✅ CORRECT:**
```csharp
namespace Data;

public class RiskDL(MRCDBContext context)
{
	// Implementation
}
```

**❌ WRONG - Block-scoped namespace:**
```csharp
namespace Data
{
	public class RiskDL(MRCDBContext context)
	{
		// Implementation
	}
}
```

### Namespace Convention
- All classes in the Data project use the **single file-scoped namespace**: `namespace Data;`
- Do NOT use sub-namespaces for data layer classes (e.g., `namespace Data.Repositories;` is incorrect)
- Exception: `MRCDBContext` uses `namespace Data;`

---

## Class Naming and Organization

### Naming Convention: *Entity* + "DL"
Data layer classes **must** follow the naming pattern: `[EntityName]DL`

**Convention:**
- `RiskDL` - Data layer for Risk entity
- `ProjectDL` - Data layer for Project entity
- `CategoryDL` - Data layer for Category entity
- `ControlDL` - Data layer for Control entity
- `MRCDBContext` - The DbContext (special case, no "DL" suffix)

**✅ CORRECT:**
```csharp
namespace Data;

public class RiskDL(MRCDBContext context)
{
	public async Task<Risk> SelectRisk(int id) { ... }
}
```

**❌ WRONG - Incorrect naming patterns:**
```csharp
public class RiskRepository { ... }      // Wrong suffix
public class RiskService { ... }         // Wrong suffix (that's for ApiClient)
public class Risk_DataAccess { ... }     // Wrong pattern
```

### File Organization
- **One public class per file** - Each DL class should be in its own file
- **File name matches class name** - `RiskDL.cs` contains `RiskDL` class
- **Private helper classes only** - Additional classes in a file must be private
- **One concern per class** - Each DL class handles one entity type

---

## Primary Constructors and Dependency Injection

### Required: Primary Constructor Pattern
All DL classes **must** use primary constructors for dependency injection. This is a C# 12+ feature that provides a clean, concise way to inject the `MRCDBContext`.

**✅ CORRECT - Primary Constructor:**
```csharp
namespace Data;

public class RiskDL(MRCDBContext context)
{
	public async Task<Risk> SelectRisk(int id)
	{
		return await context.Risks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
	}
}
```

**❌ WRONG - Traditional Constructor:**
```csharp
public class RiskDL
{
	private readonly MRCDBContext context;

	public RiskDL(MRCDBContext context)
	{
		this.context = context;
	}
}
```

### Dependency Injection Parameters
- **Required:** `MRCDBContext context` - Always the first (and typically only) parameter
- **Type:** Use the exact type `MRCDBContext`, not an abstraction
- **Naming:** Use lowercase `context` (camelCase) for the parameter name
- **Access:** The injected parameter is automatically available as a field throughout the class

**✅ CORRECT:**
```csharp
public class ControlDL(MRCDBContext context)
{
	public async Task<Control> SelectControl(int id)
	{
		return await context.Controls.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
	}
}
```

---

## Code Style Guidelines

### Indentation
- **Use tabs** for indentation, NOT spaces
- Configure your editor to use tabs for the Data project
- Tabs provide better accessibility and flexibility for developers

**✅ CORRECT:**
```csharp
public class RiskDL(MRCDBContext context)
{
→	public async Task<Risk> SelectRisk(int id)  // Tab character
→	{
→	→	return await context.Risks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);  // Two tabs
→	}
}
```

### Type Declarations
- **Never use `var`** - Always use explicit type names
- Explicit types improve code readability and maintain clarity of intent
- Exception: None - always be explicit

**✅ CORRECT:**
```csharp
Risk risk = await context.Risks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
List<Risk> risks = await context.Risks.AsNoTracking().ToListAsync();
IEnumerable<Control> controls = context.Controls.AsNoTracking().ToList();
```

**❌ WRONG - Using `var`:**
```csharp
var risk = await context.Risks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
var risks = await context.Risks.AsNoTracking().ToListAsync();
```

### Object Instantiation
- Use **target-typed new** expression: `new()` when the type is obvious from context
- This is cleaner and aligns with modern C# practices

**✅ CORRECT:**
```csharp
Risk risk = new();
risk.Name = "High Risk";
```

**❌ WRONG - Redundant type specification:**
```csharp
Risk risk = new Risk();
```

### One-Line If Statements
- **No braces** for single-line if statements
- Use simple, readable conditions only
- For complex logic, use braces and multiple lines

**✅ CORRECT:**
```csharp
if (risk is null)
	return null;

if (risks.Count > 0)
	return risks.First();
```

**❌ WRONG - Unnecessary braces:**
```csharp
if (risk is null)
{
	return null;
}
```

### Naming Conventions - General
- **No abbreviations** in names except for standard acronyms (DL, API, JWT, CRUD)
- Use full, descriptive names that clearly indicate purpose
- Use PascalCase for class names, method names, properties
- Use camelCase for local variables and parameters

**✅ CORRECT:**
```csharp
public async Task<Project> SelectProject(int projectId) { ... }
public async Task<List<Risk>> SelectRisksByCategory(string categoryKey) { ... }
public async Task<Control> UpdateControl(Control control) { ... }
```

**❌ WRONG - Abbreviations:**
```csharp
public async Task<Proj> SelProj(int pId) { ... }
public async Task<List<Risk>> SelRisksByCat(string catKey) { ... }
```

### Null Checking
- Use `is null` or `is not null` for null checks
- Never use `== null` or `!= null`
- This aligns with modern C# patterns and nullable reference types

**✅ CORRECT:**
```csharp
if (risk is null)
	return null;

if (category is not null)
	return category;
```

**❌ WRONG - Traditional null comparison:**
```csharp
if (risk == null)
	return null;

if (category != null)
	return category;
```

---

## EF Core Query Patterns

### Read-Only Queries: Use AsNoTracking()
All queries that **do not modify data** must use `.AsNoTracking()`. This improves performance by preventing EF Core from tracking entities for change detection.

**✅ CORRECT:**
```csharp
public async Task<Risk> SelectRisk(int id)
{
	return await context.Risks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
}

public async Task<List<Control>> SelectControls()
{
	return await context.Controls.AsNoTracking().ToListAsync();
}
```

**❌ WRONG - Without AsNoTracking():**
```csharp
public async Task<Risk> SelectRisk(int id)
{
	return await context.Risks.FirstOrDefaultAsync(x => x.Id == id);  // Missing AsNoTracking()
}
```

### Insert Operations: Return the Entity
When inserting records, **always return the inserted entity** after saving changes. This allows the calling code to access any auto-generated properties (like database-generated IDs).

**✅ CORRECT:**
```csharp
public async Task<Risk> InsertRisk(Risk risk)
{
	await context.Risks.AddAsync(risk);
	await context.SaveChangesAsync();
	return risk;  // Return the entity with any auto-generated values
}
```

**❌ WRONG - Not returning the entity:**
```csharp
public async Task InsertRisk(Risk risk)
{
	await context.Risks.AddAsync(risk);
	await context.SaveChangesAsync();
	// Missing return statement
}
```

### Update Operations: Return the Updated Entity
For update operations, modify the entity and save changes, then **return the updated entity**.

**✅ CORRECT:**
```csharp
public async Task<Risk> UpdateRisk(Risk risk)
{
	context.Risks.Update(risk);
	await context.SaveChangesAsync();
	return risk;
}
```

**❌ WRONG - Not returning result:**
```csharp
public async Task UpdateRisk(Risk risk)
{
	context.Risks.Update(risk);
	await context.SaveChangesAsync();
}
```

### Delete Operations: Use ExecuteDeleteAsync()
For delete operations, use `.ExecuteDeleteAsync()` instead of `.Remove()` followed by `.SaveChangesAsync()`. This generates a more efficient SQL DELETE statement.

**✅ CORRECT:**
```csharp
public async Task DeleteRisk(int id)
{
	await context.Risks.Where(x => x.Id == id).ExecuteDeleteAsync();
}
```

**❌ WRONG - Using Remove + SaveChanges:**
```csharp
public async Task DeleteRisk(int id)
{
	Risk risk = await context.Risks.FirstOrDefaultAsync(x => x.Id == id);
	if (risk is not null)
	{
		context.Risks.Remove(risk);
		await context.SaveChangesAsync();
	}
}
```

### Include Related Entities
When you need related entities, use `.Include()` to load navigation properties in a single query.

**✅ CORRECT:**
```csharp
public async Task<Project> SelectProject(int id)
{
	return await context.Projects
		.Include(x => x.Risks)
		.Include(x => x.Controls)
		.AsNoTracking()
		.FirstOrDefaultAsync(x => x.Id == id);
}
```

### Async All the Way
- **Always use async/await** for all database operations
- Use `FirstOrDefaultAsync()`, `ToListAsync()`, `CountAsync()`, etc.
- Never use `.Result` or `.Wait()` - these can cause deadlocks in ASP.NET Core

**✅ CORRECT:**
```csharp
public async Task<List<Risk>> SelectRisks()
{
	return await context.Risks.AsNoTracking().ToListAsync();
}
```

**❌ WRONG - Synchronous operations:**
```csharp
public List<Risk> SelectRisks()
{
	return context.Risks.AsNoTracking().ToList();  // Blocking call
}
```

---

## Method Naming Conventions

### Standard CRUD Method Names
Data layer methods follow a strict naming convention using verb prefixes:

| Operation | Method Name | Example |
|-----------|------------|---------|
| **SELECT** (single) | `Select[Entity]` | `SelectRisk(int id)` |
| **SELECT** (multiple) | `Select[Entities]` | `SelectRisks()` |
| **INSERT** | `Insert[Entity]` | `InsertRisk(Risk risk)` |
| **UPDATE** | `Update[Entity]` | `UpdateRisk(Risk risk)` |
| **DELETE** | `Delete[Entity]` | `DeleteRisk(int id)` |

### Pluralization Rule
- Use **singular entity name** for single record: `SelectRisk()`, `InsertRisk()`, `UpdateRisk()`, `DeleteRisk()`
- Use **plural entity name** for collections: `SelectRisks()`, `SelectControlsByProject()`

**✅ CORRECT:**
```csharp
public async Task<Risk> SelectRisk(int id) { ... }           // Single record
public async Task<List<Risk>> SelectRisks() { ... }          // Multiple records
public async Task<List<Risk>> SelectRisksByProject(int projectId) { ... }
public async Task<Risk> InsertRisk(Risk risk) { ... }        // Insert single
public async Task<Risk> UpdateRisk(Risk risk) { ... }        // Update single
public async Task DeleteRisk(int id) { ... }                 // Delete single
```

**❌ WRONG - Incorrect naming:**
```csharp
public async Task<Risk> GetRisk(int id) { ... }              // Use "Select" not "Get"
public async Task<List<Risk>> GetRisks() { ... }             // Use "Select" not "Get"
public async Task<List<Risk>> SelectRiskByProject(...) { ... }  // Should be plural "SelectRisks"
public async Task CreateRisk(...) { ... }                    // Use "Insert" not "Create"
public async Task UpsertRisk(...) { ... }                    // Use "Insert" or "Update"
```

### Method Parameters
- Use descriptive parameter names: `int id`, `int projectId`, `string categoryKey`
- Never use single-letter names: `int i`, `int p`
- Use `int id` for single entity identifier, `int entityId` for disambiguation

**✅ CORRECT:**
```csharp
public async Task<Risk> SelectRisk(int id) { ... }
public async Task<List<Control>> SelectControlsByRisk(int riskId) { ... }
public async Task<List<Risk>> SelectRisksByProject(int projectId) { ... }
```

---

## Error Handling

### Throw ApplicationException for User-Facing Errors
When an operation fails due to business logic constraints (e.g., entity not found, validation failed), throw an `ApplicationException`.

**✅ CORRECT:**
```csharp
public async Task DeleteRisk(int id)
{
	Risk risk = await context.Risks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
	if (risk is null)
		throw new ApplicationException($"Risk with ID {id} not found.");
	
	await context.Risks.Where(x => x.Id == id).ExecuteDeleteAsync();
}
```

### Let Other Exceptions Propagate
- Database connection errors, timeout errors, etc., should propagate up to the controller
- The controller will handle these with custom exception middleware
- Do NOT catch and suppress exceptions

**✅ CORRECT:**
```csharp
public async Task<Risk> SelectRisk(int id)
{
	// If database is unavailable, the exception will propagate to the controller
	return await context.Risks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
}
```

### No Generic Try-Catch Blocks
- Avoid broad `try-catch` blocks that catch all exceptions
- Only catch specific exceptions if you have a legitimate recovery strategy
- Let exceptions propagate to be handled at the API layer

---

## Using Statements

### GlobalUsings.cs Convention (Critical)
**Never use local `using` statements in Data project files.** All `using` statements must be placed in a single `GlobalUsings.cs` file at the project root.

**✅ CORRECT - Data/GlobalUsings.cs:**
```csharp
global using Microsoft.EntityFrameworkCore;
global using Models;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using Telerik.Documents.Core.Fonts;
```

**Data/RiskDL.cs - No using statements:**
```csharp
namespace Data;

public class RiskDL(MRCDBContext context)
{
	public async Task<Risk> SelectRisk(int id)
	{
		return await context.Risks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
	}
}
```

**❌ WRONG - Using statements in individual files:**
```csharp
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data;

public class RiskDL(MRCDBContext context) { ... }
```

### GlobalUsings.cs Content
Include only the namespaces used by **all or most** files in the project. This keeps the file concise and maintainable.

**Standard Data/GlobalUsings.cs:**
```csharp
global using Microsoft.EntityFrameworkCore;
global using Models;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
```

---

## Entity Relationships

### Foreign Key Naming Convention
- Use the pattern: `[EntityName]Id` for foreign key properties
- Example: `RiskId`, `ProjectId`, `CategoryId`
- Always use `int` for primary and foreign keys

### Include Navigation Properties
When querying related entities, use `.Include()` to efficiently load related data in a single query.

**✅ CORRECT:**
```csharp
public async Task<Project> SelectProject(int id)
{
	return await context.Projects
		.Include(x => x.Risks)
		.Include(x => x.Controls)
		.AsNoTracking()
		.FirstOrDefaultAsync(x => x.Id == id);
}
```

### Avoid N+1 Queries
- **Bad:** Query parent entity, then loop and query child entities separately
- **Good:** Use `.Include()` to load all related entities in one query

**❌ WRONG - N+1 Query Problem:**
```csharp
List<Project> projects = await context.Projects.AsNoTracking().ToListAsync();
foreach (Project project in projects)
{
	project.Risks = await context.Risks.Where(x => x.ProjectId == project.Id).ToListAsync();  // N queries!
}
```

**✅ CORRECT - Single Query with Includes:**
```csharp
List<Project> projects = await context.Projects
	.Include(x => x.Risks)
	.AsNoTracking()
	.ToListAsync();  // One query!
```

---

## Best Practices

### 1. Keep DL Classes Focused
- Each DL class should handle **one entity type** only
- Don't create "utility" DL classes that handle multiple entities
- Example: `RiskDL` handles Risk entity, `ProjectDL` handles Project entity

### 2. Use Consistent Async Patterns
- All methods that perform database operations should be `async`
- Return `Task<T>` or `Task` (not `void`)
- Use `await` when calling async methods

**✅ CORRECT:**
```csharp
public async Task<Risk> SelectRisk(int id) { ... }
public async Task DeleteRisk(int id) { ... }
public async Task<List<Risk>> SelectRisks() { ... }
```

### 3. Keep Performance in Mind
- Always use `.AsNoTracking()` for read-only queries
- Use `.ExecuteDeleteAsync()` for bulk deletes instead of loading entities first
- Consider pagination for large result sets
- Use `.Select()` to project only needed columns when appropriate

### 4. Validate Input Parameters
- Check for invalid IDs (e.g., id <= 0)
- Validate required string parameters
- Throw `ApplicationException` with descriptive messages

**✅ CORRECT:**
```csharp
public async Task<Risk> SelectRisk(int id)
{
	if (id <= 0)
		throw new ApplicationException("ID must be greater than 0.");
	
	return await context.Risks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
}
```

### 5. Document Complex Logic
- Add comments only for **complex logic**, not for self-explanatory code
- Explain **why** something is done, not just **what**
- Use XML comments for public methods if needed

**✅ CORRECT:**
```csharp
// Only load active risks to improve query performance
public async Task<List<Risk>> SelectActiveRisks()
{
	return await context.Risks
		.Where(x => x.IsActive)
		.AsNoTracking()
		.ToListAsync();
}
```

### 6. Transaction Handling
- Use transactions when multiple operations must succeed together
- Let Entity Framework manage transactions via `SaveChangesAsync()`
- Explicitly use `Database.BeginTransactionAsync()` for complex scenarios

---

## Reference Examples

### Complete RiskDL Example
```csharp
namespace Data;

public class RiskDL(MRCDBContext context)
{
	public async Task<Risk> SelectRisk(int id)
	{
		if (id <= 0)
			throw new ApplicationException("Risk ID must be greater than 0.");

		return await context.Risks
			.Include(x => x.Project)
			.Include(x => x.Category)
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == id);
	}

	public async Task<List<Risk>> SelectRisks()
	{
		return await context.Risks
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<List<Risk>> SelectRisksByProject(int projectId)
	{
		if (projectId <= 0)
			throw new ApplicationException("Project ID must be greater than 0.");

		return await context.Risks
			.Where(x => x.ProjectId == projectId)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<Risk> InsertRisk(Risk risk)
	{
		if (risk is null)
			throw new ApplicationException("Risk cannot be null.");

		await context.Risks.AddAsync(risk);
		await context.SaveChangesAsync();
		return risk;
	}

	public async Task<Risk> UpdateRisk(Risk risk)
	{
		if (risk is null)
			throw new ApplicationException("Risk cannot be null.");

		context.Risks.Update(risk);
		await context.SaveChangesAsync();
		return risk;
	}

	public async Task DeleteRisk(int id)
	{
		if (id <= 0)
			throw new ApplicationException("Risk ID must be greater than 0.");

		await context.Risks.Where(x => x.Id == id).ExecuteDeleteAsync();
	}
}
```

### Complete GlobalUsings.cs Example
```csharp
global using Microsoft.EntityFrameworkCore;
global using Models;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
```

---

## Verification Checklist

Before committing Data layer code, verify:

- [ ] File uses file-scoped namespace (`namespace Data;`)
- [ ] Class name follows pattern: `[Entity]DL`
- [ ] Primary constructor used for DI: `public class RiskDL(MRCDBContext context)`
- [ ] All methods are `async` and return `Task<T>` or `Task`
- [ ] Read-only queries use `.AsNoTracking()`
- [ ] Delete operations use `.ExecuteDeleteAsync()`
- [ ] Insert/Update methods return the entity
- [ ] Method names follow CRUD convention: `Select`, `Insert`, `Update`, `Delete`
- [ ] Collections use plural: `SelectRisks()`, not `SelectRisk()`
- [ ] No `var` used - all types are explicit
- [ ] Indentation uses tabs, not spaces
- [ ] No local `using` statements - all in `GlobalUsings.cs`
- [ ] Error handling uses `ApplicationException` for business logic errors
- [ ] No abbreviations in names (except standard acronyms)
- [ ] Comments explain **why**, not **what**
- [ ] Null checking uses `is null` / `is not null`

---

## Related Documentation
- [AI Coding Guide](./copilot-instructions.md)
- [C# Development Guidelines](./csharp.instructions.md)
- [Models Project Rules and Standards](./models.instructions.md)