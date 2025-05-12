using CarWebApi.Data.Repositories;
using Microsoft.Extensions.Configuration;

namespace CarWebApiFunctionalTests
{
    public static class TestDataManager
    {
        public static ICarRepository Repository { get; set; } = null!;

        static TestDataManager()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Connection string not found in appsettings.json.");
                return;
            }

            Repository = new PostGreSQLCarRepository(connectionString);
        }     
    }
}
