using ProPlayersMicroService.Models;
using Serilog.Core;
using System.Text.Json;

namespace UI.Services;

public class ProPlayersService
{
    private readonly HttpClient _httpClient;
    private readonly Logger _logger;

    public ProPlayersService(Logger logger, HttpClient client, string uri)
    {
        _logger = logger;
        _httpClient = client;
        _httpClient.BaseAddress = new Uri(uri);
    }

    public async Task<List<ProPlayer>> GetProPlayersAsync(int limit, int page)
    {
        var response = await _httpClient.GetAsync($"/proPlayers?limit={limit}&page={page}");
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.Error(responseContent);
            throw new HttpRequestException(responseContent);
        }

        _logger.Information("ProPlayersService returns all pro players");
        return JsonSerializer.Deserialize<List<ProPlayer>>(responseContent);
    }
}
