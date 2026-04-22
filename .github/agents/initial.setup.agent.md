## 1. Solution Automation (CLI Skills)
The agent has an ability to execute CLI commands to create a complete ecosystem structure and create files and put its contents in each file.

## 2. Global Standards (The House Way)
* Namespaces: Siempre usar file-scoped namespaces (ej. namespace Data;).
* Constructores: Uso obligatorio de Primary Constructors para Inyección de Dependencias.

## 3. Master Code Templates and Mandatory Routes 

### A. Evironment Configuration (Infrastructure)
The Agent will assured the creation of this files in its exact routes to enable the Web SDK and OpenAPI:

#### [PROJECT: Api]
* OpenAPI: Execute 'dotnet add Api/Api.csproj package Swashbuckle.AspNetCore --version 10.0.0'.
* Route: Api/Properties/launchSettings.json
    {
        "$schema": "https://json.schemastore.org/launchsettings.json",
        "profiles": {
            "Api": {
                "commandName": "Project",
                "dotnetRunMessages": true,
                "launchBrowser": true,
                "launchUrl": "swagger",
                "applicationUrl": "https://localhost:44332",
                "environmentVariables": { "ASPNETCORE_ENVIRONMENT": "Development" }
            }
        }
    }
* Route: Api/appsettings.json
    {
        "ConnectionStrings": {
            "APPNAME": "server=tcp:MSAZDEVWEBSQL19.lcred.org;database=MRC.DTI.Administration.DBNAME;uid=saAPPUSERNAME;pwd=PASSWORD;TrustServerCertificate=true;"
        },
        "AllowedHosts": "*",
        "MRCAppSettings": {
            "ApplicationKey": "MRC.AG.Administration.APPNAME",
            "ClientKey": "AG",
            "Environment": "DEV",
            "Location": "USA",
            "LogUrl": "http://localhost:44500",
            "UtilitiesApi": "https://devdti.lcred.net/utilities/api/",
            "UtilitiesApiNF": "https://devdti.lcred.net/utilities/apinf/",
            "JwtKey": "PONER_JWT_KEY"
        }
    }
* Route: Api/GlobalUsings.cs
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
    global using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
    global using Telerik.Windows.Documents.Spreadsheet.Model;
    global using Telerik.Windows.Documents.Spreadsheet.PropertySystem;
* Route: Api/HelperApi.cs 
    namespace Api;
    public static class HelperApi
    {
        public static byte[] GetBytes(IFormFile formFile)
        {
            if (formFile == null)
                return null;
            using (var memoryStream = new MemoryStream())
            {
                formFile.OpenReadStream().CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
* Route: Api/Program.cs 
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    IConfigurationSection mrcAppSettingsSection = builder.Configuration.GetSection("MRCAppSettings");
    MRCAppSettings mrcAppSettings = mrcAppSettingsSection.Get<MRCAppSettings>();

    builder.Services.Configure<MRCAppSettings>(mrcAppSettingsSection);
    builder.Services.AddControllers().AddJsonOptions(x => { x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; x.JsonSerializerOptions.PropertyNameCaseInsensitive = true; });
    builder.Services.AddDbContext<MRCDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("")));
    builder.Services.AddCors(options => { options.AddDefaultPolicy(policy => { policy.WithOrigins(Utilities.LocalhostWebUrl).AllowAnyHeader().AllowAnyMethod(); }); });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpClient(Utilities.HttpClientNameUtilities, client => client.BaseAddress = new Uri(mrcAppSettings.UtilitiesApi));
    builder.Services.AddHttpClient(Utilities.HttpClientNameUtilitiesNF, client => client.BaseAddress = new Uri(mrcAppSettings.UtilitiesApiNF));
    builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; });
    builder.Services.AddScoped<UserDL>();
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
    Serilog.Core.Logger logger = new LoggerConfiguration().WriteTo.Seq(mrcAppSettings.LogUrl, LogEventLevel.Information).Enrich.WithProperty("ClientKey", mrcAppSettings.ClientKey).Enrich.WithProperty("ApplicationKey", mrcAppSettings.ApplicationKey).Enrich.WithProperty("Environment", mrcAppSettings.Environment).Enrich.WithProperty("Location", mrcAppSettings.Location).CreateLogger();
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog(logger);
    WebApplication app = builder.Build();
    app.UseMiddleware<CustomExceptionHandlingMiddleware>();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
