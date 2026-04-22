---
applyTo: Models/**
---

# Models Layer - Development Guidelines

This document outlines the patterns, conventions, and best practices for working with entity models in all projects for the organization.

## Project Structure

The **Models** project contains all entity classes, data transfer objects, configuration models, and grid/display models used across the solution. It serves as the shared data contract between all layers.

## Core Patterns

### Entity Models
Entity models represent database tables and follow these conventions:

```csharp
namespace Models;

public class Risk
{
	public int Id { get; set; }                    // Primary key (always int)
	[Required, StringLength(30)]
	public string Key { get; set; }               // Business key (string)
	[Required, StringLength(255)]
	public string Name { get; set; }              // Display name
	public int ProjectId { get; set; }            // Foreign key
	
	// Navigation properties
	public Project Project { get; set; }
}
```

### Grid/Display Models
Separate models for grid displays and specialized views:

```csharp
public class ProjectGrid
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string AnalysisTypeName { get; set; }   // Flattened navigation property
	public DateOnly StartDate { get; set; }
	public string UsersDisplayNames { get; set; }  // Concatenated related data
}


## Data Annotations Conventions

### Required Fields
- **Primary business identifiers**: Always `[Required]`
- **Dont add required to the fields. Avoid public required string Name { get; set; }, for example. 
- **Foreign keys**: Required unless explicitly nullable
- **Core business properties**: Use `[Required]` with localized error messages

```csharp
[Required(ErrorMessage = "- El campo Nombre es requerido.")]
[StringLength(100, ErrorMessage = "- La longitud máxima del campo Nombre es de 100 caracteres.")]
public string Name { get; set; }
```

### String Length Constraints
- **Keys**: `[StringLength(30)]` or `[StringLength(50)]`
- **Names/Titles**: `[StringLength(100)]` to `[StringLength(255)]`
- **Descriptions**: `[StringLength(500)]` to `[StringLength(1000)]`
- **Long text fields**: `[StringLength(1700)]`
- **Principal names**: `[StringLength(128)]` for UPN, `[StringLength(255)]` for display names

```csharp
[Required, StringLength(50)]
public string Key { get; set; }

[StringLength(255)]
public string Principal { get; set; }

[StringLength(1700)]
public string PossibleCauses { get; set; }
```

### Date Handling
- Use `DateOnly` for dates without time components
- Use `DateTime` for audit fields (Created, Modified)

```csharp
[Required(ErrorMessage = "- El campo Fecha inicio es requerido.")]
public DateOnly StartDate { get; set; }

public DateTime Created { get; set; }
```

## Key Patterns

### Primary Keys
- **Always use `int Id`** as primary key
- No composite keys - use surrogate keys

### Business Keys
- Use `string Key` properties for business identifiers
- Length typically 20-50 characters
- Required and indexed in database
- Add [Key] attribute if needed for clarity

### Foreign Key Relationships
- Use `int PropertyNameId` for foreign key properties
- Include corresponding navigation property
- Example: `public int ProjectId { get; set; }` with `public Project Project { get; set; }`

### Navigation Properties
- **Single reference**: `public Project Project { get; set; }`
- **Collections**: `public List<Risk> Risks { get; set; }`
- Dont add nullable to navigation properties 
- Configure relationships in `MRCDBContext.OnModelCreating()`

### Nullable Properties
- Use nullable types (`int?`, `string?`) for optional fields
- Foundation year, counts, optional descriptions

```csharp
public int? FoundationYear { get; set; }
public int? NumberOfStudents { get; set; }
[StringLength(200)]
public string? Principal { get; set; }
```

## Naming Conventions

### Class Names
- **Entity models**: Singular noun (`Risk`, `Project`, `User`)
- **Grid models**: `{Entity}Grid` (`ProjectGrid`, `UserGrid`)
- **Card/Display models**: `{Entity}Card` (`ProjectCard`)
- **Configuration models**: `{Purpose}Settings` (`MRCAppSettings`)

### Property Names
- **PascalCase**: All public properties
- **Descriptive names**: Avoid abbreviations
- **Boolean properties**: `Active`, `IsEnabled` (not `IsActive`)
- **Navigation properties**: Match the related entity name

### Localized Properties
- Use Spanish comments for business context
- Error messages in Spanish
- Property names in English

```csharp
public string Name { get; set; }
[Required(ErrorMessage = "- El campo Nombre es requerido.")]
```

## Configuration Models

### Application Settings
Use dedicated models for configuration binding, add these properties to `MRCAppSettings` as minimum:

```csharp
public class MRCAppSettings
{
	public string ApplicationKey { get; set; }
	public string ClientKey { get; set; }
	public string Environment { get; set; }
	public string Location { get; set; }
	public string LogUrl { get; set; }
	public string UtilitiesApi { get; set; }
	public string UtilitiesApiNF { get; set; }
	public string JwtKey { get; set; }
}

```

### Environment-Specific Settings
- Properties should match `appsettings.json` structure exactly
- Use appropriate data types (`string`, `int`, `bool`)
- No validation attributes - handle validation in consuming code

## Global Using Directives

The `Models/GlobalUsings.cs` file includes common namespaces:

```csharp
global using System.ComponentModel.DataAnnotations;
global using System.Text.Json;
global using System.ComponentModel.DataAnnotations.Schema;
```

**Rules for GlobalUsings.cs:**
- Only include namespaces used in 80%+ of model classes
- Data annotations for validation
- JSON serialization attributes
- Database mapping attributes

## Data Transfer Patterns

### Grid Models
- Include only properties needed for display
- Flatten navigation properties (e.g., `ProjectName` instead of `Project.Name`)
- Concatenate related collections as display strings
- Add computed properties for UI formatting

### API Response Models
- Create specialized models for complex API responses
- Include computed properties for client convenience
- Example: `RiskMatrixRow` with `Dictionary<int, string> PhaseRiskLevels`

## Validation Guidelines

### Business Rules
- Implement validation attributes at the model level
- Use localized error messages in Spanish
- Combine multiple validation attributes when needed

```csharp
[Required(ErrorMessage = "- El campo Usuario es requerido")]
[StringLength(128)]
public string UserPrincipalName { get; set; }
```

### Cross-Property Validation
- Implement `IValidatableObject` for complex business rules
- Use custom validation attributes for reusable validations
- Handle cross-entity validation in the business logic layer

## Collection Properties

### Navigation Collections
- Always use `List<T>` for navigation collections
- Initialize in constructors if needed for EF Core
- Configure relationships in `MRCDBContext`

```csharp
public List<Risk> Risks { get; set; }
public List<Phase> Phases { get; set; }
public List<User> Users { get; set; }
```

## Special Patterns

## Code Style

### Property Declaration
- **Explicit types**: Never use `var`
- **Object initialization**: Use target-typed new `new()`
- **Property formatting**: One property per line

### Comments
- Use Spanish comments for business context
- Include domain knowledge in comments
- Explain calculated properties and complex relationships

### Indentation
- **Use tabs** (not spaces)
- Consistent indentation for attributes and properties
- Group related properties together

## Integration Points

### EF Core Configuration
- Models are mapped in `Data/MRCDBContext.cs`
- Use Fluent API for complex relationships
- Configure indexes, constraints, and cascading rules

### JSON Serialization
- Models are serialized for API responses
- Use `System.Text.Json` attributes when needed
- Test serialization for complex nested objects

### Validation Integration
- Validation attributes work with both client-side (Blazor) and server-side (API) validation
- Error messages display in Telerik components
- Custom validators can be added for business rules

## Testing Considerations

### Model Validation
- Test validation attributes with various inputs
- Verify error message localization
- Test edge cases for string lengths and date ranges

### Serialization Testing
- Verify JSON serialization/deserialization
- Test circular reference handling
- Validate complex object graphs

## Common Pitfalls

### Avoid These Patterns
- ❌ Using `var` for property types
- ❌ Composite primary keys
- ❌ Abbreviations in property names
- ❌ Navigation properties without corresponding foreign keys
- ❌ Mixing validation logic between models and business layer
- ❌ Using spaces for indentation
- ❌ Missing error message localization

### Best Practices
- ✅ Consistent naming conventions
- ✅ Appropriate data annotations
- ✅ Separate models for different purposes (entity vs. grid vs. card)
- ✅ Proper foreign key relationships
- ✅ Localized error messages
- ✅ Tab indentation
- ✅ Business context in comments