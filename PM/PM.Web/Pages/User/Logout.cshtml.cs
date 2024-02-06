using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PM.Web.Pages.User;

public class LogoutPageModel(ILogger<LogoutPageModel> logger) : PageModel
{
    public void OnGet() => logger.LogInformation("User logged out page loaded at {DateLoaded}", DateTime.Now);
}