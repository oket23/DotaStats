using ProTeamsMicroService.Models;
using Serilog.Core;
using System.Text.Json;

namespace UI.Services;

public class ProTeamsService
{
    private readonly HttpClient _httpClient;
    private readonly Logger _logger;

    public ProTeamsService(Logger logger, HttpClient client, string uri)
    {
        _logger = logger;
        _httpClient = client;
        _httpClient.BaseAddress = new Uri(uri);
    }

    public async Task<List<Team>> GetProTeamsAsync(int limit,int page)
    {
        var response = await _httpClient.GetAsync($"/pro_teams?limit={limit}&page={page}");
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.Error(responseContent);
            throw new HttpRequestException(responseContent);
        }

        _logger.Information("ProTeamsService returns all pro teams");
        return JsonSerializer.Deserialize<List<Team>>(responseContent);
    }

    public async Task<List<ResponseTeam>> GetProTeamsAndFavoriteHeroAsync(int limit, int page)
    {
        var response = await _httpClient.GetAsync($"pro_teams/favorite?limit={limit}&page={page}");
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.Error(responseContent);
            throw new HttpRequestException(responseContent);
        }

        _logger.Information("ProTeamsService returns all pro teams and favorite hero");
        return JsonSerializer.Deserialize<List<ResponseTeam>>(responseContent);
    }
}
