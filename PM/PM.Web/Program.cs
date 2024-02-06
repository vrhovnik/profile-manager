using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using PM.Web.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<AzureAdOptions>()
    .Bind(builder.Configuration.GetSection(BaseOptions.AzureAdSectionName))
    .ValidateDataAnnotations();
builder.Services.AddOptions<AppOptions>()
    .Bind(builder.Configuration.GetSection(BaseOptions.AppSectionName))
    .ValidateDataAnnotations();
builder.Services.AddOptions<DataOptions>()
    .Bind(builder.Configuration.GetSection(BaseOptions.DataSectionName))
    .ValidateDataAnnotations();

builder.Services.AddHealthChecks();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection(BaseOptions.AzureAdSectionName));

builder.Services.AddControllers();
builder.Services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
        options.Conventions.AddPageRoute("/Info/Index", ""))
    .AddMvcOptions(options =>
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    }).AddMicrosoftIdentityUI();
var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }).AllowAnonymous();
    endpoints.MapRazorPages();
    endpoints.MapControllers();
});
app.Run();