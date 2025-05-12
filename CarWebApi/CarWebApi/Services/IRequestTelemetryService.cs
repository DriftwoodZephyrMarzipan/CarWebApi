namespace CarWebApi.Services;

public interface IRequestTelemetryService
{
    void Record(string controller, string action);
    IDictionary<string, IDictionary<string, int>> GetStats();
}
