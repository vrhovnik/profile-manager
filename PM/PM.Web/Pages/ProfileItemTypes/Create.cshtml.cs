using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.ProfileItemTypes;

[Authorize]
public class CreatePageModel(ILogger<CreatePageModel> logger, IProfileItemTypesRepository profileItemTypesRepository)
    : PageModel
{
    public void OnGetAsync() => logger.LogInformation("Loading ProfileItemTypes create page at {DateLoaded}", DateTime.Now);

    public async Task<IActionResult> OnPostAsync()
    {
        var form = await Request.ReadFormAsync();

        try
        {
            await profileItemTypesRepository.InsertAsync(ProfileItemType);
            logger.LogInformation("ProfileTypes with {Name} has been saved", ProfileItemType.Name);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return Page();
        }

        return RedirectToPage("/ProfileItemTypes/Index");
    }

    [BindProperty] public ProfileItemType ProfileItemType { get; set; }
}