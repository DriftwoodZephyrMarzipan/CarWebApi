namespace CarWebApi.Services;

public class RequestTelemetryService : IRequestTelemetryService
{
    private readonly Dictionary<string, Dictionary<string, int>> _data = new();
    private readonly object _lock = new();

    public void Record(string controller, string action)
    {
        lock (_lock)
        {
            if (!_data.ContainsKey(controller))
                _data[controller] = new Dictionary<string, int>();

            if (!_data[controller].ContainsKey(action))
                _data[controller][action] = 0;

            _data[controller][action]++;
        }
    }

    public IDictionary<string, IDictionary<string, int>> GetStats()
    {
        lock (_lock)
        {
            // Return a copy to avoid exposing internal data
            return _data.ToDictionary(
                entry => entry.Key,
                entry => (IDictionary<string, int>)entry.Value.ToDictionary(kv => kv.Key, kv => kv.Value)
            );
        }
    }
}
