using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.ProfileTypes;

[Authorize]
public class EditPageModel(
    ILogger<EditPageModel> logger,
    IProfileTypeRepository profileTypeRepository)
    : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading ProfileTypes edit page at {DateLoaded}", DateTime.Now);
        CurrentProfileType = await profileTypeRepository.DetailsAsync(Id);
        logger.LogInformation("ProfileTypes {Name} loaded", CurrentProfileType.Name);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogInformation("Saving ProfileTypes with id {ProfileTypeId}", Id);
        var form = await Request.ReadFormAsync();
        try
        {
            await profileTypeRepository.UpdateAsync(CurrentProfileType);
            logger.LogInformation("ProfileTypes {Name} has been updated", CurrentProfileType.Name);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return RedirectToPage("/ProfileTypes/Edit/{id}", new { Id });
        }

        return RedirectToPage("/ProfileTypes/Index");
    }

    [BindProperty(SupportsGet = true)] public string Id { get; set; }
    [BindProperty] public ProfileType CurrentProfileType { get; set; }
}