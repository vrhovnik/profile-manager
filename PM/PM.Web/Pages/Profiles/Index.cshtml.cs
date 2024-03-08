using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.Profiles;

public class IndexPageModel(
    ILogger<IndexPageModel> logger,
    IProfileRepository profileRepository,
    IOptions<DataOptions> dataOptions)
    : PageModel
{
    public async Task<IActionResult> OnGetAsync(int? pageNumber)
    {
        logger.LogInformation("Loading index page for profiles {DateCreated}", DateTime.Now);
        var page = pageNumber ?? 1;
        logger.LogInformation("Loaded profiles page with query - {Query}", Query);
        Profiles = await profileRepository.SearchAsync(page, dataOptions.Value.PagingSize, Query);
        logger.LogInformation("Loaded {ProfilesCount} profiles", Profiles.Count);

        if (!Request.IsHtmx()) return Page();

        return Partial("_ProfilesList", Profiles);
    }

    [BindProperty(SupportsGet = true)] public string Query { get; set; }
    [BindProperty] public PaginatedList<Profile> Profiles { get; set; }
}