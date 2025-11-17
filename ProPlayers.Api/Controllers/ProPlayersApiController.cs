using Microsoft.AspNetCore.Mvc;
using ProPlayers.Api.Services;
using ProPlayersMicroService.Models;

namespace ProPlayers.Api.Controllers;

[ApiController]
[Route("proplayers")]
[Produces("application/json")]
public class ProPlayersController : ControllerBase
{
    private readonly ILogger<ProPlayersController> _logger;
    private readonly ProPlayersServices _proPlayersService;

    public ProPlayersController(ILogger<ProPlayersController> logger, ProPlayersServices proPlayersService)
    {
        _logger = logger;
        _proPlayersService = proPlayersService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ProPlayer>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProPlayers([FromQuery] int limit = 10, [FromQuery] int page = 1)
    {
        var players = await _proPlayersService.GetProPlayersAsync(limit, page);
        return Ok(players);
    }
}
