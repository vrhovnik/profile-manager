using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.ProfileItems;

[Authorize]
public class CreatePageModel(
    ILogger<CreatePageModel> logger,
    IProfileItemRepository profileItemRepository,
    IProfileItemTypesRepository profileItemTypesRepository) : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading profile items create page at {DateLoaded}. Continuing loading item types",
            DateTime.Now);
        ItemTypes = await profileItemTypesRepository.GetAsync();
        logger.LogInformation("Item types loaded successfully. {Count} items loaded", ItemTypes.Count);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogInformation("Reading entered form data at {DateLoaded}", DateTime.Now);
        var form = await Request.ReadFormAsync();

        try
        {
            var formItemTypeId = int.Parse(form["ddlItemTypes"]);
            logger.LogInformation("Selected item type id is {ItemTypeId}", formItemTypeId);
            var profileItemType = new ProfileItemType { ProfileItemTypeId = formItemTypeId };
            ProfileItem.ItemType = profileItemType;
            await profileItemRepository.InsertAsync(ProfileItem);
            logger.LogInformation("Profile Item with {Name} has been saved", ProfileItem.Name);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return Page();
        }

        return RedirectToPage("/ProfileItems/Index");
    }

    [BindProperty] public ProfileItem ProfileItem { get; set; }
    [BindProperty] public List<ProfileItemType> ItemTypes { get; set; }
}