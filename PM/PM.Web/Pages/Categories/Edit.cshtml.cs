using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.Categories;

[Authorize]
public class EditPageModel(
    ILogger<EditPageModel> logger,
    ICategoryRepository categoryRepository)
    : PageModel
{
    public async Task OnGetAsync()
    {
        logger.LogInformation("Loading category edit page at {DateLoaded}", DateTime.Now);
        CurrentCategory = await categoryRepository.DetailsAsync(Id);
        logger.LogInformation("Category {Name} loaded", CurrentCategory.Name);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogInformation("Saving category with id {CityId}", Id);
        var form = await Request.ReadFormAsync();
        try
        {
            await categoryRepository.UpdateAsync(CurrentCategory);
            logger.LogInformation("Category {Name} has been updated", CurrentCategory.Name);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return RedirectToPage("/Categories/Edit/{id}", new { Id });
        }

        return RedirectToPage("/Categories/Index");
    }

    [BindProperty(SupportsGet = true)] public int Id { get; set; }
    [BindProperty] public Category CurrentCategory { get; set; }
}