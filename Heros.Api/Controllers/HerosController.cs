using HeroMicroService.Models;
using Heros.Api.Services;
using Microsoft.AspNetCore.Mvc;
using UI.Models;

namespace Heros.Api.Controllers;

[ApiController]
[Route("heros")]
[Produces("application/json")]
public class HerosController : ControllerBase
{
    private readonly ILogger<HerosController> _logger;
    private readonly HeroApiService _heroService;

    public HerosController(ILogger<HerosController> logger, HeroApiService heroService)
    {
        _logger = logger;
        _heroService = heroService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Hero>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllHerosAsync()
    {
        var heros = await _heroService.GetAllHeroAsync();
        return Ok(heros);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Hero), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHeroByIdAsync(int id)
    {
        var hero = await _heroService.GetHeroByIdAsync(id);
        if (hero == null)
            return NotFound();

        return Ok(hero);
    }

    [HttpGet("{id:int}/benchmarks")]
    [ProducesResponseType(typeof(HeroStatsWrapper), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHeroBenchmarksAsync(int id)
    {
        var benchmarks = await _heroService.GetHeroBenchmarksAsync(id);
        return Ok(benchmarks);
    }
}