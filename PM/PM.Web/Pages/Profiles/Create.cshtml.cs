using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.Profiles;

[Authorize]
public class CreatePageModel(
    ILogger<CreatePageModel> logger,
    IProfileRepository profileRepository,
    ICategoryRepository categoryRepository,
    IProfileTypeRepository profileTypeRepository) : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading profiles create page at {DateLoaded}. Continuing loading profile types",
            DateTime.Now);
        ProfileTypes = await profileTypeRepository.GetAsync();
        logger.LogInformation("Profile types loaded successfully. {Count} items loaded. Continuing loading categories.", ProfileTypes.Count);
        Categories = await categoryRepository.GetAsync();
        logger.LogInformation("Categories loaded successfully. {Count} items loaded", Categories.Count);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogInformation("Reading entered form data at {DateLoaded}", DateTime.Now);
        var form = await Request.ReadFormAsync();

        try
        {
            var formItemTypeId = int.Parse(form["ddlItemTypes"]);
            logger.LogInformation("Selected profile type is {ItemTypeId}", formItemTypeId);
            var profileItemType = new ProfileType { ProfileTypeId = formItemTypeId };
            ProfileItem.Type = profileItemType;
            await profileRepository.InsertAsync(ProfileItem);
            logger.LogInformation("Profile with {Name} has been saved. continuing to items", ProfileItem.Name);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return Page();
        }

        return RedirectToPage("/Profiles/Index");
    }

    [BindProperty] public Profile ProfileItem { get; set; }
    [BindProperty] public List<ProfileType> ProfileTypes { get; set; }
    [BindProperty] public List<Category> Categories { get; set; }
}