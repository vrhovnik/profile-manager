using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.ProfileItems;

public class IndexPageModel(
    ILogger<IndexPageModel> logger,
    IProfileItemRepository profileItemRepository,
    IOptions<DataOptions> dataOptions)
    : PageModel
{
    public async Task<IActionResult> OnGetAsync(int? pageNumber)
    {
        logger.LogInformation("Loading index page for profile items {DateCreated}", DateTime.Now);
        var page = pageNumber ?? 1;
        logger.LogInformation("Loaded profile items page with query - {Query}", Query);
        ProfileItems = await profileItemRepository.SearchAsync(page, dataOptions.Value.PagingSize, Query);
        logger.LogInformation("Loaded {ProfileItemsCount} profile items", ProfileItems.Count);

        if (!Request.IsHtmx()) return Page();

        return Partial("_ProfileItemList", ProfileItems);
    }

    [BindProperty(SupportsGet = true)] public string Query { get; set; }
    [BindProperty] public PaginatedList<ProfileItem> ProfileItems { get; set; }
}