* Route: Api/web.config
    ```xml
    <?xml version="1.0" encoding="utf-8"?>
    <configuration>
        <location path="." inheritInChildApplications="false">
            <system.web>
                <identity impersonate="false" />
            </system.web>
            <system.webServer>
                <security>
                    <requestFiltering>
                        <requestLimits maxAllowedContentLength="52428800"></requestLimits>
                    </requestFiltering>
                </security>
                <modules runAllManagedModulesForAllRequests="false">
                    <remove name="WebDAVModule" />
                </modules>
            </system.webServer>
        </location>
    </configuration>
    ```
* Route: Api/Middleware/CustomExceptionHandlingMiddleware.cs
    ```c#
    namespace Api.Middleware;
    public class CustomExceptionHandlingMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlingMiddleware> logger)
    {
        private const int MaxMessageLength = 255;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleGlobalExceptionAsync(context, ex);
            }
        }
        private async Task HandleGlobalExceptionAsync(HttpContext context, Exception ex)
        {
            string requestId = context.TraceIdentifier;
            string message = ex.Message;
            if (message.Length > MaxMessageLength)
                message = $"{message[..MaxMessageLength]}...";
            if (ex is ApplicationException)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                requestId = string.Empty;
            }
            else
            {
                logger.LogError(ex, "{Message} (requestId: {RequestId})", ex.Message, requestId);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            await context.Response.WriteAsJsonAsync(new { Id = requestId, Message = message });
        }
    }
    ```
* Add reference to projects in route: Api/Api.csproj
    ```xml
    <ItemGroup>
		<ProjectReference Include="..\Data\Data.csproj" />
		<ProjectReference Include="..\Models\Models.csproj" />
	</ItemGroup>
    ```
#### [PROJECT: ApiClient]
* Route: ApiClient/GlobalUsings.cs
    global using Models;
    global using MRC.DTI.Dev.Utilities.Business;
    global using System.Net;
    global using System.Net.Http.Headers;
    global using System.Text;
    global using System.Text.Json;
    global using System.Net.Http.Json;
    global using System.IdentityModel.Tokens.Jwt;
* Route: ApiClient/HelperApiClient.cs
    namespace ApiClient;
    public static class HelperApiClient
    {
    }
* Route: ApiClient/LoginApiHandler.cs
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
* Route: ApiClient/LoginApiService.cs
    ```c#
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
                if (!string.IsNullOrWhiteSpace(token) && !IsTokenExpired(token))
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
            string body = JsonSerializer.Serialize(new User { UserPrincipalName = userPrincipalName, Name = "NULL", LastName = "NULL", MaternalSurname = "NULL", RoleKey = "NULL" });
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

        private bool IsTokenExpired(string token)
        {
            JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;
            string userId = jwtToken?.Claims.FirstOrDefault(x => x.Type == "Sid")?.Value;
            if (userId is null || userId == "0")
                return true;
            if (jwtToken.ValidTo < DateTime.UtcNow)
                return true;
            return false;
        }
    }
    ```
