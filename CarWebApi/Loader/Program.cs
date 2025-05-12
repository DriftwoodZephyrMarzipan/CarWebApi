using CarWebApi.Data.Providers;
using CarWebApi.Data.Repositories;
using Microsoft.Extensions.Configuration;


/**
 * This program processes a JSON file containing electric vehicle data from Washington State.
 * It extracts makes, models, and car details and prints the counts of each, then adds them
 * to the database specified in the appsettings file.
 **/

if (args.Length != 1)
{
    Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} <filename>");
    return;
}

string filename = args[0];

var fileProvider = new WashingtonEVJSONFileProvider();
var (makes, models, cars) = fileProvider.ProcessFile(filename);

Console.WriteLine("Processed " + makes.Count + " makes, " + models.Count + " models, and " + cars.Count + " cars.");

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

var repository = new PostGreSQLCarRepository(connectionString);

foreach(var car in cars)
{
    repository.CreateCarAsync(car).Wait();
}
