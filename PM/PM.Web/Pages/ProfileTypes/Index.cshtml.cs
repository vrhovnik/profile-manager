using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.ProfileTypes;

public class IndexPageModel(
    ILogger<IndexPageModel> logger,
    IProfileTypeRepository profileTypeRepository,
    IOptions<DataOptions> dataOptions)
    : PageModel
{
    public async Task<IActionResult> OnGetAsync(int? pageNumber)
    {
        logger.LogInformation("Loading index page for profile types {DateCreated}", DateTime.Now);
        var page = pageNumber ?? 1;
        logger.LogInformation("Loaded profile types page with query - {Query}", Query);
        ProfileTypes = await profileTypeRepository.SearchAsync(page, dataOptions.Value.PagingSize, Query);
        logger.LogInformation("Loaded {ProfileTypesCount} items", ProfileTypes.Count);

        if (!Request.IsHtmx()) return Page();

        return Partial("_ProfileTypesList", ProfileTypes);
    }

    [BindProperty(SupportsGet = true)] public string Query { get; set; }
    [BindProperty] public PaginatedList<ProfileType> ProfileTypes { get; set; }
}