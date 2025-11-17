namespace Gateway.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();

        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/heros-api/swagger/v1/swagger.json", "Heros API");
                c.SwaggerEndpoint("/proteams-api/swagger/v1/swagger.json", "ProTeams API");
                c.SwaggerEndpoint("/proplayers-api/swagger/v1/swagger.json", "ProPlayers API");
            });
        }

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapReverseProxy();

        app.Run();
    }
}
