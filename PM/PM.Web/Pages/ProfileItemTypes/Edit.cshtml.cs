using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.ProfileItemTypes;

[Authorize]
public class EditPageModel(
    ILogger<EditPageModel> logger,
    IProfileItemTypesRepository profileItemTypesRepository)
    : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading ProfileItemTypes edit page at {DateLoaded}", DateTime.Now);
        CurrentProfileItemType = await profileItemTypesRepository.DetailsAsync(Id);
        logger.LogInformation("ProfileItemTypes {Name} loaded", CurrentProfileItemType.Name);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogInformation("Saving ProfileTypes with id {ProfileTypeId}", Id);
        var form = await Request.ReadFormAsync();
        try
        {
            await profileItemTypesRepository.UpdateAsync(CurrentProfileItemType);
            logger.LogInformation("ProfileItemTypes {Name} has been updated", CurrentProfileItemType.Name);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return RedirectToPage("/ProfileItemTypes/Edit/{id}", new { Id });
        }

        return RedirectToPage("/ProfileItemTypes/Index");
    }

    [BindProperty(SupportsGet = true)] public int Id { get; set; }
    [BindProperty] public ProfileItemType CurrentProfileItemType { get; set; }
}