using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PM.Core;

namespace PM.Web.Controllers;

[AllowAnonymous, ApiController, Route(RouteHelper.ApiCategoriesBaseRoute),
 Produces(MediaTypeNames.Application.Json)]
public class CategoryController(ILogger<CategoryController> logger) : Controller
{
    [HttpGet]
    [Route(RouteHelper.HealthRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult IsAlive()
    {
        logger.LogInformation("Called alive endpoint for categories API at {DateCalled}", DateTime.UtcNow);
        return new ContentResult { StatusCode = 200, Content = $"I am alive at {DateTime.Now}" };
    }
}