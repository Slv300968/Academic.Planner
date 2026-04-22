# Migration Rule: Modal Rendering and StateHasChanged in Blazor

## Problem
After migrating to .NET 10 and/or updating Blazor, modals may stop displaying correctly if the component state is not explicitly re-rendered after changing visibility or data variables.

This happens because Blazor does not automatically detect some state changes (e.g., `WindowIsVisible = true`) unless you call `StateHasChanged()` at the right moment, especially when using async methods or updating properties before/after await.

## Rule
**Whenever you change the visibility state of a modal, or variables that affect its rendering, call `StateHasChanged()` immediately after the change.**

This applies to:
- Showing or hiding modals (`WindowIsVisible`, `IsDialogOpen`, etc.)
- Changes to data that affect modal content
- Before and after async operations that modify the visual state

## Correct Example
```csharp
public async Task ShowDialog(int userId, string method)
{
    IsLoading = true;
    try
    {
        User = new();
        WindowIsVisible = true;
        StateHasChanged(); // <-- Force re-render after showing modal
        UserId = userId;
        Method = method;
        await FillDropDownRoles();
        await FillDropDownSocieties();
        Title = "New user";
        SelectedSocieties = [];
        if (Method.Equals("UPDATE", StringComparison.OrdinalIgnoreCase))
        {
            Title = "Edit user";
            User = await UserService.SelectUser(UserId);
            SelectedSocieties = User.Societies.Select(x => x.Key).ToList();
        }
    }
    catch (Exception ex)
    {
        await Dialogs.AlertAsync(HelperWeb.GetErrorMessageForClient(ex.Message), HelperWeb.ErrorTitle);
    }
    finally
    {
        IsLoading = false;
        StateHasChanged(); // <-- Force re-render after finishing
    }
}
```

## Incorrect Example
```csharp
public async Task ShowDialog(string method, int userId)
{
    IsLoading = true;
    try
    {
        ClearForm();
        Method = method;
        WindowIsVisible = true; // <-- Missing StateHasChanged()
        Title = Method == "UPDATE" ? "Edit" : "New";
        FillDropDownRoles();
        List<Task> tasks = [ SelectSocieties() ];
        await Task.WhenAll(tasks);
        if (Method == "UPDATE")
            await FillFormUser(userId);
        FillSocietiesGrids();
    }
    catch (Exception ex)
    {
        await Dialogs.AlertAsync(HelperWeb.GetErrorMessageForClient(ex.Message), HelperWeb.ErrorTitle);
    }
    finally
    {
        IsLoading = false;
    }
}
```

## General Automated Solution
1. **Search all methods that show/hide modals or change visibility variables.**
2. **Add `StateHasChanged();` immediately after changing the visibility variable.**
3. **Add `StateHasChanged();` in the `finally` block of async methods that affect the visual state.**

## Recommendation
- Apply this rule to all components managing modals, not just UserData.
- If you use UI libraries (Radzen, MudBlazor, Telerik, etc.), check if they require a similar pattern.
- Consider creating a helper or base method for showing modals that always calls `StateHasChanged()`.

---

**This rule is essential after Blazor/.NET migrations to avoid rendering and user experience issues in all system modals.**
