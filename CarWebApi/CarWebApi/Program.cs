using CarWebApi.Data.Repositories;
using Serilog;

namespace CarWebApi;

public class Program
{    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "Logs", "log.txt"), 
                          rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog();

        // add configs
        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Configuration.AddEnvironmentVariables();

        // This lets us handle PostGreSQL timestamps <-> datetimes gracefully
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();


        builder.Services.AddEndpointsApiExplorer();

        builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
        builder.Services.AddSingleton<ICarRepository, PostGreSQLCarRepository>();

        var app = builder.Build();

        //if (app.Environment.IsDevelopment())
        //{
        app.MapOpenApi();
        app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "EV Cars"));
        //}

        // Configure the HTTP request pipeline.
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
