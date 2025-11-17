using HeroMicroService.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using UI.Models;

namespace Heros.Api.Services;

public class HeroApiService
{
    private readonly HttpClient _client;
    private readonly ILogger<HeroApiService> _logger;
    private readonly IDistributedCache _cache;
    public HeroApiService(HttpClient client, ILogger<HeroApiService> logger, IDistributedCache cache)
    {
        _client = client;
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<Hero>> GetAllHeroAsync()
    {
        var key = "heroes:all";

        var json = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(json))
        {
            var response = await _client.GetAsync("heroes");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get heroes from API");
                throw new HttpRequestException("Failed to get heroes from API");
            }
            await _cache.SetStringAsync(key, responseContent, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });

            _logger.LogInformation("HeroApiService gets all heroes from Dota 2 api");
            return JsonSerializer.Deserialize<List<Hero>>(responseContent);
        }
        else
        {
            _logger.LogInformation("HeroApiService gets all heroes from cache");
            return JsonSerializer.Deserialize<List<Hero>>(json);
        }
        
    }
    public async Task<Hero> GetHeroByIdAsync(int heroId)
    {
        var allHero = await GetAllHeroAsync();

        _logger.LogInformation($"HeroApiService gets hero with id:{heroId} from Dota 2 api");
        return allHero.FirstOrDefault(x => x.Id.Equals(heroId));
    }

    public async Task<HeroStatsWrapper> GetHeroBenchmarksAsync(int heroId)
    {
        var key = $"heroes:benchmarks:{heroId}";

        var json = await _cache.GetStringAsync(key);
        if (string.IsNullOrEmpty(json))
        {
            var response = await _client.GetAsync($"benchmarks?hero_id={heroId}");
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get hero benchmarks from API");
                throw new HttpRequestException("Failed to get hero benchmarks from API");
            }

            _logger.LogInformation($"HeroApiService gets benchmark by hero with id:{heroId} from Dota 2 api");

            await _cache.SetStringAsync(key, responseContent, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
            });

            return JsonSerializer.Deserialize<HeroStatsWrapper>(responseContent);
        }
        else
        {
            _logger.LogInformation($"HeroApiService gets benchmark by hero with id:{heroId} from cache");
            return JsonSerializer.Deserialize<HeroStatsWrapper>(json);
        }
    }

}
