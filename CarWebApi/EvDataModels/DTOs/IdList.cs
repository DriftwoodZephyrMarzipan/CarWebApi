using System.Text.Json.Serialization;

namespace CarWebApi.EvDataModels.DTOs;

public  class IdList
{
    [JsonPropertyName("id_list")]
    public List<int> Ids { get; set; } = new List<int>();
    [JsonPropertyName("data_type")]
    public string DataType { get; set; } = string.Empty;

    public IdList()
    {
    }
    public IdList(List<int> ids)
    {
        Ids = ids;
    }


}
