# Telerik UI for Blazor 12 Migration Guide

## Objective
Update the solution to be compatible with Telerik UI for Blazor 12, resolving breaking changes and ensuring functionality.

---

## 1. Namespace Changes and Type Ambiguity

### Problem:
Some custom model names may conflict with types included in Telerik or other libraries.

Detected cases:
- `Area`
- `Sustainability`

The ambiguity can appear in both `.razor` and `.razor.cs` files.

---

### General Solution

- **Do not use `using Models;` to resolve these conflicts.**
- Always use the **fully qualified name** when referring to these custom models.

That is, instead of `Area` or `Sustainability`, you must use:

- `Models.Area`
- `Models.Sustainability`

---

## 2. Removal of the `Centered` Property in `<TelerikWindow>`

### Problem:
- The `Centered="true"` property no longer exists in Telerik 12.

### Solution:
- **Remove any use of `Centered="true"`** in all `<TelerikWindow>` components.
- If you need to center the window, use the new positioning options in Telerik 12 (see official documentation).

---

## 3. Additional Changes and Specific Recommendations for Telerik 12

### 3.1 NuGet Package Update
- Make sure the `Telerik.UI.for.Blazor` package in your Web.csproj is updated to version 12.x.x.
- If you use other Telerik packages (Spreadsheet, PDF, etc.), update them also to version 12.x.x if available.

### 3.2 Changes in TelerikGrid
- Review the pagination API (`PageSize`, `TotalCount`, events like `OnRead`).
- If you use editing, review the changes in editing parameters and methods.
- Verify changes in column definition (`Field`, `Title`, templates).

### 3.3 Changes in TelerikTextArea and TelerikTextBox
- Review supported parameters and changes in validation.
- Adjust integration with `EditForm` and `ValidationSummary` if necessary.

### 3.4 Changes in TelerikDropDownList and Select Components
- Review the use of `ValueField`, `TextField` and data handling.
- Validate the behavior of `DefaultText`.

### 3.5 Changes in TelerikWindow
- Already covered: Remove `Centered`.
- Review new positioning and sizing parameters.

### 3.6 Changes in Icons
- If you use SVG icons (`SvgIcon`), review the new way to import and use them.
- Update the import in `_Imports.razor` if necessary.

### 3.7 Changes in Themes and CSS
- Review changes in CSS classes, themes or structure of Telerik 12 style files.
- Update style files and theme references if necessary.

### 3.8 Changes in TelerikLoaderContainer
- Review available parameters and events.
- The visibility and animation behavior may have changed.

### 3.9 Changes in Validation
- Review integration with Blazor `EditForm` and changes in error messages.

### 3.10 Compatibility with .NET 10
- Telerik 12 is compatible with .NET 10, but make sure all packages and dependencies are aligned with this version.

---

## 4. Adjustments in Imports

- Make sure you have in `_Imports.razor`:
  ```razor
  @using Telerik.Blazor.Components
  @using Telerik.SvgIcons
  ```
- If you use custom components, check if they require new imports or namespace changes.

---

## 5. Changes in CSS and Themes

- Check if there are changes in CSS classes, themes or structure of Telerik 12 style files.
- Update style files and theme references if necessary.

---

## 6. Validation and Testing

1. **Compile the solution** and fix any type ambiguity errors or deprecated parameters.
2. **Visually test** all `<TelerikWindow>` dialogs, grids, forms and custom components.
3. **Consult the official documentation** of Telerik 12 for additional breaking changes:
   - https://docs.telerik.com/blazor-ui/upgrade/overview
   - https://docs.telerik.com/blazor-ui/release-notes
   - https://docs.telerik.com/blazor-ui/upgrade/breaking-changes

---


## 7. Rules on Additional Properties in Components

**Do not add or modify additional properties like `FillMode` and `Size` in Telerik components if they do not exist in the original code.**

- If the component already has these properties, leave them as they are.
- If it does not have those properties, do not add or modify them, as the default values continue to work correctly in Telerik 12.
- Apply this rule to all components, especially buttons (`TelerikButton`), text boxes, dropdown lists, etc.

This ensures that the original design and functionality are maintained and avoids unnecessary changes to the markup.

---

## 8. Summary of Common Changes

- [ ] Add `using Models;` where necessary for the Area model.
- [ ] Remove `Centered="true"` from `<TelerikWindow>`.
- [ ] Search and replace all `k-icon` with equivalent `SvgIcon`.
- [ ] Review and update Telerik component parameters.
- [ ] Validate imports and themes.
- [ ] Visually and functionally test the entire app.

---

