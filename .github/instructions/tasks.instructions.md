---
applyTo: Tasks/**
---

# Tasks Project Rules and Standards

## Overview
The **Tasks** project is a .NET Console Application for batch operations and scheduled jobs. All classes follow strict naming conventions, code style standards, and architectural patterns to ensure consistency and maintainability across the solution.

---

## Table of Contents
1. [File Structure and Namespaces](#file-structure-and-namespaces)
2. [Class Naming and Organization](#class-naming-and-organization)
3. [Primary Constructors and Dependency Injection](#primary-constructors-and-dependency-injection)
4. [Code Style Guidelines](#code-style-guidelines)
5. [Entry Point Pattern (Program.cs)](#entry-point-pattern-programcs)
6. [Task Class Pattern](#task-class-pattern)
7. [Error Handling](#error-handling)
8. [Logging Pattern](#logging-pattern)
9. [Using Statements](#using-statements)
10. [Configuration Management](#configuration-management)
11. [Best Practices](#best-practices)
12. [Reference Examples](#reference-examples)

---

## File Structure and Namespaces

### File-Scoped Namespaces (Required)
All Tasks project files must use **file-scoped namespaces** ending with a semicolon. This is the modern C# convention and improves readability.

**✅ CORRECT:**
```csharp
namespace Tasks;

public class ImportBankReconciliationReports(ILogger<ImportBankReconciliationReports> ilogger, ReportDL reportDL)
{
	// Implementation
}
```

**❌ WRONG - Block-scoped namespace:**
```csharp
namespace Tasks
{
	public class ImportBankReconciliationReports(ILogger<ImportBankReconciliationReports> ilogger, ReportDL reportDL)
	{
		// Implementation
	}
}
```

### Namespace Convention
- All classes in the Tasks project use the **single file-scoped namespace**: `namespace Tasks;`
- Do NOT use sub-namespaces for task classes (e.g., `namespace Tasks.Jobs;` is incorrect)

---

## Class Naming and Organization

### Naming Convention: Verb + Noun
Task classes should follow a descriptive naming pattern that indicates the action performed:

**Convention:**
- `ImportBankReconciliationReports` - Imports bank reconciliation reports from SAP
- `ImportWorkflows` - Imports workflow configurations
- `SendNotifications` - Sends email notifications to users

**✅ CORRECT:**
```csharp
namespace Tasks;

public class ImportBankReconciliationReports(ILogger<ImportBankReconciliationReports> ilogger, ReportDL reportDL)
{
	public async Task Init() { ... }
}
```

**❌ WRONG - Incorrect naming patterns:**
```csharp
public class BankReconciliationReportImporter { ... }  // Wrong pattern
public class ReportTask { ... }                        // Too generic
public class DoImport { ... }                          // Not descriptive
```

### File Organization
- **One public class per file** - Each task class should be in its own file
- **File name matches class name** - `ImportWorkflows.cs` contains `ImportWorkflows` class
- **Program.cs** handles service registration and command routing

---

## Primary Constructors and Dependency Injection

### Required: Primary Constructor Pattern
All task classes **must** use primary constructors for dependency injection. This is a C# 12+ feature that provides a clean, concise way to inject dependencies.

**✅ CORRECT - Primary Constructor:**
```csharp
namespace Tasks;

public class ImportBankReconciliationReports(ILogger<ImportBankReconciliationReports> ilogger, ReportDL reportDL, BankFromBankSapDL bankFromBankSapDL)
{
	public async Task Init()
	{
		ilogger.LogInformation("ImportBankReconciliationReports.Init()");
		List<BankReconciliationReport> items = await reportDL.SelectReports();
	}
}
```

**❌ WRONG - Traditional Constructor:**
```csharp
public class ImportBankReconciliationReports
{
	private readonly ILogger<ImportBankReconciliationReports> _logger;
	private readonly ReportDL _reportDL;

	public ImportBankReconciliationReports(ILogger<ImportBankReconciliationReports> logger, ReportDL reportDL)
	{
		_logger = logger;
		_reportDL = reportDL;
	}
}
```

### Dependency Injection Parameters
- **Required:** `ILogger<T> ilogger` - Always include logger for the task class
- **Required:** Data layer classes needed for the task (e.g., `ReportDL`, `UserDL`)
- **Optional:** `IOptions<MRCAppSettings> mrcAppSettings` - When app settings are needed
- **Naming:** Use lowercase `ilogger` for logger, lowercase names for DL classes
- **Access:** The injected parameters are automatically available as fields throughout the class

**✅ CORRECT:**
```csharp
public class SendNotifications(ILogger<SendNotifications> ilogger, ReportDL reportDL, UserDL userDL, EmailDL emailDL, IOptions<MRCAppSettings> mrcAppSettings)
{
	private readonly MRCAppSettings _mrcAppSettings = mrcAppSettings.Value;

	public async Task Init(string[] args)
	{
		ilogger.LogInformation("SendNotifications.Init(args: {Args})", args);
	}
}
```

### Service Registration in Program.cs
All task classes and their dependencies must be registered as **Singletons** in `RegisterServices()`:

```csharp
services.AddSingleton<BankFromBankSapDL>();
services.AddSingleton<ReportDL>();
services.AddSingleton<EmailDL>();
services.AddSingleton<UserDL>();
services.AddSingleton<WorkflowDL>();
services.AddSingleton<ImportBankReconciliationReports>();
services.AddSingleton<ImportWorkflows>();
services.AddSingleton<SendNotifications>();
```

---

## Code Style Guidelines

### Indentation
- **Use tabs** for indentation, NOT spaces
- Configure your editor to use tabs for the Tasks project
- Tabs provide better accessibility and flexibility for developers

### Type Declarations
- **Never use `var`** - Always use explicit type names
- Explicit types improve code readability and maintain clarity of intent

**✅ CORRECT:**
```csharp
List<BankReconciliationReport> items = await reportDL.SelectReports();
List<User> users = await userDL.SelectUsers(true);
string initialStatusKey = Helper.STATUS_CREATED;
```

**❌ WRONG - Using `var`:**
```csharp
var items = await reportDL.SelectReports();
var users = await userDL.SelectUsers(true);
```

### Object Instantiation
- Use **target-typed new** expression: `new()` when the type is obvious from context

**✅ CORRECT:**
```csharp
List<Workflow> items = [];
BankReconciliationReport itemClone = new()
{
	AccountDate = item.AccountDate,
	StatusKey = item.StatusKey
};
```

**❌ WRONG - Redundant type specification:**
```csharp
List<Workflow> items = new List<Workflow>();
BankReconciliationReport itemClone = new BankReconciliationReport();
```

### One-Line If Statements
- **No braces** for single-line if statements

**✅ CORRECT:**
```csharp
if (items.Count == 0)
	return;

if (args.Length == 0)
	return;
```

**❌ WRONG - Unnecessary braces:**
```csharp
if (items.Count == 0)
{
	return;
}
```

### Null Checking
- Use `is null` or `is not null` for null checks
- Use null-coalescing assignment `??=` when appropriate

**✅ CORRECT:**
```csharp
if (user == null)
	continue;

bankFromBankSap ??= banksFromBankSap.Where(x => x.BankShortKey.StartsWith(rfcWorkflow.BankShortKey)).FirstOrDefault();
```

---

## Entry Point Pattern (Program.cs)

### Structure
The Program.cs file follows a specific structure:
1. Static `_serviceProvider` and `configuration` fields
2. `Main` method with command-line argument handling
3. `RegisterServices` method for DI configuration
4. `DisposeServices` method for cleanup

### Command-Line Argument Handling
Commands are passed as uppercase strings and routed to the appropriate task:

**✅ CORRECT:**
```csharp
static async Task Main(string[] args)
{
	RegisterServices();
	if (args.Length == 0)
		return;

	string command = args[0];
	try
	{
		if (command == "IMPORTBANKRECONCILIATIONREPORTS")
		{
			ImportBankReconciliationReports appService = _serviceProvider.GetService<ImportBankReconciliationReports>();
			await appService.Init();
		}
		else if (command == "IMPORTWORKFLOWS")
		{
			ImportWorkflows appService = _serviceProvider.GetService<ImportWorkflows>();
			await appService.Init();
		}
		else if (command == "SENDNOTIFICATIONS")
		{
			SendNotifications appService = _serviceProvider.GetService<SendNotifications>();
			await appService.Init(args);
		}
	}
	catch (Exception ex)
	{
		ILogger<Program> logger = _serviceProvider.GetService<ILogger<Program>>();
		logger.LogError(ex, "{Message}: ", ex.Message);
		Console.WriteLine(ex.Message);
	}
	finally
	{
		DisposeServices();
	}
}
```

### Service Registration
Configure services, logging, and database context:

```csharp
private static void RegisterServices()
{
	configuration = new ConfigurationBuilder()
		.SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
		.AddJsonFile("appsettings.json", false)
		.Build();
	
	IServiceCollection services = new ServiceCollection();
	IConfigurationSection mrcAppSettingsSection = configuration.GetSection("MRCAppSettings");
	services.Configure<MRCAppSettings>(mrcAppSettingsSection);
	services.AddDbContext<MRCDBContext>(options => 
		options.UseSqlServer(configuration.GetConnectionString("BankReconciliationReports")));
	
	// Register DL classes as Singletons
	services.AddSingleton<ReportDL>();
	services.AddSingleton<UserDL>();
	
	// Register task classes as Singletons
	services.AddSingleton<ImportBankReconciliationReports>();
	services.AddSingleton<SendNotifications>();

	// Configure Serilog
	MRCAppSettings mrcAppSettings = mrcAppSettingsSection.Get<MRCAppSettings>();
	Logger logger = new LoggerConfiguration()
		.WriteTo.Seq(mrcAppSettings.LogUrl, LogEventLevel.Information)
		.Enrich.WithProperty("ClientKey", mrcAppSettings.ClientKey)
		.Enrich.WithProperty("ApplicationKey", mrcAppSettings.ApplicationKey)
		.Enrich.WithProperty("Environment", mrcAppSettings.Environment)
		.Enrich.WithProperty("Location", mrcAppSettings.Location)
		.CreateLogger();
	services.AddLogging(builder => 
	{ 
		builder.SetMinimumLevel(LogLevel.Information); 
		builder.AddSerilog(logger: logger, dispose: true); 
	});
	
	_serviceProvider = services.BuildServiceProvider();
}
```

---

## Task Class Pattern

### Init Method Convention
All task classes must have an `Init` method as the entry point:

**✅ CORRECT - Without arguments:**
```csharp
public async Task Init()
{
	ilogger.LogInformation("ImportBankReconciliationReports.Init()");
	// Task logic here
}
```

**✅ CORRECT - With arguments:**
```csharp
public async Task Init(string[] args)
{
	ilogger.LogInformation("SendNotifications.Init(args: {Args})", args);
	// Task logic here
}
```

### Helper Methods
- Use `private` access modifier for helper methods
- Use `static` when the method doesn't need instance data
- Name helper methods descriptively

**✅ CORRECT:**
```csharp
private async Task SendEmailNotification_UserStatus(User user, string statusKey, List<BankReconciliationReport> items)
{
	// Implementation
}

public static BankReconciliationReport CloneBRR(BankReconciliationReport item)
{
	// Implementation
}
```

---

## Error Handling

### Try-Catch in Loops
When processing multiple items, wrap individual item processing in try-catch to prevent one failure from stopping the entire batch:

**✅ CORRECT:**
```csharp
foreach (BankReconciliationReport item in items)
{
	try
	{
		await ProcessItem(item);
	}
	catch (Exception ex)
	{
		ilogger.LogError(ex, "message: {Message}", ex.Message);
		Console.WriteLine(ex.Message);
	}
}
```

### Top-Level Exception Handling
Program.cs wraps the entire execution in try-catch-finally:

```csharp
try
{
	// Execute task
}
catch (Exception ex)
{
	ILogger<Program> logger = _serviceProvider.GetService<ILogger<Program>>();
	logger.LogError(ex, "{Message}: ", ex.Message);
	Console.WriteLine(ex.Message);
}
finally
{
	DisposeServices();
}
```

### Console Output for Errors
Always output error messages to console for visibility in scheduled job logs:

```csharp
catch (Exception ex)
{
	ilogger.LogError(ex, "message: {Message}", ex.Message);
	Console.WriteLine(ex.Message);  // Always include this
}
```

---

## Logging Pattern

### Structured Logging
Use structured logging with Serilog. Log at method entry with parameters:

**✅ CORRECT:**
```csharp
ilogger.LogInformation("ImportBankReconciliationReports.Init()");
ilogger.LogInformation("SendNotifications.Init(args: {Args})", args);
ilogger.LogError(ex, "SendNotifications.Init( user: {@User})", user);
```

**❌ WRONG - String interpolation:**
```csharp
ilogger.LogInformation($"ImportBankReconciliationReports.Init()");
ilogger.LogError(ex, $"Error processing user: {user}");
```

### Log Levels
- `LogInformation` - Task start, significant progress points
- `LogError` - Exceptions and failures (always include exception object)

### Serilog Configuration
Configure Serilog to write to Seq server with enriched properties:

```csharp
Logger logger = new LoggerConfiguration()
	.WriteTo.Seq(mrcAppSettings.LogUrl, LogEventLevel.Information)
	.Enrich.WithProperty("ClientKey", mrcAppSettings.ClientKey)
	.Enrich.WithProperty("ApplicationKey", mrcAppSettings.ApplicationKey)
	.Enrich.WithProperty("Environment", mrcAppSettings.Environment)
	.Enrich.WithProperty("Location", mrcAppSettings.Location)
	.CreateLogger();
```

---

## Using Statements

### GlobalUsings.cs Convention (Critical)
**Never use local `using` statements in Tasks project files.** All `using` statements must be placed in a single `GlobalUsings.cs` file at the project root.

**✅ CORRECT - Tasks/GlobalUsings.cs:**
```csharp
global using Data;
global using Models;
global using Microsoft.Extensions.Options;
global using System.Text;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Serilog;
global using Serilog.Events;
global using Serilog.Core;
```

**Tasks/ImportWorkflows.cs - No using statements:**
```csharp
namespace Tasks;

public class ImportWorkflows(ILogger<ImportWorkflows> ilogger, UserDL userDL, WorkflowDL workflowDL)
{
	public async Task Init()
	{
		ilogger.LogInformation("ImportWorkflows.Init()");
	}
}
```

**❌ WRONG - Using statements in individual files:**
```csharp
using Microsoft.Extensions.Logging;
using Data;

namespace Tasks;

public class ImportWorkflows(ILogger<ImportWorkflows> ilogger, UserDL userDL) { ... }
```

---

## Configuration Management

### appsettings.json Structure
```json
{
	"ConnectionStrings": {
		"BankReconciliationReports": "connection-string-here"
	},
	"MRCAppSettings": {
		"ApplicationKey": "MRC.ATMex.Administration.BankReconciliationReports",
		"ClientKey": "ATMEX",
		"Environment": "DEV",
		"Location": "USA",
		"LogUrl": "https://seq-server-url/",
		"UtilitiesApi": "https://api-url/",
		"ServerSapRead": "SAPSERVER",
		"ServerSapWrite": "SAPSERVER"
	}
}
```

### Accessing Configuration
Use `IOptions<MRCAppSettings>` pattern:

```csharp
public class SendNotifications(ILogger<SendNotifications> ilogger, IOptions<MRCAppSettings> mrcAppSettings)
{
	private readonly MRCAppSettings _mrcAppSettings = mrcAppSettings.Value;

	public async Task Init(string[] args)
	{
		string apiUrl = _mrcAppSettings.UtilitiesApi;
	}
}
```

### CronJobs.txt
Document scheduled execution times for reference:

```plaintext
*/5 * * * * dotnet /tasks/Tasks.dll IMPORTBANKRECONCILIATIONREPORTS
*/5 * * * * dotnet /tasks/Tasks.dll IMPORTWORKFLOWS
45 17 * * * dotnet /tasks/Tasks.dll SENDNOTIFICATIONS
```

---

## Best Practices

### 1. Keep Tasks Focused
- Each task class should handle **one specific job**
- Don't combine unrelated operations in a single task
- Create separate task classes for different concerns

### 2. Use Helper Class Constants
Reference constants from `Helper` class for status keys and role keys:

```csharp
string initialStatusKey = Helper.STATUS_CREATED;
string nextStatusKey = Helper.STATUS_PENDINGREVIEW;
item.RoleKey = Helper.ConvertRoleKeySAPToRoleKey(item.RoleKey);
```

### 3. Batch Processing
When processing collections, consider:
- Individual error handling per item
- Progress logging for long-running tasks
- Batch updates when possible for performance

### 4. Async All the Way
- All methods that call data layer should be `async`
- Use `await` for all async operations
- Never use `.Result` or `.Wait()`

### 5. Clean Up Resources
Always dispose services in `finally` block:

```csharp
finally
{
	DisposeServices();
}
```

---

## Reference Examples

### Complete Task Class Example
```csharp
namespace Tasks;

