using System.Text.Json;
using System.Text.Json.Serialization;
using NpgsqlTypes;

namespace CarWebApi.EvDataModels.Converters;

/// <summary>
/// Custom JSON converter for NpgsqlPoint. This will allow ingestion and output of PostGreSQL POINT type 
/// data in the following format:  "POINT (LONG,LAT)", with LONG and LAT indicated the longitude and latitude
/// to 5 digits of precision past the mantissa.
/// </summary>
public class NpgsqlPointJsonConverter : JsonConverter<NpgsqlPoint>
{
    public override NpgsqlPoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value == null || !value.StartsWith("POINT (") || !value.EndsWith(")"))
        {
            throw new JsonException("Invalid POINT format.");
        }

        // Parse the "POINT (x y)" format
        var pointData = value[7..^1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (pointData.Length != 2 ||
            !double.TryParse(pointData[0], out var x) ||
            !double.TryParse(pointData[1], out var y))
        {
            throw new JsonException("Invalid POINT data.");
        }

        return new NpgsqlPoint(x, y);
    }

    public override void Write(Utf8JsonWriter writer, NpgsqlPoint value, JsonSerializerOptions options)
    {
        // Format as "POINT (x y)" with 5 digits of precision
        writer.WriteStringValue($"POINT ({value.X:F5} {value.Y:F5})");
    }
}