## 9. Changes in Export and Import Methods of Telerik.Documents.Spreadsheet

### 9.1 Export

**Before (deprecated):**
```csharp
using (MemoryStream stream = new())
{
    XlsxFormatProvider formatProvider = new();
    formatProvider.Export(workbook, stream, TimeSpan.FromMinutes(5));
    excelBytes = stream.ToArray();
}
```

**Now (Telerik 12):**
```csharp
XlsxFormatProvider formatProvider = new();
excelBytes = formatProvider.Export(workbook);
```
- The Export method now returns the byte array directly. It does not require MemoryStream or timeout.

### 9.2 Import

**Before (deprecated):**
```csharp
Workbook workbook = formatProvider.Import(stream, TimeSpan.FromSeconds(240));
```

**Now (Telerik 12):**
```csharp
Workbook workbook = formatProvider.Import(stream);
```
- The Import method only accepts the stream or byte array, it no longer accepts timeout.

---

## 10. Rule for Export Methods in Other Formats (CSV, PDF, etc.)

- **For XLSX files:** Use `XlsxFormatProvider.Export(workbook)` which returns a byte array.
- **For CSV files:** Use `CsvFormatProvider.Export(workbook, stream)` with a `Stream` (does not return byte[]).
- **For other formats (PDF, etc.):** Check the documentation, but normally the method with `Stream` is used.

**Example for CSV:**
```csharp
using (var stream = new FileStream(fileName, FileMode.Create))
{
    CsvFormatProvider formatProvider = new();
    formatProvider.Export(workbook, stream);
}
```

**Do not attempt to use the method that returns byte[] for CSV, because it does not exist.**

---

## 11. Verification of Menu Button in MainLayout

Verify that the menu button in the main layout (`MainLayout`) has the following markup:

```razor
<TelerikButton OnClick="@(() => DrawerRef.ToggleAsync())" Icon="@SvgIcon.Menu" FillMode="@ThemeConstants.Button.FillMode.Flat" Class="drawer-toggle-button" />
```

If the button is not present or does not match exactly this format, update it to comply with this structure. This ensures visual and functional compatibility with Telerik 12 and sidebar menu styles.

---

## 12. Changes in LoginDisplay.razor Component

Update the `LoginDisplay.razor` component to simplify login/logout buttons according to Telerik 12.

### 12.1 Logout Button

**Before (deprecated):**
```razor
<button style="display: inline-block; color:white; background-color:#404040;" @onclick="BeginLogout"><span class="k-icon k-i-logout"></span></button>
```

**Now (Telerik 12):**
```razor
<button @onclick="BeginLogOut">Log Out</button>
```

### 12.2 Login Link

**Before (deprecated):**
```razor
<a href="authentication/login" style="color:white;"><span class="k-icon k-i-login"></span></a>
```

**Now (Telerik 12):**
```razor
<button @onclick="BeginLogIn">Log In</button>
```

### 12.3 Methods in @code Section

**Correct structure:**
```csharp
@code{
	public void BeginLogOut()
	{
		NavigationManager.NavigateToLogout("authentication/logout");
	}

	public void BeginLogIn()
	{
		NavigationManager.NavigateTo(NavigationManager.BaseUri);
	}
}
```

**Note:** The method name changed from `BeginLogout` to `BeginLogOut` (with capital O), and `BeginLogIn` was added to handle navigation to login.

### 12.4 NavigationManager Injection

**Correct structure:**
```razor
@inject NavigationManager NavigationManager
```

Make sure NavigationManager is injected in the component so that navigation methods work correctly.

---

## 13. Migration of Kendo UI Icons (k-icon) to Telerik SvgIcon

### 13.1 IMPORTANT: Applicability by Project Type

⚠️ **This section ONLY applies to Razor Pages (.razor) in Blazor Projects** ⚠️

- **Blazor Projects (.razor):** You MUST migrate `k-icon` → `SvgIcon`
- **WebCore/JavaScript Projects (.aspx, .cshtml, .js):** The `k-icon` continue to work normally. **NO CHANGES REQUIRED.**

If your project uses both (Blazor and WebCore), apply the changes ONLY in the Blazor folder.

---

### 13.2 Problem (Blazor Only)
Kendo UI icons (`k-icon k-i-*`) use **icon font CSS classes** that are **NOT compatible with Telerik 12**. When rendered through `MarkupString`, these icons do not display correctly because the fonts are not available or the styles have changed.

### 13.2 Symptoms
- Icons do not display in the application
- Blank boxes or unrendered characters appear
- Functionality works but the UI looks broken

### 13.3 Search for k-icon Icons in Code

Run this **regex** search in your IDE to find all `k-icon`:

