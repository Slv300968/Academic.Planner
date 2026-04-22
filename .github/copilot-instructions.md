# Custom instructions for copilot

- Indent with tabs

## General Guidelines
- Do not split Telerik tags into multiple lines (N rows). Always use 1 line, except for TelerikGrid which can be multiline.

## Additional Api project coding standards

- All controllers must inherit from ControllerBase and use [ApiController] and [Route("[controller]")] attributes.
- Use dependency injection via primary constructor for all dependencies (ILogger, DL classes, etc).
- All controller action methods must be async Task<ActionResult> or async Task<IActionResult> for async operations.
- Use explicit types for all variables, never use var.
- Use PascalCase for class names and method names, camelCase for local variables and parameters.
- Use pluralized method names for returning collections (e.g., SelectAssets, SelectUsersGrid).
- Use [FromBody] only for complex types in POST/PUT methods.
- Do not use abbreviations in function, variable, or parameter names.
- Use [HttpGet("ActionName")], [HttpPost("ActionName")], [HttpPut("ActionName")], [HttpDelete("ActionName")] for controller actions. The action name must be explicitly specified in the attribute, matching the method name. Do not use plain [HttpGet], [HttpPost], etc.
- Do not use NotFound() for null results, returning null is acceptable.
- Always log the action at the start of each controller method.
- Do not use local usings; all usings must be in GlobalUsings.cs.
- Use string interpolation for logging parameters.
- Use explicit return types for all methods.
- Use Ok() for successful responses.
- For dependency injection, do not assign injected parameters to local variables or fields unless required by framework (e.g., IOptions).
- Use only one statement per line.
- For object instantiation, use the short form: Workbook workbook = new ();
- For one-line if statements, do not use braces.
- Don't use string interpolation in logger methods.

## Data project coding standards

- All DL (Data Layer) classes must use primary constructor for dependency injection (e.g., public class AssetDL(MRCDBContext context)).
- All DL classes must be in the Data namespace.
- Use explicit types for all variables, never use var.
- Use PascalCase for class names and method names, camelCase for local variables and parameters.
- Use pluralized method names for returning collections (e.g., SelectAssets, SelectCentersGrid).
- Do not use abbreviations in function, variable, or parameter names.
- Use async/await for all database operations that return data or modify state.
- Use .ToListAsync(), .FirstOrDefaultAsync(), etc. for EF Core queries.
- Use .OrderBy or .OrderByDescending for sorting collections from the database.
- Use .Include for related entities when needed.
- For object instantiation, use the short form: Workbook workbook = new ();
- Do not use local usings; all usings must be in GlobalUsings.cs.
- Use only one statement per line.
- For one-line if statements, do not use braces.
- Use full property names and avoid abbreviations in all code.
- Use explicit return types for all methods.
- Use dependency injection for MRCDBContext, do not instantiate it directly.
- Use .AsNoTracking() for read-only queries when appropriate.
- Use regions or comments to separate helper methods if needed.
- In Insert methods, return the inserted entity.
- In Update methods, return the updated entity.
- Always use ExecuteDeleteAsync in Delete methods.

## ApiClient project coding standards

- All service classes must use primary constructor for dependency injection (e.g., public class AssetService(IHttpClientFactory httpClientFactory)).
- All service classes must be in the ApiClient namespace.
- Use explicit types for all variables, never use var.
- Use PascalCase for class names and method names, camelCase for local variables and parameters.
- Use pluralized method names for returning collections (e.g., SelectAssets, SelectUsersGrid).
- Do not use abbreviations in function, variable, or parameter names.
- Use async/await for all HTTP operations.
- Use HttpClient from IHttpClientFactory, do not instantiate HttpClient directly.
- Use HttpResponseMessage and check IsSuccessStatusCode for error handling.
- Use JsonSerializer for serialization/deserialization.
- For object instantiation, use the short form: Workbook workbook = new ();
- Do not use local usings; all usings must be in GlobalUsings.cs.
- Use only one statement per line.
- For one-line if statements, do not use braces.
- Use explicit return types for all methods.
- Use full property names and avoid abbreviations in all code.
- Use string interpolation for building URLs and logging.
- Throw ApplicationException with the response content for error handling.
- Return null or default values for NoContent responses as appropriate.

## Models project coding standards

- All model classes must be in the Models namespace.
- Use PascalCase for class names and property names.
- Use explicit types for all properties, never use var.
- Use [Required], [MaxLength], [Range], [Column], and other data annotations for validation and schema mapping.
- Use full property names and avoid abbreviations.
- Use List<T> for collection navigation properties.
- Use only one statement per line.
- For one-line if statements, do not use braces.
- For object instantiation, use the short form: Workbook workbook = new ();
- Do not use local usings; all usings must be in GlobalUsings.cs.
- Use explicit return types for all methods.
- Use regions or comments to separate helper classes if needed.

## Web project coding standards

- All components and services must use PascalCase for class names and method names, camelCase for local variables and parameters.
- Use explicit types for all variables, never use var.
- Use dependency injection via [Inject] for all services and dependencies.
- All C# code for Razor components must be in code-behind files (.razor.cs).
- The only .razor files allowed to contain C# code are LoginDisplay.razor, RedirectToLogin.razor, and Authentication.razor.
- Use partial classes for code-behind files (.razor.cs) and keep markup-only UI in .razor files and all component logic in .razor.cs files.
- Use only one statement per line.
- For one-line if statements, do not use braces.
- For object instantiation, use the short form: Workbook workbook = new ();
- Do not use local usings; all usings must be in GlobalUsings.cs.
- Use explicit return types for all methods.
- Use regions or comments to separate helper methods if needed.
- Use EventCallback for event communication between components.
- Use [Parameter] and [CascadingParameter] for component parameters.
- Use string interpolation for building messages and logging.
- Use async/await for all asynchronous operations.
- Use full property names and avoid abbreviations in all code.
- Use proper error handling and display user-friendly error messages.
- UI validation should use DataAnnotations and ValidationSummary.
- Do not put logic in .razor files; keep it in .razor.cs files, except in LoginDisplay.razor, RedirectToLogin.razor, and Authentication.razor.
