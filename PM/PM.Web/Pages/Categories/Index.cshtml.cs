using Htmx;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.Categories;

public class IndexPageModel(
    ILogger<IndexPageModel> logger,
    ICategoryRepository cityRepository,
    IOptions<DataOptions> dataOptions)
    : PageModel
{
    public async Task<IActionResult> OnGetAsync(int? pageNumber)
    {
        logger.LogInformation("Loading index page for categories {DateCreated}", DateTime.Now);
        var page = pageNumber ?? 1;
        logger.LogInformation("Loaded categories page with query - {Query}", Query);
        Categories = await cityRepository.SearchAsync(page, dataOptions.Value.PagingSize, Query);
        logger.LogInformation("Loaded {CategoryCount} items", Categories.Count);

        if (!Request.IsHtmx()) return Page();

        return Partial("_CategoriesList", Categories);
    }

    [BindProperty(SupportsGet = true)] public string Query { get; set; }
    [BindProperty] public PaginatedList<Category> Categories { get; set; }
}