public class ImportBankReconciliationReports(ILogger<ImportBankReconciliationReports> ilogger, ReportDL bankReconciliationReportDL, BankFromBankSapDL bankFromBankSapDL)
{
	public async Task Init()
	{
		ilogger.LogInformation("ImportBankReconciliationReports.Init()");
		string initialStatusKey = Helper.STATUS_CREATED;
		string nextStatusKey = Helper.STATUS_PENDINGREVIEW;
		List<BankReconciliationReport> items = await bankReconciliationReportDL.RFC_BR_SelectBankReconciliationReports_Status(initialStatusKey);
		if (items.Count == 0)
			return;
		foreach (BankReconciliationReport item in items)
		{
			try
			{
				BankReconciliationReport item2 = CloneBRR(item);
				item.StatusKey = initialStatusKey;
				item.ApprovalNotification = false;
				item.BankKey = await bankFromBankSapDL.SelectBankFromBankSap(item.BankShortKey, item.SocietyKey);
				await bankReconciliationReportDL.InsertReport(item);
				await bankReconciliationReportDL.RFC_BR_UpdateBankReconciliationReport_Status(item2, nextStatusKey);
			}
			catch (Exception ex)
			{
				ilogger.LogError(ex, "message: {Message}", ex.Message);
				Console.WriteLine(ex.Message);
			}
		}
	}

