using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.ProfileItems;

[Authorize]
public class EditPageModel(
    ILogger<EditPageModel> logger,
    IProfileItemRepository profileItemRepository)
    : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading profile items edit page at {DateLoaded}", DateTime.Now);
        CurrentProfileItem = await profileItemRepository.DetailsAsync(Id);
        logger.LogInformation("Profile item edit {Name} loaded", CurrentProfileItem.Name);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogInformation("Saving profile item with id {ProfileItemId}", Id);
        var form = await Request.ReadFormAsync();
        try
        {
            await profileItemRepository.UpdateAsync(CurrentProfileItem);
            logger.LogInformation("profile item  {Name} has been updated", CurrentProfileItem.Name);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return RedirectToPage("/ProfileItems/Edit/{id}", new { Id });
        }

        return RedirectToPage("/ProfileItems/Index");
    }

    [BindProperty(SupportsGet = true)] public int Id { get; set; }
    [BindProperty] public ProfileItem CurrentProfileItem { get; set; }
}