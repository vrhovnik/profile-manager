using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.ProfileTypes;

[Authorize]
public class CreatePageModel(ILogger<CreatePageModel> logger, IProfileTypeRepository profileTypeRepository)
    : PageModel
{
    public void OnGetAsync() => logger.LogInformation("Loading ProfileTypes create page at {DateLoaded}", DateTime.Now);

    public async Task<IActionResult> OnPostAsync()
    {
        var form = await Request.ReadFormAsync();

        try
        {
            await profileTypeRepository.InsertAsync(ProfileType);
            logger.LogInformation("ProfileTypes with {Name} has been saved", ProfileType.Name);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return Page();
        }

        return RedirectToPage("/Categories/Index");
    }

    [BindProperty] public ProfileType ProfileType { get; set; }
}