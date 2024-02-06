using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PM.Web.Pages.Info;

public class PrivacyModel(ILogger<PrivacyModel> logger) : PageModel
{
    public void OnGet() => logger.LogInformation("Loaded Privacy page at {DateLoaded}", DateTime.Now);
}