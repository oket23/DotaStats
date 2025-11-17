using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ProPlayersMicroService.Models;
using System.Text.Json;

namespace ProPlayers.Api.Services;

public class ProPlayersServices
{
    private readonly HttpClient _client;
    private readonly ILogger<ProPlayersServices> _logger;
    private readonly IDistributedCache _cache;
    public ProPlayersServices(HttpClient client, ILogger<ProPlayersServices> logger, IDistributedCache cache)
    {
        _client = client;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<ProPlayer>> GetProPlayersAsync(int limit = 10, int page = 1)
    {
        var paginationProPlayers = new List<ProPlayer>();
        var key = $"proPlayers:all:limit={limit}:page={page}";

        var json = await _cache.GetStringAsync(key);
        if(string.IsNullOrEmpty(json))
        {
            var response = await _client.GetAsync("proPlayers");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get pro players from API");
                throw new HttpRequestException("Failed to get pro players from API");
            }
            var allPlayers = JsonSerializer.Deserialize<List<ProPlayer>>(responseContent);

            paginationProPlayers = allPlayers.Skip((page - 1) * limit)
                .Take(limit)
                .ToList();

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(paginationProPlayers), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });

            _logger.LogInformation("ProPlayersService gets pro players from Dota 2 API");
            return paginationProPlayers;
        }
        else
        {
            _logger.LogInformation("ProPlayersService gets pro players from Dota 2 cache");
            return JsonSerializer.Deserialize<List<ProPlayer>>(json);
        }
    }
}
