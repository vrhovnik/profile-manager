using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using PM.Core;
using PM.Data.SQL;
using PM.Interfaces;
using PM.Storage.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<DataOptions>()
    .Bind(builder.Configuration.GetSection(OptionNames.DataOptionsName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<StorageOptions>()
    .Bind(builder.Configuration.GetSection(OptionNames.StorageOptionsName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<AzureAdOptions>()
    .Bind(builder.Configuration.GetSection(OptionNames.AzureAdOptionsName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHealthChecks();
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
    options.Conventions.AddPageRoute("/Info/Index", ""));

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options => { options.FallbackPolicy = options.DefaultPolicy; });
builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

var storageOptions = builder.Configuration.GetSection(OptionNames.StorageOptionsName).Get<StorageOptions>();
builder.Services.AddScoped<ISettingsService, StorageSettingsService>(_ =>
    new StorageSettingsService(storageOptions.SettingsContainer, storageOptions.ConnectionString));

var dataOptions = builder.Configuration.GetSection(OptionNames.DataOptionsName).Get<DataOptions>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(_ =>
    new CategoryRepository(dataOptions.ConnectionString));

var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Info/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/" + RouteHelper.HealthRoute, new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }).AllowAnonymous();
    endpoints.MapRazorPages();
    endpoints.MapControllers();
});

app.Run();