* Route: ApiClient/TransactionTypeService.cs
    ```c#
    namespace ApiClient;

    public class TransactionTypeService(IHttpClientFactory httpClientFactory)
    {
        private readonly string apiName = "TransactionType";

        public async Task<TransactionType> SelectTransactionType(int id)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectTransactionType?id={id}");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.NoContent)
                return null;
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<TransactionType>(result, Helper.JsonSerializerOptions);
        }

        public async Task<List<TransactionType>> SelectTransactionTypes()
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectTransactionTypes");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<TransactionType>>(result, Helper.JsonSerializerOptions);
        }

        public async Task<List<TransactionTypeGrid>> SelectTransactionTypesGrid()
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectTransactionTypesGrid");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<TransactionTypeGrid>>(result, Helper.JsonSerializerOptions);
        }

        public async Task<bool> TransactionTypeIsAssignedToAssetDocuments(int transactionTypeId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/TransactionTypeIsAssignedToAssetDocuments?transactionTypeId={transactionTypeId}");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<bool>(result, Helper.JsonSerializerOptions);
        }

        public async Task<TransactionType> InsertTransactionType(TransactionType transactionType)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{apiName}/InsertTransactionType", transactionType);
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<TransactionType>(result, Helper.JsonSerializerOptions);
        }

        public async Task<TransactionType> UpdateTransactionType(TransactionType transactionType)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{apiName}/UpdateTransactionType", transactionType);
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<TransactionType>(result, Helper.JsonSerializerOptions);
        }

        public async Task<int> DeleteTransactionType(int id)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.DeleteAsync($"{apiName}/DeleteTransactionType?id={id}");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<int>(result, Helper.JsonSerializerOptions);
        }
    }
    ```
* Route: ApiClient/UserService.cs
    ```c#
    namespace ApiClient;

    public class UserService(IHttpClientFactory httpClientFactory)
    {
        private readonly string apiName = "User";

        public async Task<User> SelectUser(int id)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectUser?id={id}");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.NoContent)
                return null;
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<User>(result, Helper.JsonSerializerOptions);
        }

        public async Task<User> SelectUser_UserPrincipalName(string userPrincipalName)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectUser_UserPrincipalName?userPrincipalName={userPrincipalName}");
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized) // Validación para el unauthorized que se regresa en la consulta del login
                    return null;
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            }
            if (response.StatusCode == HttpStatusCode.NoContent)
                return null;
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<User>(result, Helper.JsonSerializerOptions);
        }

        public async Task<User> SelectUserAD_UserPrincipalName(string userPrincipalName)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectUserAD_UserPrincipalName?userPrincipalName={userPrincipalName}");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.NoContent)
                return null;
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<User>(result, Helper.JsonSerializerOptions);
        }

        public async Task<List<User>> SelectUsers_Group(int groupId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectUsers_Group?groupId={groupId}");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<User>>(result, Helper.JsonSerializerOptions);
        }

        public async Task<List<User>> SelectUsers_Type(string userTypeKey)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectUsers_Type?userTypeKey={userTypeKey}");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<User>>(result, Helper.JsonSerializerOptions);
        }

        public async Task<List<UserGrid>> SelectUsersGrid()
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectUsersGrid");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<List<UserGrid>>(result, Helper.JsonSerializerOptions);
        }

        public async Task<User> InsertUser(User item)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            StringContent content = new(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync($"{apiName}/InsertUser", content);
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.NoContent)
                return null;
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<User>(result, Helper.JsonSerializerOptions);
        }

        public async Task<User> UpdateUser(User item)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            StringContent content = new(JsonSerializer.Serialize(item), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PutAsync($"{apiName}/UpdateUser", content);
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.NoContent)
                return null;
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<User>(result, Helper.JsonSerializerOptions);
        }

        public async Task<int> UpdateUser_DefaultGroup(int userId, int groupId)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/UpdateUser_DefaultGroup?userId={userId}&groupId={groupId}");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.NoContent)
                return 0;
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<int>(result, Helper.JsonSerializerOptions);
        }

        public async Task<int> DeleteUser(int id)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientName);
            HttpResponseMessage response = await httpClient.DeleteAsync($"{apiName}/DeleteUser?id={id}");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.NoContent)
                return 0;
            Stream result = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<int>(result, Helper.JsonSerializerOptions);
        }
    }
    ```
* Add reference to projects in route: ApiClient/ApiClient.csproj
    ```xml
    <ItemGroup>
		<ProjectReference Include="..\Models\Models.csproj" />
	</ItemGroup>
    ```
#### [PROJECT: Web]
* Route: Web/Properties/launchSettings.json
    {
        "profiles": {
            "Web": {
                "commandName": "Project",
                "launchBrowser": true,
                "inspectUri": "{wsProtocol}://{url.hostname}:{url.port}/_framework/debug/ws-proxy?browser={browserInspectUri}",
                "environmentVariables": { "ASPNETCORE_ENVIRONMENT": "Development" },
                "applicationUrl": "https://localhost:44331"
            }
        }
    } 
