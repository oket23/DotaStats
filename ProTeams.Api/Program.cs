using Microsoft.OpenApi.Models;
using ProTeams.Api.Services;
using Serilog;

namespace ProTeams.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(builder.Configuration["Logging:LogPath"], outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [HeroMicroService]: {Message:lj}{NewLine}{Exception}")
            .MinimumLevel.Debug()
            .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProTeams API", Version = "v1" });
            c.AddServer(new OpenApiServer { Url = "/proteams-api" });
        });

        builder.Services.AddControllers();

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration["Redis:ConnectionString"];
        });

        builder.Services.AddHttpClient<ProTeamsService>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["OpenDotaPath"]);
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(x =>
        {
            x.SwaggerEndpoint("/swagger/v1/swagger.json", "ProTeams API V1");
        });

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
