using System.Text.Json.Serialization;

namespace CarWebApi.EvDataModels.Models;

public class Make
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("manufacturer")]
    public string Manufacturer { get; set; } = string.Empty;
}
