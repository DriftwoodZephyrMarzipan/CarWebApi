namespace CarWebApi.Services;

public class DatabaseTelemetryService : IDatabaseTelemetryService
{
    private readonly ILogger<DatabaseTelemetryService> _logger;
    private readonly List<string> _queryLog = new List<string>();

    public DatabaseTelemetryService(ILogger<DatabaseTelemetryService> logger)
    {
        _logger = logger;
    }

    public void LogQuery(string query, long durationMs)
    {
        _queryLog.Insert(0, $"{query} ({durationMs}ms)");

        // Keep only the last 10 queries
        if (_queryLog.Count > 10)
        {
            _queryLog.RemoveAt(10);
        }

        _logger.LogInformation("Executed query: {Query} in {Duration}ms", query, durationMs);
    }

    public List<string> GetLastQueries() => _queryLog;
}
