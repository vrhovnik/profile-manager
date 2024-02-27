using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.User;

[Authorize]
public class SettingsPageModel(ISettingsService settingsService, ILogger<SettingsPageModel> logger) : PageModel
{
    public async Task OnGetAsync()
    {
        var uniqueUserId = User.Identity.Name!.GetUniqueValue();
        logger.LogInformation("Loading user settings page at {DateCreated}", DateTime.Now);
        UserSettings = await settingsService.GetAsync(uniqueUserId);
        logger.LogInformation("User settings loaded at {DateCreated} with {Id}", DateTime.Now,
            UserSettings.SettingsId);
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        var uniqueUserId = User.Identity.Name!.GetUniqueValue();
        var settingsId = uniqueUserId.GetUniqueValue();
        logger.LogInformation("Saving data to storage for user {Id} with settings id {SettingsId}", settingsId, 
            settingsId);
        UserSettings.SettingsId = settingsId;
        await settingsService.UpdateAsync(UserSettings);
        logger.LogInformation("Data saved to storage for user {Id} with {SettingsId} identification", settingsId,
            settingsId);
        return RedirectToPage("/User/Settings");
    }

    [BindProperty] public Settings UserSettings { get; set; }
}