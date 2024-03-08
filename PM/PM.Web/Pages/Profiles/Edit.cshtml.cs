using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.Profiles;

[Authorize]
public class EditPageModel(
    ILogger<EditPageModel> logger,
    IProfileRepository profileRepository,
    IProfileTypeRepository profileTypesRepository,
    ICategoryRepository categoryRepository)
    : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading profiles edit page at {DateLoaded}", DateTime.Now);
        CurrentProfile = await profileRepository.DetailsAsync(Id);
        logger.LogInformation("Profile edit {Name} loaded. Loading profile types", CurrentProfile.Name);
        ProfileTypes = await profileTypesRepository.GetAsync();
        logger.LogInformation("Profile types loaded successfully. {Count} items loaded. Loading categories", ProfileTypes.Count);
        Categories = await categoryRepository.GetAsync();
        logger.LogInformation("Categories loaded successfully. {Count} items loaded", Categories.Count);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogInformation("Saving profile with id {ProfileItemId}", Id);
        var form = await Request.ReadFormAsync();
        try
        {
            var formItemTypeId = int.Parse(form["ddlItemTypes"]);
            logger.LogInformation("Selected profile type id is {ItemTypeId}", formItemTypeId);
            var profileItemType = new ProfileType { ProfileTypeId = formItemTypeId };
            CurrentProfile.Type = profileItemType;
            await profileRepository.UpdateAsync(CurrentProfile);
            logger.LogInformation("profile item  {Name} has been updated", CurrentProfile.Name);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return RedirectToPage("/ProfileItems/Edit/{id}", new { Id });
        }

        return RedirectToPage("/ProfileItems/Index");
    }

    [BindProperty(SupportsGet = true)] public int Id { get; set; }
    [BindProperty] public Profile CurrentProfile { get; set; }
    [BindProperty] public List<ProfileType> ProfileTypes { get; set; }
    [BindProperty] public List<Category> Categories { get; set; }
}