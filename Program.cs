using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using StudentManagementSystem.Components;
using StudentManagementSystem.Features.Data;
using StudentManagementSystem.Features.Helpers;
using StudentManagementSystem.Features.Repositories.Implementations;
using StudentManagementSystem.Features.Repositories.Interfaces;
using StudentManagementSystem.Features.Services.Implementations;
using StudentManagementSystem.Features.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// EF Core MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var mySqlServerVersion = new MySqlServerVersion(new Version(8, 0, 36));

void ConfigureDatabase(DbContextOptionsBuilder options)
{
    options.UseMySql(connectionString, mySqlServerVersion);
}

builder.Services.AddDbContext<AppDbContext>(ConfigureDatabase);

// Repositories
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IUserService, UserService>();

// Authentication & Authorization
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());

// MudBlazor
builder.Services.AddMudServices();

// Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
