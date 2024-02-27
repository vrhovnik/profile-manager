using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;

namespace PM.Web.Pages.ProfileTypes;

[Authorize]
public class DeletePageModel(
    ILogger<DeletePageModel> logger,
    IProfileTypeRepository profileTypeRepository)
    : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading profile type  with {CityId}", Id);
        CurrentProfileType = await profileTypeRepository.DetailsAsync(Id);
        logger.LogInformation("Loaded profile type {Name}", CurrentProfileType.Name);
    }

    public async Task<RedirectToPageResult> OnPostAsync()
    {
        try
        {
            logger.LogInformation("Deleting profile type with id {Id}", Id);
            await profileTypeRepository.DeleteAsync(Id);
            logger.LogInformation("Profile type with {Id} deleted", Id);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return RedirectToPage("/ProfileTypes/Delete/{Id}", new { Id });
        }

        return RedirectToPage("/ProfileTypes/Index");
    }

    [BindProperty(SupportsGet = true)] public string Id { get; set; }
    [BindProperty] public Models.ProfileType CurrentProfileType { get; set; }
}