```
k-i-[a-z0-9\-]+
```

**Places to search:**
1. `.razor` files - in HTML sections
2. `.cs` / `.razor.cs` files - in methods that return HTML (strings with `<span class="k-icon...">`)
3. `HelperWeb.cs` or similar files - in methods that build HTML

### 13.4 Mapping Table: k-icon → SvgIcon

| Original k-icon | Equivalent SvgIcon | Usage |
|---|---|---|
| `k-i-download` | `SvgIcon.Download` | Download files |
| `k-i-file` | `SvgIcon.File` | Generic files |
| `k-i-folder` | `SvgIcon.Folder` | Folders |
| `k-i-edit` | `SvgIcon.Edit` | Edit |
| `k-i-delete` | `SvgIcon.Trash` | Delete |
| `k-i-save` | `SvgIcon.Save` | Save |
| `k-i-plus` | `SvgIcon.Plus` | Add |
| `k-i-minus` | `SvgIcon.Minus` | Remove |
| `k-i-search` | `SvgIcon.Search` | Search |
| `k-i-filter` | `SvgIcon.Filter` | Filter |
| `k-i-excel` | `SvgIcon.FileXls` | Excel file |
| `k-i-pdf` | `SvgIcon.FilePdf` | PDF file |
| `k-i-login` | `SvgIcon.LogIn` | Sign in |
| `k-i-logout` | `SvgIcon.LogOut` | Sign out |
| `k-i-menu` | `SvgIcon.Menu` | Menu |
| `k-i-close` | `SvgIcon.X` | Close |
| `k-i-check` | `SvgIcon.Check` | Check/Accept |
| `k-i-arrow-left` | `SvgIcon.ArrowLeft` | Left arrow |
| `k-i-arrow-right` | `SvgIcon.ArrowRight` | Right arrow |
| `k-i-sort-asc` | `SvgIcon.SortAscending` | Sort ascending |
| `k-i-sort-desc` | `SvgIcon.SortDescending` | Sort descending |

### 13.5 Migration Options

#### Option A: Use TelerikSvgIcon in Templates (RECOMMENDED)

**Before (Telerik 12 - DOES NOT WORK):**
```razor
<Template>
    @((MarkupString)HelperWeb.BuildLink())
</Template>
```

**Now (Telerik 12 - WORKS):**
```razor
<Template Context="item">
    @{
        var myItem = item as MyModel;
        string url = HelperWeb.GetDownloadUrl(myItem.Id, NavigationManager.BaseUri);
    }
    <a href="@url" target="_blank">
        <TelerikSvgIcon Icon="@SvgIcon.Download"></TelerikSvgIcon>
    </a>
</Template>
```

**Advantages:**
- Icons render correctly
- Automatic dynamic CSS
- Improved accessibility

#### Option B: Use SvgIcon in Buttons Directly

**In Telerik components (GridCommandButton, TelerikButton):**
```razor
<GridCommandButton OnClick="@OnClickDownload" Icon="@SvgIcon.Download">Download</GridCommandButton>
```

Does not require `MarkupString`, the component handles the icon automatically.

### Option C: NOT ALLOWED - DO NOT USE

You are not allowed to build HTML with inline SVG in helper methods. If it MUST be an HTML string, **refactor the code to use options A or B instead.**

The solution is always to abandon `MarkupString` and use native Razor components.

---

### 13.6 Step-by-Step Migration Procedure

1. **Search for all occurrences:**
   ```
   k-i- or k-icon
   ```

2. **For each occurrence:**
   - Identify the specific icon (e.g., `k-i-download`)
   - Find its SvgIcon equivalent (e.g., `SvgIcon.Download`)
   - Determine if it is in:
     - A Razor template → **Use Option A** (TelerikSvgIcon)
     - A Telerik button → **Use Option B** (Icon property)
     - A helper method → **Evaluate if you can avoid MarkupString**

3. **Replace:**
   - Remove the tag `<span class="k-icon k-i-..."></span>`
   - Replace with `<TelerikSvgIcon Icon="@SvgIcon.IconName"></TelerikSvgIcon>`

4. **Test:**
   - Compile
   - Visually verify that icons display

### 13.7 Migration Checklist

- [ ] Search for `k-icon` in all `.razor` and `.cs` files
- [ ] Search for `k-i-` in methods that return HTML strings
- [ ] Replace in Grid templates
- [ ] Replace in Icon properties of Telerik components
- [ ] Verify that all `_Imports.razor` have `@using Telerik.SvgIcons`
- [ ] Compile the solution
- [ ] Visually test each page and dialog

---
