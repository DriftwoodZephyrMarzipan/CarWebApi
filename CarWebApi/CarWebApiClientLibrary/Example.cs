namespace CarWebApiClientLibrary
{
    public class Example
    {
        public static async Task Main(string[] args)
        {
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
        }


    }
}
