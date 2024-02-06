using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PM.Web.Pages.User;

[Authorize]
public class ProfilePageModel(ILogger<ProfilePageModel> logger) : PageModel
{
    public void OnGet()
    {
        logger.LogInformation("Profile page visited at {DateCreated}", DateTime.Now);
    }
}