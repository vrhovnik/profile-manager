using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Controllers;

[AllowAnonymous, ApiController, Route(RouteHelper.ApiCategoriesBaseRoute),
 Produces(MediaTypeNames.Application.Json)]
public class CategoryController(ILogger<CategoryController> logger, ICategoryRepository categoryRepository) : Controller
{
    [HttpGet]
    [Route(RouteHelper.HealthRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult IsAlive()
    {
        logger.LogInformation("Called alive endpoint for categories API at {DateCalled}", DateTime.UtcNow);
        return new ContentResult { StatusCode = 200, Content = $"I am alive at {DateTime.Now}" };
    }

    [HttpGet]
    [Route(RouteHelper.ApiGetAllRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(List<Category>))]
    public async Task<IActionResult> GetAllAsync()
    {
        logger.LogInformation("Called get all categories endpoint at {DateCalled}", DateTime.UtcNow);
        var categories = await categoryRepository.GetAsync();
        logger.LogInformation("Returning {Count} categories from database", categories.Count);
        return Ok(categories);
    }
}