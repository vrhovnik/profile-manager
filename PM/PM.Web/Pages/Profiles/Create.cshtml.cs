using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PM.Web.Pages.Profiles;

public class CreatePageModel(ILogger<CreatePageModel> logger) : PageModel
{
    public void OnGet() => logger.LogInformation("Create profile page called at {DateLoaded}", DateTime.Now);
}