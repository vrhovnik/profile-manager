using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PM.Web.Pages.Info;
[AllowAnonymous]
public class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    public void OnGet() => logger.LogInformation("Index page visited at {DateLoaded}.", DateTime.Now);
}