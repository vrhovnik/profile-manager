using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;

namespace PM.Web.Pages.Categories;

[Authorize]
public class DeletePageModel(
    ILogger<DeletePageModel> logger,
    ICategoryRepository categoryRepository)
    : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading city  with {CityId}", Id);
        CurrentCategory = await categoryRepository.DetailsAsync(Id);
        logger.LogInformation("Loaded city {Name}", CurrentCategory.Name);
    }

    public async Task<RedirectToPageResult> OnPostAsync()
    {
        try
        {
            logger.LogInformation("Deleting city with id {Id}", Id);
            await categoryRepository.DeleteAsync(Id);
            logger.LogInformation("City with {Id} deleted", Id);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return RedirectToPage("/Categories/Delete/{Id}", new { Id });
        }

        return RedirectToPage("/Categories/Index");
    }

    [BindProperty(SupportsGet = true)] public string Id { get; set; }
    [BindProperty] public Models.Category CurrentCategory { get; set; }
}