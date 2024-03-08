using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.Profiles;

public class ItemsPageModel(ILogger<ItemsPageModel> logger, IProfileRepository profileRepository) : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading profiles items page at {DateLoaded}", DateTime.Now);
        var profile = await profileRepository.DetailsAsync(ProfileId);
        logger.LogInformation("Profile {Name} loaded successfully. Continuing loading items", profile.Name);
        CurrentProfile = profile;
    }

    [BindProperty(SupportsGet = true)] public int ProfileId { get; set; }
    [BindProperty] public Profile CurrentProfile { get; set; }
}