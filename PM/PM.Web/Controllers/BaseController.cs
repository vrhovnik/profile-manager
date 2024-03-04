using Microsoft.AspNetCore.Mvc;
using PM.Core;

namespace PM.Web.Controllers;

public abstract class BaseController<T>(ILogger<T> logger) : Controller where T : class
{
    protected readonly ILogger<T> logger = logger;
    [HttpGet]
    [Route(RouteHelper.HealthRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult IsAlive()
    {
        logger.LogInformation("Called alive endpoint {Controller} at {DateCalled}", nameof(T), DateTime.UtcNow);
        return new ContentResult { StatusCode = 200, Content = $"I am alive at {DateTime.Now}" };
    }
}