* Route: Web/appsettings.json
    {
        "AzureAd": {
            "Authority": "https://login.microsoftonline.com/cf1edb36-330e-4e4a-977b-7aab1eeefb86",
            "ClientId": "beea1ab1-c819-4b39-b311-1ff0279c23db",
            "ValidateAuthority": true
        }
    }
* Route: Web/GlobalUsings.cs
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
    global using Telerik.Blazor.Components.Map;
    global using System.Text.RegularExpressions;
* Route: Web/GlobalUsings.cs
    ```c#
    namespace Web;
    public static class HelperWeb
    {
    }
    ```
* Route: Web/Program.cs
    ```c#
    WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
    string apiUrl = Utilities.GetApiUrl_BlazorWebProject(builder.HostEnvironment.BaseAddress);
    ConfigurationManager configurationManager = new();
    configurationManager.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

    builder.RootComponents.Add<App>("#app");
    builder.Services.AddTelerikBlazor();

    builder.Services.AddSingleton<TokenApiStateProvider>();
    builder.Services.AddScoped<ILoginApiService, LoginApiService>();
    builder.Services.AddScoped<LoginApiHandler>();
    builder.Services.AddHttpClient(Utilities.HttpClientNameAnonymous, client => client.BaseAddress = new Uri(apiUrl));
    builder.Services.AddHttpClient(Utilities.HttpClientName, client => client.BaseAddress = new Uri(apiUrl)).AddHttpMessageHandler<LoginApiHandler>();

    builder.Services.AddScoped<UserService>();

    builder.Services.AddSingleton(typeof(ITelerikStringLocalizer), typeof(ResxLocalizer));
    Utilities.SetCultureAsync("es-MX");
    builder.Services.AddMsalAuthentication<RemoteAuthenticationState, CustomUserAccount>(options =>
    {
        builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
        options.ProviderOptions.LoginMode = "redirect";
        options.ProviderOptions.DefaultAccessTokenScopes.Add("https://graph.microsoft.com/User.Read");
    }).AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, CustomUserAccount, CustomUserFactory>();
    builder.Services.AddAuthorizationCore(options => { options.AddPolicy("UserRequired", policy => { policy.RequireClaim("userId"); }); });
    await builder.Build().RunAsync();
    ```
* Route: Web/web.config
    ```xml
    <?xml version="1.0" encoding="UTF-8"?>
    <configuration>
    <system.webServer>
        <staticContent>
        <remove fileExtension=".blat" />
        <remove fileExtension=".dat" />
        <remove fileExtension=".dll" />
        <remove fileExtension=".json" />
        <remove fileExtension=".wasm" />
        <remove fileExtension=".woff" />
        <remove fileExtension=".woff2" />
        <mimeMap fileExtension=".blat" mimeType="application/octet-stream" />
        <mimeMap fileExtension=".dll" mimeType="application/octet-stream" />
        <mimeMap fileExtension=".dat" mimeType="application/octet-stream" />
        <mimeMap fileExtension=".json" mimeType="application/json" />
        <mimeMap fileExtension=".wasm" mimeType="application/wasm" />
        <mimeMap fileExtension=".woff" mimeType="application/font-woff" />
        <mimeMap fileExtension=".woff2" mimeType="application/font-woff" />
        </staticContent>
        <httpCompression>
        <dynamicTypes>
            <add mimeType="application/octet-stream" enabled="true" />
            <add mimeType="application/wasm" enabled="true" />
        </dynamicTypes>
        </httpCompression>
        <rewrite>
        <rules>
            <rule name="Serve subdir">
            <match url=".*" />
            <action type="Rewrite" url="wwwroot\{R:0}" />
            </rule>
            <rule name="SPA fallback routing" stopProcessing="true">
            <match url=".*" />
            <conditions logicalGrouping="MatchAll">
                <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            </conditions>
            <action type="Rewrite" url="wwwroot\" />
            </rule>
        </rules>
        </rewrite>
    </system.webServer>
        <system.web>
            <identity impersonate="false" />
        </system.web>
    </configuration>
    ```
