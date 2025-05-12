namespace CarWebApi.Services
{
    public interface IDatabaseTelemetryService
    {
        void LogQuery(string query, long durationMs);
        List<string> GetLastQueries();
    }
}
