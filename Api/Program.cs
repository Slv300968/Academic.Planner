WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(x =>
{
x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddDbContext<AcademicPlannerDBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("AcademicPlanner")));
builder.Services.AddCors(options =>
{
options.AddDefaultPolicy(policy =>
{
policy.WithOrigins("https://localhost:7173").AllowAnyHeader().AllowAnyMethod();
});
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<SubjectDL>();
builder.Services.AddScoped<TopicDL>();
builder.Services.AddScoped<QuestionDL>();
builder.Services.AddScoped<StudyEventDL>();
builder.Services.AddScoped<DashboardDL>();
	builder.Services.AddScoped<StudyResourceDL>();
	builder.Services.AddScoped<FlashcardDL>();

WebApplication app = builder.Build();

app.UseMiddleware<CustomExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

using (IServiceScope scope = app.Services.CreateScope())
{
AcademicPlannerDBContext dbContext = scope.ServiceProvider.GetRequiredService<AcademicPlannerDBContext>();
dbContext.Database.EnsureCreated();
await SeedData.SeedAsync(dbContext);
}

app.Run();