* Add reference to projects in route: Web/Web.csproj
    ```xml
    <ItemGroup>
		<ProjectReference Include="..\ApiClient\ApiClient.csproj" />
		<ProjectReference Include="..\Models\Models.csproj" />
	</ItemGroup>
    ```
#### [PROJECT: Data]
* Route: Data/GlobalUsings.cs
    El agente configurará este archivo en el proyecto Data para eliminar redundancia:
    global using System.Data;
    global using Models;
    global using Microsoft.EntityFrameworkCore;
    global using Microsoft.Data.SqlClient;
    global using System.Net;
    global using System.Net.Http;
    global using System.Text;
    global using System.Text.Json;
    global using MRC.DTI.Dev.Utilities.Business;
* Route: Data/MRCDBContext.cs
    namespace Data;
    public class MRCDBContext(DbContextOptions<MRCDBContext> options) : DbContext(options) {
        public DbContextOptions<MRCDBContext> Options => options;
        protected override void OnModelCreating(ModelBuilder modelBuilder) => base.OnModelCreating(modelBuilder);
    }
* Route: Data/HelperData.cs
    ```c#
    namespace Data;

    public static class HelperData
    {
    }

    public class HelperDataNS(MRCDBContext context)
    {
        public void UpdateRelatedEntities<T>(ICollection<T> existingEntities, ICollection<T> updatedEntities, DbSet<T> dbSet) where T : class
        {
            string primaryKey = Utilities.GetPrimaryKeyName<T>();
            List<object> existingIds = existingEntities.Select(e => context.Entry(e).Property(primaryKey).CurrentValue).ToList();
            List<object> updatedIds = updatedEntities.Select(e => context.Entry(e).Property(primaryKey).CurrentValue).ToList();

            foreach (T item in updatedEntities)
            {
                object itemId = context.Entry(item).Property(primaryKey).CurrentValue;
                if (!existingIds.Contains(itemId))
                {
                    dbSet.Attach(item);
                    existingEntities.Add(item);
                }
            }
            foreach (T item in existingEntities.ToList())
            {
                object itemId = context.Entry(item).Property(primaryKey).CurrentValue;
                if (!updatedIds.Contains(itemId))
                    existingEntities.Remove(item);
            }
        }
    }
    ```
