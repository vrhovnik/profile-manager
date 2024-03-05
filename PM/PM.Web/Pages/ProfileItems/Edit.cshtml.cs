using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.ProfileItems;

[Authorize]
public class EditPageModel(
    ILogger<EditPageModel> logger,
    IProfileItemRepository profileItemRepository,
    IProfileItemTypesRepository profileItemTypesRepository)
    : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading profile items edit page at {DateLoaded}", DateTime.Now);
        CurrentProfileItem = await profileItemRepository.DetailsAsync(Id);
        logger.LogInformation("Profile item edit {Name} loaded", CurrentProfileItem.Name);
        ItemTypes = await profileItemTypesRepository.GetAsync();
        logger.LogInformation("Item types loaded successfully. {Count} items loaded", ItemTypes.Count);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogInformation("Saving profile item with id {ProfileItemId}", Id);
        var form = await Request.ReadFormAsync();
        try
        {
            var formItemTypeId = int.Parse(form["ddlItemTypes"]);
            logger.LogInformation("Selected item type id is {ItemTypeId}", formItemTypeId);
            var profileItemType = new ProfileItemType { ProfileItemTypeId = formItemTypeId };
            CurrentProfileItem.ItemType = profileItemType;
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
    [BindProperty] public List<ProfileItemType> ItemTypes { get; set; }
}