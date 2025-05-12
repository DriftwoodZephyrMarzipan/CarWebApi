using CarWebApi.EvDataModels.Converters;
using NpgsqlTypes;
using System.Text.Json.Serialization;

namespace CarWebApi.EvDataModels.Models;

public class Car
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("uuid")]
    public Guid Uuid { get; set; } = Guid.Empty;
    [JsonPropertyName("sid")]
    public string Sid { get; set; } = string.Empty;
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("vin_1_10")]
    public string Vin1To10 { get; set; } = string.Empty;
    [JsonPropertyName("county")]
    public string County { get; set; } = null!;
    [JsonPropertyName("city")]
    public string City { get; set; } = null!;
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;
    [JsonPropertyName("zip_code")]
    public string ZipCode { get; set; } = null!;
    [JsonPropertyName("model")]
    public Model Model { get; set; } = null!;
    [JsonPropertyName("legislative_district")]
    public string LegislativeDistrict { get; set; } = null!;
    [JsonPropertyName("dol_vehicle_id")]
    public string DolVehicleId { get; set; } = string.Empty;
    [JsonPropertyName("vehicle_location")]
    [JsonConverter(typeof(NpgsqlPointJsonConverter))]
    public NpgsqlPoint? VehicleLocation { get; set; }
    [JsonPropertyName("electric_utility")]
    public string ElectricUtility { get; set; } = null!;
    /// <summary>
    /// This appears to be a sequence value with unknown constraints, stored
    /// as a string since without a spec we cannot tell if it's a number or an identifier
    /// </summary>
    [JsonPropertyName("_2020_census_tract")]
    public string CensusTract2020 { get; set; } = null!;
    /// <summary>
    /// It is assumed this is a choropleth indicator for a specific region on a map
    /// </summary>
    [JsonPropertyName("counties")]
    public Int64? Counties { get; set; }
    /// <summary>
    /// It is assumed this is a choropleth indicator for a specific region on a map
    /// </summary>
    [JsonPropertyName("districts")]
    public Int64? Districts { get; set; }
    /// <summary>
    /// It is assumed this is a choropleth indicator for a specific region on a map
    /// </summary>
    [JsonPropertyName("legislative_district_boundary")]
    public Int64? LegislativeDistrictBoundary { get; set; }
}
