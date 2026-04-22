WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTelerikBlazor();

builder.Services.AddHttpClient("AcademicPlannerApi", client =>
	client.BaseAddress = new Uri(builder.Configuration["ApiUrl"] ?? "https://localhost:7110/"));

builder.Services.AddScoped<SubjectService>();
builder.Services.AddScoped<TopicService>();
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<StudyEventService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<StudyResourceService>();
builder.Services.AddScoped<FlashcardService>();

await builder.Build().RunAsync();
