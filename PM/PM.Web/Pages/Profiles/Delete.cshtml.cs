using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;

namespace PM.Web.Pages.Profiles;

[Authorize]
public class DeletePageModel(
    ILogger<DeletePageModel> logger,
    IProfileItemRepository profileItemRepository)
    : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading profile item with {ProfileItemId}", Id);
        CurrentProfileItem = await profileItemRepository.DetailsAsync(Id);
        logger.LogInformation("Loaded profile item {Name}", CurrentProfileItem.Name);
    }

    public async Task<RedirectToPageResult> OnPostAsync()
    {
        try
        {
            logger.LogInformation("Deleting profile item with id {Id}", Id);
            await profileItemRepository.DeleteAsync(Id);
            logger.LogInformation("Profile item with {Id} deleted", Id);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return RedirectToPage("/ProfileItems/Delete/{Id}", new { Id });
        }

        return RedirectToPage("/ProfileItems/Index");
    }

    [BindProperty(SupportsGet = true)] public int Id { get; set; }
    [BindProperty] public Models.ProfileItem CurrentProfileItem { get; set; }
}