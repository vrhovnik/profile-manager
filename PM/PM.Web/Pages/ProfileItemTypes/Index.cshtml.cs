using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.ProfileItemTypes;

public class IndexPageModel(
    ILogger<IndexPageModel> logger,
    IProfileItemTypesRepository profileItemTypeRepository,
    IOptions<DataOptions> dataOptions)
    : PageModel
{
    public async Task<IActionResult> OnGetAsync(int? pageNumber)
    {
        logger.LogInformation("Loading index page for profile item types {DateCreated}", DateTime.Now);
        var page = pageNumber ?? 1;
        logger.LogInformation("Loaded profile types page with query - {Query}", Query);
        ProfileItemTypes = await profileItemTypeRepository.SearchAsync(page, dataOptions.Value.PagingSize, Query);
        logger.LogInformation("Loaded {ProfileTypesCount} profile item types", ProfileItemTypes.Count);

        if (!Request.IsHtmx()) return Page();

        return Partial("_ProfileItemTypesList", ProfileItemTypes);
    }

    [BindProperty(SupportsGet = true)] public string Query { get; set; }
    [BindProperty] public PaginatedList<ProfileItemType> ProfileItemTypes { get; set; }
}