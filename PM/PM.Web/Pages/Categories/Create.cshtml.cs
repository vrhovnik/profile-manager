using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Pages.Categories;

[Authorize]
public class CreatePageModel(ILogger<CreatePageModel> logger, ICategoryRepository categoryRepository)
    : PageModel
{
    public void OnGetAsync() => logger.LogInformation("Loading category create page at {DateLoaded}", DateTime.Now);

    public async Task<IActionResult> OnPostAsync()
    {
        var form = await Request.ReadFormAsync();

        try
        {
            await categoryRepository.InsertAsync(CurrentCategory);
            logger.LogInformation("Category with {Name} has been saved", CurrentCategory.Name);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return Page();
        }

        return RedirectToPage("/Categories/Index");
    }

    [BindProperty] public Category CurrentCategory { get; set; }
}