	public static BankReconciliationReport CloneBRR(BankReconciliationReport item)
	{
		BankReconciliationReport itemClone = new()
		{
			AccountDate = item.AccountDate,
			StatusKey = item.StatusKey
		};
		return itemClone;
	}
}
```

### Complete GlobalUsings.cs Example
```csharp
global using Data;
global using Models;
global using Microsoft.Extensions.Options;
global using System.Text;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Serilog;
global using Serilog.Events;
global using Serilog.Core;
```

---

## Verification Checklist

Before committing Tasks project code, verify:

- [ ] File uses file-scoped namespace (`namespace Tasks;`)
- [ ] Class name follows Verb+Noun pattern (e.g., `ImportWorkflows`)
- [ ] Primary constructor used for DI
- [ ] Logger is first parameter: `ILogger<ClassName> ilogger`
- [ ] Task class has `Init()` or `Init(string[] args)` method
- [ ] All methods are `async` where appropriate
- [ ] No `var` used - all types are explicit
- [ ] Indentation uses tabs, not spaces
- [ ] No local `using` statements - all in `GlobalUsings.cs`
- [ ] Error handling in loops allows processing to continue
- [ ] Errors logged with `LogError` AND output to `Console.WriteLine`
- [ ] Structured logging used (no string interpolation)
- [ ] Task class registered as Singleton in Program.cs
- [ ] Command string added to Program.cs for routing
- [ ] CronJobs.txt updated with schedule (if applicable)

---

## Related Documentation
- [AI Coding Guide](../copilot-instructions.md)
- [C# Development Guidelines](./csharp.instructions.md)
- [Data Project Rules](./data.instructions.md)
- [Models Project Rules and Standards](./models.instructions.md)