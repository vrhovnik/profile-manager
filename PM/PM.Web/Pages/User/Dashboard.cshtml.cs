using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PM.Web.Pages.User;

[Authorize]
public class DashboardPageModel(ILogger<DashboardPageModel> logger) : PageModel
{
    public void OnGet() => logger.LogInformation("Loaded admin dashboard at {DateLoaded}", DateTime.Now);
}