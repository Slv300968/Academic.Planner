---
applyTo: Web/**
---

# Web Project Rules and Standards

## Overview
The **Web** project is the Blazor WebAssembly front-end application. All components follow strict naming conventions, code style standards, and architectural patterns to ensure consistency and maintainability across the solution. This project uses Telerik UI for Blazor components and Microsoft Authentication Library (MSAL) for Azure AD authentication.

---

## Table of Contents
1. [File Structure and Namespaces](#file-structure-and-namespaces)
2. [Class Naming and Organization](#class-naming-and-organization)
3. [Primary Constructors and Dependency Injection](#primary-constructors-and-dependency-injection)
4. [Code Style Guidelines](#code-style-guidelines)
5. [Component Patterns](#component-patterns)
6. [Method Naming Conventions](#method-naming-conventions)
7. [Error Handling](#error-handling)
8. [Using Statements](#using-statements)
9. [State Management](#state-management)
10. [Best Practices](#best-practices)
11. [MrcComponentBase and ExecuteLoadingAction](#mrccomponentbase-and-executeloadingaction)
12. [Reference Examples](#reference-examples)

---

## File Structure and Namespaces

### File-Scoped Namespaces (Required)
All Web project code-behind files must use **file-scoped namespaces** ending with a semicolon.

**✅ CORRECT:**
```csharp
namespace Web.Pages;

public partial class AssetsGrid
{
	// Implementation
}
```

**❌ WRONG - Block-scoped namespace:**
```csharp
namespace Web.Pages
{
	public partial class AssetsGrid
	{
		// Implementation
	}
}
```

### Namespace Convention
- Pages use: `namespace Web.Pages;`
- Layout components use: `namespace Web.Layout;`
- Services use: `namespace Web.Services;`
- Root-level files use: `namespace Web;`

### Folder Structure
```
Web/
├── Layout/           # Layout components and state management
├── Pages/            # Page components (.razor and .razor.cs)
│   ├── Administration/   # Admin pages
│   └── Reports/          # Report pages
├── Services/         # Web-specific services
├── Resources/        # Localization resources
└── wwwroot/          # Static assets
```

---

## Class Naming and Organization

### Naming Convention: Descriptive Names
Component classes follow descriptive naming patterns:

**Grid Pages:**
- `AssetsGrid` - Grid page for Assets
- `SocietiesGrid` - Grid page for Societies
- `UsersGrid` - Grid page for Users

**Data/Form Pages:**
- `AssetData` - Form/detail page for Asset
- `SocietyData` - Form/detail page for Society
- `UserData` - Form/detail page for User

**✅ CORRECT:**
```csharp
namespace Web.Pages;

public partial class AssetsGrid
{
	[Inject] public AssetService AssetService { get; set; }
	// Implementation
}
```

```csharp
namespace Web.Pages;

public partial class AssetData
{
	[Parameter] public int? AssetId { get; set; }
	// Implementation
}
```

**❌ WRONG - Incorrect naming patterns:**
```csharp
public partial class AssetsList { ... }      // Use "Grid" suffix
public partial class AssetForm { ... }       // Use "Data" suffix
public partial class Asset_Page { ... }      // Wrong pattern
```

### File Organization
- **Razor and code-behind separation** - Every `.razor` file has a corresponding `.razor.cs` file
- **Never write C# code in .razor files** - All logic goes in code-behind, except for `LoginDisplay.razor`, `RedirectToLogin.razor`, and `Authentication.razor`
- **One component per file** - Each component has its own file pair
- **Partial classes** - Code-behind uses `partial class` to match the Razor component

### Razor Code Rule
- All Razor component C# code must live in the matching `.razor.cs` file.
- The only allowed exceptions are `LoginDisplay.razor`, `RedirectToLogin.razor`, and `Authentication.razor`, which may contain Blazor plumbing code in the `.razor` file.
- Outside those exceptions, `.razor` files should contain markup and component directives only.

---

## Primary Constructors and Dependency Injection

### Use [Inject] Attribute for DI
Blazor components use the `[Inject]` attribute for dependency injection on properties.

**✅ CORRECT - Property Injection:**
```csharp
namespace Web.Pages;

public partial class AssetsGrid
{
	[Inject] private NavigationManager NavigationManager { get; set; }
	[Inject] public AssetService AssetService { get; set; }
	[Inject] public StateProvider StateProvider { get; set; }
	[CascadingParameter] public DialogFactory Dialogs { get; set; }
	// Implementation
}
```

**❌ WRONG - Constructor injection in components:**
```csharp
public partial class AssetsGrid
{
	private readonly AssetService _assetService;

	public AssetsGrid(AssetService assetService)
	{
		_assetService = assetService;  // Wrong for Blazor components
	}
}
```

### Primary Constructors for Services
Web services (non-component classes) **must** use primary constructors.

**✅ CORRECT - Service with Primary Constructor:**
```csharp
namespace Web.Services;

public class HelperWebNS(IHttpClientFactory httpClientFactory)
{
	public string GetDownloadAssetDocumentLink(int? assetDocumentId, string assetDocumentName)
	{
		HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
		// Implementation
	}
}
```

### Common Injected Services
- `NavigationManager` - For navigation
- `StateProvider` - For application state
- `[Entity]Service` - For API calls (from ApiClient project)
- `IJSRuntime` - For JavaScript interop
- `DialogFactory` - For Telerik dialogs (use `[CascadingParameter]`)

---

## Code Style Guidelines

### Indentation
- **Use tabs** for indentation, NOT spaces

**✅ CORRECT:**
```csharp
public partial class AssetsGrid
{
→	[Inject] public AssetService AssetService { get; set; }

→	protected async override Task OnInitializedAsync()
→	{
→	→	await SelectAssetsFacade();
→	}
}
```

### Type Declarations
- **Never use `var`** - Always use explicit type names

**✅ CORRECT:**
```csharp
List<AssetGrid> items = await AssetService.SelectAssetsGrid(searchText, assetStatusActive, groupId);
Asset asset = await AssetService.SelectAsset(id);
bool isConfirmed = await Dialogs.ConfirmAsync("Message", "Title");
```

**❌ WRONG - Using `var`:**
```csharp
var items = await AssetService.SelectAssetsGrid(searchText, assetStatusActive, groupId);
var asset = await AssetService.SelectAsset(id);
```

### Object Instantiation
- Use **target-typed new** expression: `new()` when type is obvious
- Use **collection expressions** `[]` for empty lists

**✅ CORRECT:**
```csharp
Asset Asset = new();
List<AssetGrid> AssetsData = [];
List<Society> SocietiesOwners = [];
StringBuilder errorMessageSB = new();
```

**❌ WRONG - Redundant type specification:**
```csharp
Asset Asset = new Asset();
List<AssetGrid> AssetsData = new List<AssetGrid>();
```

### One-Line If Statements
- **No braces** for single-line if statements

**✅ CORRECT:**
```csharp
if (string.IsNullOrEmpty(value))
	return;

if (Asset.NeighborhoodId == Helper.ASSET_NEIGHBORHOODOTHERID)
	DisplayNeighborhoodOther = "";
```

**❌ WRONG - Unnecessary braces:**
```csharp
if (string.IsNullOrEmpty(value))
{
	return;
}
```

### Naming Conventions - General
- **No abbreviations** except standard acronyms (API, HTTP, JS)
- Use PascalCase for properties, methods, and class names
- Use camelCase for local variables and private fields
- Private fields without underscore prefix: `private List<Asset> AssetsData = [];`

**✅ CORRECT:**
```csharp
private List<AssetGrid> AssetsData = [];
private bool IsLoading = true;
private string SearchText = "";
public string AssetGeoReference { get; set; }
```

### Null Checking
- Use `is null` or `is not null` for null checks

**✅ CORRECT:**
```csharp
if (user is null)
	return;

if (realEstateTerritory is not null)
	Asset.TerritoryKey = realEstateTerritory.Territory.Key;
```

**❌ WRONG:**
```csharp
if (user == null)
	return;
```

---

## Component Patterns

### Razor File Rules
- **Never write C# code in .razor files** - Use code-behind (.razor.cs), except in `LoginDisplay.razor`, `RedirectToLogin.razor`, and `Authentication.razor`
- Keep Razor files focused on markup and Blazor directives
- Use `@attribute [Authorize]` for protected pages

**✅ CORRECT - .razor file:**
```razor
@page "/assets"
@attribute [Authorize(Policy = "UserRequired")]
@inherits MrcComponentBase

<div class="header-container">
    <h4>Inventario de inmuebles</h4>
</div>
<TelerikGrid @ref="AssetsGridRef" Data="@AssetsData">
    <!-- Grid markup -->
</TelerikGrid>
```

**❌ WRONG - C# code in .razor file:**
```razor
@page "/assets"

@code {
    private List<Asset> items;  // Wrong! Move to code-behind
    
    protected override async Task OnInitializedAsync()
    {
        // Wrong! Move to code-behind
    }
}
```

### Page Component Structure
```csharp
namespace Web.Pages;

// Base class declared in .razor file via @inherits MrcComponentBase
// MrcComponentBase provides: IsLoading, Dialog (DialogFactory)
public partial class AssetsGrid
{
	// 1. Injected services
	[Inject] private NavigationManager NavigationManager { get; set; }
	[Inject] public AssetService AssetService { get; set; }

	// 2. Parameters
	[Parameter] public int? AssetId { get; set; }

	// 3. Public properties (bound to UI)
	public string AssetGeoReference { get; set; }

	// 4. Private fields
	private List<AssetGrid> AssetsData = [];

	// 5. Lifecycle methods
	protected async override Task OnInitializedAsync()
	{
		await SelectAssetsFacade();
	}

	// 6. Event handlers (OnClick, OnChange, etc.)
	protected void OnClickNavigateInsertAsset()
	{
		NavigationManager.NavigateTo("asset");
	}

	// 7. Helper methods
	protected async Task SelectAssetsFacade()
	{
		// Implementation
	}
}
```

### Edit vs View Mode
- Use computed property `IsEdit` to determine mode

**✅ CORRECT:**
```csharp
[Parameter] public int? AssetId { get; set; }
private bool IsEdit => AssetId is not null;

protected override async Task OnInitializedAsync()
{
	Title = IsEdit ? "Editar" : "Nuevo";
	if (IsEdit)
		await FillFormAsset(AssetId.Value);
}
```

---

## Method Naming Conventions

### Standard Method Prefixes

| Purpose | Prefix | Example |
|---------|--------|---------|
| Click handler | `OnClick` | `OnClickNavigateInsertAsset()` |
| Change handler | `OnChange` | `OnChangeAssetStatusActive()` |
| Blur handler | `OnBlur` | `OnBlurZipCode()` |
| Submit handler | `OnSubmit` | `OnSubmitAsset()` |
| Fill dropdown | `FillDropDown` | `FillDropDownSocieties()` |
| Select data | `Select` | `SelectAssetsFacade()` |
| Set value | `Set` | `SetAssetAddress()` |
| Navigation | `NavigateTo` / `BackTo` | `BackToAssetsGrid()` |

**✅ CORRECT:**
```csharp
protected void OnClickNavigateInsertAsset()
{
	NavigationManager.NavigateTo("asset");
}

protected async Task OnChangeGroupOption(object value)
{
	// Handle change
}

protected async Task FillDropDownSocietiesOwners()
{
	SocietiesOwners = await SocietyService.SelectSocieties_Type(Helper.SOCIETYTYPEKEY_OWNER);
}

protected void BackToAssetsGrid()
{
	NavigationManager.NavigateTo("assets");
}
```

### Facade Pattern for Data Loading
Use `*Facade` suffix for methods that wrap async loading operations via `ExecuteLoadingAction` (from `MrcComponentBase`).

**✅ CORRECT:**
```csharp
protected async Task SelectAssetsFacade()
{
	await ExecuteLoadingAction(async () =>
	{
		AssetsData = await SelectAssetsGrid();
	});
}

protected async Task<List<AssetGrid>> SelectAssetsGrid()
{
	return await AssetService.SelectAssetsGrid(SearchText, assetStatusActive, StateProvider.UserGroupId);
}
```

**❌ WRONG - Manual try-catch:**
```csharp
protected async Task SelectAssetsFacade()
{
	IsLoading = true;
	try
	{
		AssetsData = await SelectAssetsGrid();
	}
	catch (Exception ex)
	{
		await Dialog.AlertAsync(Utilities.GetErrorMessageForClient(ex.Message), Utilities.DIALOGTITLE_ERROR);
	}
	finally
	{
		IsLoading = false;
	}
}
```

---

## Error Handling

### Display Errors with Dialogs
Use `Dialogs.AlertAsync` for displaying errors to users.

**✅ CORRECT:**
```csharp
catch (Exception ex)
{
	await Dialogs.AlertAsync(Utilities.GetErrorMessageForClient(ex.Message), Utilities.DIALOGTITLE_ERROR);
}
```

### Confirmation Dialogs
Use `Dialogs.ConfirmAsync` for user confirmations.

**✅ CORRECT:**
```csharp
protected async Task OnClickOpenDialogDeleteAsset()
{
	AssetGrid asset = SelectedAssets.FirstOrDefault();
	bool isConfirmed = await Dialogs.ConfirmAsync($"¿Está seguro de eliminar el inmueble '{asset.Name}'?", "Confirmar eliminación");
	if (!isConfirmed)
		return;
	await AssetService.DeleteAsset(asset.Id);
	await SelectAssetsFacade();
}
```

### Form Validation
Use JavaScript interop for form validation alerts.

**✅ CORRECT:**
```csharp
protected async Task<bool> IsCustomFormValid()
{
	bool formValid = true;
	StringBuilder errorMessageSB = new();

	if (string.IsNullOrEmpty(Asset.Name))
		errorMessageSB.AppendLine("- El campo Nombre es requerido.");

	if (errorMessageSB.Length > 0)
	{
		formValid = false;
		string errorMessage = $"Favor de corregir los siguientes errores:\n{errorMessageSB}";
		await JSRuntime.InvokeAsync<string>("alert", errorMessage);
	}
	return formValid;
}
```

### Loading State
`IsLoading` is managed automatically by `ExecuteLoadingAction` from `MrcComponentBase`. **Never** set `IsLoading` manually or write try-catch blocks.

**✅ CORRECT:**
```csharp
protected async Task SaveAssetFacade()
{
	if (!await IsCustomFormValid())
		return;
	await ExecuteLoadingAction(async () =>
	{
		Asset = await AssetService.InsertAsset(Asset);
	});
}
```

**❌ WRONG - Manual IsLoading/try-catch:**
```csharp
protected async Task SaveAssetFacade()
{
	IsLoading = true;
	try
	{
		Asset = await AssetService.InsertAsset(Asset);
	}
	catch (Exception ex)
	{
		await Dialog.AlertAsync(Utilities.GetErrorMessageForClient(ex.Message), Utilities.DIALOGTITLE_ERROR);
	}
	finally
	{
		IsLoading = false;
	}
}
```

---

## Using Statements

### GlobalUsings.cs Convention (Critical)
**Never use local `using` statements in Web project files.** All using statements must be in `GlobalUsings.cs`.

**✅ CORRECT - Web/GlobalUsings.cs:**
```csharp
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
global using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
global using Microsoft.JSInterop;
global using Models;
global using MRC.DTI.Dev.Utilities.Business;
global using System.Text;
global using System.Text.Json;
global using Telerik.Blazor;
global using Telerik.Blazor.Components;
global using Web;
global using Web.Layout;
global using Web.Services;
global using ApiClient;
```

**Web/Pages/AssetsGrid.razor.cs - No using statements (exception for specific Telerik imports):**
```csharp
using Telerik.Blazor.Components.Grid;  // Exception: specific component imports

namespace Web.Pages;

public partial class AssetsGrid
{
	// Implementation
}
```

### _Imports.razor
The `_Imports.razor` file contains Razor-specific imports for all `.razor` files.

---

## State Management

### StateProvider Pattern
Use `StateProvider` for application-wide state.

**✅ CORRECT:**
```csharp
[Inject] public StateProvider StateProvider { get; set; }

protected override async Task OnInitializedAsync()
{
	int userId = StateProvider.UserId;
	int groupId = StateProvider.UserGroupId;
	bool isReadOnly = StateProvider.UserGroupReadOnly;
}
```

### Grid State Persistence
Persist grid filters and settings in `StateProvider.GridSettings`.

**✅ CORRECT:**
```csharp
protected void OnGridStateChanged(GridStateEventArgs<AssetGrid> args)
{
	GridState<AssetGrid> stateGrid = AssetsGridRef.GetState();
	StateProvider.GridSettings[Helper.USERSETTINGS_ASSETSGRID_GRIDFILTERS] = JsonSerializer.Serialize(stateGrid);
}

protected async Task OnGridStateInit(GridStateEventArgs<AssetGrid> args)
{
	GridState<AssetGrid> storedStateGrid = StateProvider.GetGridSettingsItem<GridState<AssetGrid>>(Helper.USERSETTINGS_ASSETSGRID_GRIDFILTERS);
	if (storedStateGrid != null)
		args.GridState = storedStateGrid;
}
```

---

## Best Practices

### 1. Parallel Task Execution
Use `Task.WhenAll` for independent async operations.

**✅ CORRECT:**
```csharp
protected override async Task OnInitializedAsync()
{
	List<Task> tasks =
	[
		FillDropDownSocietiesOwners(),
		FillDropDownSocietiesOperators(),
		FillDropDownGovernments(),
		FillDropDownUses()
	];
	await Task.WhenAll(tasks);
}
```

### 2. Use Constants from Helper Class
Reference constants from `Helper` class for keys and values.

**✅ CORRECT:**
```csharp
if (StateProvider.UserRoleKey == Helper.ROLEKEY_SUPERADMIN)
	// Show admin options

SocietiesOwners = await SocietyService.SelectSocieties_Type(Helper.SOCIETYTYPEKEY_OWNER);
```

### 3. Navigation Patterns
Use `NavigationManager` for navigation.

**✅ CORRECT:**
```csharp
// Simple navigation
NavigationManager.NavigateTo("assets");

// Navigation with parameter
NavigationManager.NavigateTo($"Asset/{id}");

// Force reload
NavigationManager.NavigateTo("Assets", forceLoad: true);
```

### 4. Telerik Component References
Use typed references for Telerik components.

**✅ CORRECT:**
```csharp
private TelerikGrid<AssetGrid> AssetsGridRef;
private TelerikValidationSummary ValidationSummaryAsset;
```

---

## MrcComponentBase and ExecuteLoadingAction

All page components must inherit from `MrcComponentBase`. This base class provides `IsLoading` and `Dialog` (`DialogFactory`), so these must **never** be declared manually in code-behind files.

### Inherit from MrcComponentBase

- In the `.razor` file: add `@inherits MrcComponentBase` **after** the last `@attribute` directive.
- In the `.razor.cs` file: do **not** declare a base class — leave as `public partial class MyComponent`.
- **Remove** any manually declared `IsLoading` field and `[CascadingParameter] DialogFactory` property — they are provided by the base class.
- Access the dialog via `Dialog` (the base class property name).

**✅ CORRECT - .razor file:**
```razor
@page "/societies"
@attribute [Authorize(Policy = "UserRequired")]
@inherits MrcComponentBase
```

**✅ CORRECT - .razor.cs file:**
```csharp
namespace Web.Pages;

// No base class here — picked up from @inherits in the .razor file
public partial class SocietiesGrid
{
	[Inject] public SocietyService SocietyService { get; set; }
	// No IsLoading field — provided by MrcComponentBase
	// No [CascadingParameter] DialogFactory — provided by MrcComponentBase as Dialog
}
```

### Replace try-catch with ExecuteLoadingAction

All methods with `try-catch` blocks must use `ExecuteLoadingAction` instead. `ExecuteLoadingAction` handles `IsLoading`, error display, and `StateHasChanged()` automatically.

**✅ CORRECT:**
```csharp
protected async Task SelectSocietiesFacade()
{
	await ExecuteLoadingAction(async () =>
	{
		SocietiesData = await SocietyService.SelectSocietiesGrid();
	});
}
```

### Dialog Placement Rules

When combining dialogs with `ExecuteLoadingAction`, order matters:

| Action | Position |
|--------|----------|
| `ConfirmAsync` | **Before** `ExecuteLoadingAction`, early return on cancel |
| Validation (`IsValid`, `IsCustomFormValid`) | **Before** `ExecuteLoadingAction`, early return on failure |
| Success `AlertAsync` | **After** `ExecuteLoadingAction` |
| Error handling | Automatic — handled internally by `ExecuteLoadingAction` |

**✅ CORRECT - Full pattern with confirm + success alert:**
```csharp
protected async Task OnClickDeleteSociety()
{
	SocietyGrid society = SelectedSocieties.FirstOrDefault();
	if (!await Dialog.ConfirmAsync($"¿Está seguro de eliminar '{society.Name}'?", "Confirmar eliminación"))
		return;
	await ExecuteLoadingAction(async () =>
	{
		await SocietyService.DeleteSociety(society.Id);
		SocietiesData = await SocietyService.SelectSocietiesGrid();
	});
}
```

**❌ WRONG - Confirm or dialogs inside ExecuteLoadingAction:**
```csharp
protected async Task OnClickDeleteSociety()
{
	await ExecuteLoadingAction(async () =>
	{
		// WRONG: ConfirmAsync must be outside ExecuteLoadingAction
		bool isConfirmed = await Dialog.ConfirmAsync("Delete?", "Confirm");
		if (!isConfirmed)
			return;
		await SocietyService.DeleteSociety(SelectedItem.Id);
	});
}
```

---

## Reference Examples

### Complete Grid Page Example (Code-Behind)
```csharp
namespace Web.Pages;

// Base class declared in .razor via @inherits MrcComponentBase
// Provides: IsLoading, Dialog (DialogFactory)
public partial class SocietiesGrid
{
	[Inject] private NavigationManager NavigationManager { get; set; }
	[Inject] public SocietyService SocietyService { get; set; }
	[Inject] public StateProvider StateProvider { get; set; }

	private List<SocietyGrid> SocietiesData = [];
	private IEnumerable<SocietyGrid> SelectedSocieties = [];
	private TelerikGrid<SocietyGrid> SocietiesGridRef;

	protected async override Task OnInitializedAsync()
	{
		await SelectSocietiesFacade();
	}

	protected void OnClickNavigateInsertSociety()
	{
		NavigationManager.NavigateTo("society");
	}

	protected void OnClickNavigateUpdateSociety()
	{
		int id = SelectedSocieties.First().Id;
		NavigationManager.NavigateTo($"Society/{id}");
	}

	protected async Task OnClickOpenDialogDeleteSociety()
	{
		SocietyGrid society = SelectedSocieties.FirstOrDefault();
		if (!await Dialog.ConfirmAsync($"¿Está seguro de eliminar '{society.Name}'?", "Confirmar eliminación"))
			return;
		await ExecuteLoadingAction(async () =>
		{
			await SocietyService.DeleteSociety(society.Id);
			SocietiesData = await SocietyService.SelectSocietiesGrid();
		});
	}

	protected async Task SelectSocietiesFacade()
	{
		await ExecuteLoadingAction(async () =>
		{
			SocietiesData = await SocietyService.SelectSocietiesGrid();
		});
	}

	protected void BackToAssetsGrid()
	{
		NavigationManager.NavigateTo("assets");
	}
}
```

### Complete GlobalUsings.cs Example
```csharp
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
global using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
global using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
global using Microsoft.JSInterop;
global using Models;
global using MRC.DTI.Dev.Utilities.Business;
global using System.Reflection;
global using System.Security.Claims;
global using System.Text;
global using System.Text.Json;
global using Telerik.Blazor;
global using Telerik.DataSource;
global using Telerik.Blazor.Components;
global using Telerik.Blazor.Services;
global using Web;
global using Web.Resources;
global using Web.Services;
global using Web.Layout;
global using ApiClient;
global using Telerik.SvgIcons;
```

---

## Verification Checklist

Before committing Web layer code, verify:

- [ ] File uses file-scoped namespace (`namespace Web.Pages;`)
- [ ] Component uses `partial class`
- [ ] All C# code is in code-behind (.razor.cs), not in .razor file
- [ ] Uses `[Inject]` attribute for dependency injection
- [ ] Services use primary constructors
- [ ] All methods are `async` when calling services
- [ ] Component inherits from `MrcComponentBase` via `@inherits MrcComponentBase` in `.razor` file
- [ ] No manually declared `IsLoading` field — provided by `MrcComponentBase`
- [ ] No manually declared `[CascadingParameter] DialogFactory` — provided by `MrcComponentBase` as `Dialog`
- [ ] All try-catch blocks replaced with `ExecuteLoadingAction`
- [ ] `ConfirmAsync` and validations placed **before** `ExecuteLoadingAction`
- [ ] Success `AlertAsync` placed **after** `ExecuteLoadingAction`
- [ ] Error handling uses `Dialog.AlertAsync` (via base class)
- [ ] Method names follow conventions (`OnClick`, `OnChange`, `FillDropDown`, etc.)
- [ ] No `var` used - all types are explicit
- [ ] Indentation uses tabs, not spaces
- [ ] No local `using` statements - all in `GlobalUsings.cs`
- [ ] Grid pages use `*Grid` suffix
- [ ] Form/detail pages use `*Data` suffix
- [ ] No abbreviations in names (except standard acronyms)
- [ ] Null checking uses `is null` / `is not null`
- [ ] Collection expressions `[]` used for empty lists

---

## Related Documentation
- [AI Coding Guide](../copilot-instructions.md)
- [Api Project Rules and Standards](./api.instructions.md)
- [ApiClient Project Rules and Standards](./apiclient.instructions.md)
- [Data Project Rules and Standards](./data.instructions.md)
- [Models Project Rules and Standards](./models.instructions.md)
