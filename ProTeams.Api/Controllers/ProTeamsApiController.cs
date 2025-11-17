using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProTeams.Api.Services;
using ProTeamsMicroService.Models;

namespace ProTeams.Api.Controllers;

[ApiController]
[Route("proteams")]
[Produces("application/json")]
public class ProTeamsController : ControllerBase
{
    private readonly ILogger<ProTeamsController> _logger;
    private readonly ProTeamsService _proTeamsService;

    public ProTeamsController(ILogger<ProTeamsController> logger, ProTeamsService proTeamsService)
    {
        _logger = logger;
        _proTeamsService = proTeamsService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Team>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProTeams([FromQuery] int limit = 10, [FromQuery] int page = 1)
    {
        var result = await _proTeamsService.GetTeamsAsync(limit, page);
        return Ok(result);
    }

    [HttpGet("favorite")]
    [ProducesResponseType(typeof(List<ResponseTeam>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProTeamsAndFavoriteHeroes([FromQuery] int limit = 10, [FromQuery] int page = 1)
    {
        var result = await _proTeamsService.GetProTeamAndFavoriteHeroAsync(limit, page);
        return Ok(result);
    }
}
