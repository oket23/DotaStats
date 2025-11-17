using Microsoft.Extensions.Configuration;
using Serilog;
using UI.Services;

namespace TelegramUI;

public class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) 
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var logger = new LoggerConfiguration()
            .WriteTo.File(configuration["Logging:LogPath"], outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [HeroMicroService]: {Message:lj}{NewLine}{Exception}")
            .MinimumLevel.Debug()
            .CreateLogger();

        var httpClient = new HttpClient();
        var heroService = new HeroService(logger, httpClient, configuration["HerosApiPath"]);
        var proPlayerService = new ProPlayersService(logger, httpClient, configuration["ProTeamsApiPath"]);
        var proTeamService = new ProTeamsService(logger, httpClient, configuration["ProPlayersApiPath"]);
        var telegramService = new TelegramBotService(logger, proPlayerService, proTeamService, heroService, configuration["TelegramToken"]);

        telegramService.StartTelegramBot();
    }
}