* Route: Data/UserDL.cs
    ```c#
    namespace Data;

    public class UserDL(MRCDBContext context, IHttpClientFactory httpClientFactory, HelperDataNS helperDataNS)
    {
        public async Task<User> SelectUser(int id, bool asNoTracking = true)
        {
            IQueryable<User> query = context.Users.Where(x => x.Id == id).Include(x => x.Groups);
            if (asNoTracking)
                return await query.AsNoTracking().FirstOrDefaultAsync();
            return await query.FirstOrDefaultAsync();
        }

        public async Task<User> SelectUser_UserPrincipalName(string userPrincipalName)
        {
            User item = await context.Users.Where(x => x.UserPrincipalName == userPrincipalName).FirstOrDefaultAsync();
            return item;
        }

        public async Task<User> SelectUserAD_UserPrincipalName(string userPrincipalName)
        {
            HttpClient httpClient = httpClientFactory.CreateClient(Utilities.HttpClientNameUtilities);
            HttpResponseMessage response = await httpClient.GetAsync($"AzureAD/SelectUser_UserPrincipalName/{userPrincipalName}");
            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(await response.Content.ReadAsStringAsync());
            if (response.StatusCode == HttpStatusCode.NoContent)
                return null;
            Stream result = await response.Content.ReadAsStreamAsync();
            MrcUser mrcUser = await JsonSerializer.DeserializeAsync<MrcUser>(result, Helper.JsonSerializerOptions);
            User item = new() { Id = 0, UserPrincipalName = mrcUser.UserPrincipalName, Name = mrcUser.DisplayName };
            return item;
        }

        public async Task<List<User>> SelectUsers_Group(int groupId)
        {
            List<User> items = await context.Users.Where(u => u.Groups.Any(x => x.Id == groupId)).AsNoTracking().ToListAsync();
            return items;
        }

        public async Task<List<User>> SelectUsers_Type(string userTypeKey)
        {
            List<User> items = await context.Users
                .Where(x =>
                    (userTypeKey == Helper.USERTYPEKEY_FACILITATOR && x.IsFacilitator) ||
                    (userTypeKey == Helper.USERTYPEKEY_LEGALREPRESENTATIVE && x.IsLegalRepresentative)
                )
                .AsNoTracking()
                .ToListAsync();
            return items;
        }

        public async Task<List<UserGrid>> SelectUsersGrid()
        {
            List<UserGrid> items = await context.Database
                .SqlQueryRaw<UserGrid>("EXEC [dbo].[SelectUsersGrid]")
                .AsNoTracking()
                .ToListAsync();
            return items;
        }

        public async Task<User> InsertUser(User user)
        {
            AttachExistingRelations(user);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            User existingUser = await SelectUser(user.Id, asNoTracking: false);
            context.Entry(existingUser).CurrentValues.SetValues(user);
            helperDataNS.UpdateRelatedEntities(existingUser.Groups, user.Groups, context.Groups);
            await context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<int> UpdateUser_DefaultGroup(int userId, int groupId)
        {
            int result = await context.Users.Where(x => x.Id == userId).ExecuteUpdateAsync(x => x.SetProperty(u => u.GroupId, groupId));
            return result;
        }

        public async Task<int> DeleteUser(int id)
        {
            int result = await context.Users.Where(x => x.Id == id).ExecuteDeleteAsync();
            return result;
        }
        public async Task ValidateUserGroups(int userId, int groupId)
        {
            if (groupId == 0)
                return;
            User user = await SelectUser(userId);
            if (!user.Groups.Any(x => x.Id == groupId))
            {
                await UpdateUser_DefaultGroup(userId, 0);
                throw new Exception($"{Helper.RELOADSESSION}Se le han retirado los permisos al grupo seleccionado.");
            }
        }

        private void AttachExistingRelations(User user)
        {
            foreach (Group group in user.Groups)
                context.Groups.Attach(group);
        }
    }
    ```
* Add reference to projects in route: Data/Data.csproj
    ```xml
    <ItemGroup>
		<ProjectReference Include="..\Models\Models.csproj" />
	</ItemGroup>
    ```
### [PROJECT: Models]
* Route: Models/GlobalUsings.cs
    global using System.ComponentModel.DataAnnotations;
    global using System.ComponentModel.DataAnnotations.Schema;
    global using System.Text.Json;
* Route: Models/Helper.cs
    namespace Models;
    public class Helper
    {
        public static JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    }
* Route: Models/MRCAppSettings.cs
    ```c#
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
        public string JwtKey { get; set; }
        public string ApiUrl { get; set; }
        public string TestUserPrincipalName { get; set; }
    }
    ```
* Route: Models/User.cs
    ```c#
    namespace Models;

    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "- El campo Clave de usuario es requerido"), MaxLength(128)]
        [EmailAddress(ErrorMessage = "- El campo Clave de usuario no tiene el formato correcto (usuario@dominio.org)")]
        public string UserPrincipalName { get; set; }
        [Required(ErrorMessage = "- El campo Nombre es requerido"), MaxLength(100)]
        public string Name { get; set; }
        [Required(ErrorMessage = "- El campo Apellido paterno es requerido"), MaxLength(50)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "- El campo Apellido materno es requerido"), MaxLength(50)]
        public string MaternalSurname { get; set; }
        [MaxLength(128)]
        public string Email { get; set; }
        [Required(ErrorMessage = "- El campo Rol es requerido"), MaxLength(50)]
        public string RoleKey { get; set; }
        public int GroupId { get; set; }
        public Role Role { get; set; }
    }

    public class UserBasic
    {
        public int Id { get; set; }
        public string UserPrincipalName { get; set; }
        public string DisplayName { get; set; }
    }

    public class UserGrid
    {
        public int Id { get; set; }
        public string UserPrincipalName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
    }
    ```