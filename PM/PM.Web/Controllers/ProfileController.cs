using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PM.Core;
using PM.Interfaces;
using PM.Models;

namespace PM.Web.Controllers;

[AllowAnonymous, ApiController, Route(RouteHelper.ApiProfilesBaseRoute),
 Produces(MediaTypeNames.Application.Json)]
public class ProfileController(
    ILogger<ProfileController> logger,
    IProfileTypeRepository profileTypeRepository,
    IProfileRepository profileRepository) :
    BaseController<ProfileController>(logger)
{
    [HttpGet]
    [Route(RouteHelper.ApiGetAllProfileTypesRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Produces(typeof(List<ProfileType>))]
    public async Task<IActionResult> GetAllProfileTypesAsync()
    {
        logger.LogInformation("Called get all profile types endpoint at {DateCalled}", DateTime.UtcNow);
        var profileTypes = await profileTypeRepository.GetAsync();
        logger.LogInformation("Returning {Count} profile types from database", profileTypes.Count);
        return Ok(profileTypes);
    }
}