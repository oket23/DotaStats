using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ProTeamsMicroService.Models;
using System.Text.Json;

namespace ProTeams.Api.Services;

public class ProTeamsService
{
    private readonly HttpClient _client;
    private readonly ILogger<ProTeamsService> _logger;
    private readonly IDistributedCache _cache;
    public ProTeamsService(HttpClient client, ILogger<ProTeamsService> logger, IDistributedCache cache)
    {
        _client = client;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<ResponseTeam>> GetProTeamAndFavoriteHeroAsync(int limit, int page)
    {
        var key = $"proTeams:favoriteHero:limit={limit}:page={page}";
        var result = new List<ResponseTeam>();

        var json = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(json))
        {
            var teams = await GetTeamsAsync(limit, page);

            var tasks = teams.Select(async team => new ResponseTeam
            {
                Team = team,
                FavoriteHero = await GetFavoriteHeroByTeamAsync(team)
            });

            result = (await Task.WhenAll(tasks)).ToList();

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });
            _logger.LogInformation("ProTeamsService got all pro teams and favorite hero from API");
        }
        else
        {
            _logger.LogInformation("ProTeamsService got all pro teams and favorite hero from cache");
            result = JsonSerializer.Deserialize<List<ResponseTeam>>(json);
        }

        _logger.LogInformation($"All teams with favorite hero successfully returned");
        return result;
    }

    private async Task<TeamHeroStats> GetFavoriteHeroByTeamAsync(Team team)
    {
        var heroStats = await GetHeroStatsAsync(team.Id);

        if (heroStats != null)
        {
            var favorite = new TeamHeroStats();
            int maxGames = int.MinValue;

            foreach (var hero in heroStats)
            {
                if (hero.Games > maxGames)
                {
                    maxGames = hero.Games;
                    favorite = hero;
                }
            }

            _logger.LogInformation($"Heroes stats for team with id: {team.Id} successfully returned");
            return favorite;
        }

        _logger.LogError($"Heroes stats for team with id: {team.Id} not found");
        throw new HttpRequestException($"Heroes stats for team with id: {team.Id} not found");
    }

    public async Task<List<Team>> GetTeamsAsync(int limit = 10, int page = 1)
    {
        var key = $"proTeams:allTeams:limit={limit}:page={page}";

        var json = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(json))
        {
            var response = await _client.GetAsync("teams");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get pro teams from API");
                throw new HttpRequestException("Failed to get pro teams from API");
            }

            var allTeams = JsonSerializer.Deserialize<List<Team>>(responseContent);

            var paginationTeams = allTeams
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            _logger.LogInformation("ProTeamsService got pro teams from API");

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(paginationTeams), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });

            return paginationTeams;
        }
        else
        {
            _logger.LogInformation("ProTeamsService got all pro teams from cache");
            return JsonSerializer.Deserialize<List<Team>>(json);
        }
    }

    private async Task<List<TeamHeroStats>> GetHeroStatsAsync(int teamId)
    {
        var response = await _client.GetAsync($"teams/{teamId}/heroes");
        var responseContext = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Failed to got hero stats for team: {teamId} from API");
            throw new HttpRequestException($"Failed to got hero stats for team: {teamId} from API");
        }

        _logger.LogInformation($"ProTeamsService got hero stats for team: {teamId} from API");
        return JsonSerializer.Deserialize<List<TeamHeroStats>>(responseContext);
    }
}
