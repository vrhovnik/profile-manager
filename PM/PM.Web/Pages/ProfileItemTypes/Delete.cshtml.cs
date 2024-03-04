using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;

namespace PM.Web.Pages.ProfileItemTypes;

[Authorize]
public class DeletePageModel(
    ILogger<DeletePageModel> logger,
    IProfileItemTypesRepository profileItemTypeRepository)
    : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading profile item type  with {ProfileItemType}", Id);
        CurrentProfileItemType = await profileItemTypeRepository.DetailsAsync(Id);
        logger.LogInformation("Loaded profile item type {Name}", CurrentProfileItemType.Name);
    }

    public async Task<RedirectToPageResult> OnPostAsync()
    {
        try
        {
            logger.LogInformation("Deleting profile item type with id {Id}", Id);
            await profileItemTypeRepository.DeleteAsync(Id);
            logger.LogInformation("Profile item type with {Id} deleted", Id);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return RedirectToPage("/ProfileItemTypes/Delete/{Id}", new { Id });
        }

        return RedirectToPage("/ProfileItemTypes/Index");
    }

    [BindProperty(SupportsGet = true)] public int Id { get; set; }
    [BindProperty] public Models.ProfileItemType CurrentProfileItemType { get; set; }
}