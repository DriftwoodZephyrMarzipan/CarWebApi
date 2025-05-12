using System.ComponentModel;
using System.Text.Json.Serialization;

namespace CarWebApi.EvDataModels.Models;

public class Model
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("make")]
    public Make Make { get; set; } = null!;
    [JsonPropertyName("model")]
    public string ModelName { get; set; } = string.Empty;
    [JsonPropertyName("model_year")]
    public int ModelYear { get; set; }
    [JsonPropertyName("ev_type")]
    public EvType EvType { get; set; } = EvType.None;
    [JsonPropertyName("cafv_type")]
    public CafvType CafvType { get; set; } = CafvType.None;
    [JsonPropertyName("electric_range")]
    public int ElectricRange { get; set; } = 0;
    [JsonPropertyName("base_msrp")]
    public decimal BaseMSRP { get; set; } = 0.0m;
}

public enum EvType
{
    [Description("None")]
    None,
    [Description("Plug-in Hybrid Electric Vehicle (PHEV)")]
    PHEV,
    [Description("Battery Electric Vehicle (BEV)")]
    BEV
}

public enum CafvType
{
    [Description("None")]
    None,
    [Description("Not eligible due to low battery range")]
    NotEligibleDueToRange,
    [Description("Eligibility unknown as battery range has not been researched")]
    EligiblityUnkonwnBatteryRangeNotResearched,
    [Description("Clean Alternative Fuel Vehicle Eligible")]
    CleanAlternativeFuelVehicleEligible,
}
