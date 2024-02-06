using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PM.Web.Pages.User;

[Authorize]
public class DashboardPageModel(ILogger<DashboardPageModel> logger) : PageModel
{
    public void OnGet() => logger.LogInformation("Dashboard page visited at {DateCreated}", DateTime.Now);
}