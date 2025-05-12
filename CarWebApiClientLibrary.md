## CarWebApiClientLibrary

This class is a client for the Car Web API. It provides methods to interact with the API endpoints for Makes, Models, Cars, CafvTypes, and EvTypes.

Though it hides much of the detail, it sets the LastHttpResponseMessage value to the last HttpResponse received. This can be interrogated for error messages or further processing.

Example Usage:

```
var client = new CarWebApiClient("https://localhost:5001");

var models = await client.GetAllModelsAsync();
foreach (var model in models)
{
	Console.WriteLine($"Model ID: {model.Id}, Name: {model.ModelName}");
}

var modelById = await client.GetModelAsync(1);
Console.WriteLine($"Model ID: {modelById.Id}, Name: {modelById.ModelName}");

var lastResponse = client.LastHttpResponseMessage;
if (lastResponse != null)
{
	Console.WriteLine($"Last HTTP Response: {lastResponse.StatusCode}");
	Console.WriteLine($"Last HTTP Response Content: {await lastResponse.Content.ReadAsStringAsync()}");
}
else
{
	Console.WriteLine("No HTTP response received.");
}
```

