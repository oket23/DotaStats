using ProPlayersMicroService.Models;
using Serilog.Core;
using System.Text.Json;

namespace UI.Services;

public class ProPlayersService
{
    private readonly HttpClient _httpClient;
    private readonly Logger _logger;

    public ProPlayersService(Logger logger, HttpClient client)
    {
        _logger = logger;
        _httpClient = client;
    }

    public async Task<List<ProPlayer>> GetProPlayersAsync(int limit, int page)
    {
        _logger.Information($"Sending request to {_httpClient.BaseAddress}");
        var response = await _httpClient.GetAsync($"proplayers?limit={limit}&page={page}");
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
