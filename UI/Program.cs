using Microsoft.Extensions.Configuration;
using Serilog;
using System.Threading.Tasks;
using UI.Services;

namespace UI;

public class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) 
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var logger = new LoggerConfiguration()
            .WriteTo.File(configuration["Logging:LogPath"], outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [HeroMicroService]: {Message:lj}{NewLine}{Exception}")
            .MinimumLevel.Debug()
            .CreateLogger();

        var heroHttpClient = new HttpClient() { BaseAddress = new Uri(configuration["HerosApiPath"]) };
        var proPlayeHttpClient = new HttpClient() { BaseAddress = new Uri(configuration["ProPlayersApiPath"]) }; 
        var proTeamHttpClient = new HttpClient() { BaseAddress = new Uri(configuration["ProTeamsApiPath"]) };
        var heroService = new HeroService(logger, heroHttpClient);
        var proPlayerService = new ProPlayersService(logger, proPlayeHttpClient);
        var proTeamService = new ProTeamsService(logger, proTeamHttpClient);
        var telegramService = new TelegramBotService(logger, proPlayerService, proTeamService, heroService, configuration["TelegramToken"]);

        telegramService.StartTelegramBot();
        while (true)
        {
            await Task.Delay(1000);
        }
    }
}
