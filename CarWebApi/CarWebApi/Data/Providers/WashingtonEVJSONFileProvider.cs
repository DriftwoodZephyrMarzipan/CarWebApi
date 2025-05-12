using CarWebApi.EvDataModels.Models;
using CarWebApi.Utils;
using NpgsqlTypes;
using System.Text.Json;

namespace CarWebApi.Data.Providers;

public class WashingtonEVJSONFileProvider
{
    public (List<Make> Makes, List<Model> Models, List<Car> Cars) ProcessFile(string fileName)
    {
        // Read and parse the JSON file
        var jsonData = File.ReadAllText(fileName);
        var document = JsonDocument.Parse(jsonData);

        // Extract the "data" array
        var dataRows = document.RootElement.GetProperty("data").EnumerateArray();

        var uniqueMakes = new Dictionary<string, Make>();
        var uniqueModels = new Dictionary<string, Model>();
        var uniqueCars = new Dictionary<Guid,Car>();

        foreach (var row in dataRows)
        {
            var rowArray = row.EnumerateArray().ToArray();

            // Extract fields with null checks
            var sid = rowArray[0].GetString() ?? throw new JsonException("Sid is null");
            var uuid = rowArray[1].GetString() ?? throw new JsonException("Uuid is null");
            var createdAt = rowArray[3].ValueKind == JsonValueKind.Number
                ? DateTimeOffset.FromUnixTimeSeconds(rowArray[3].GetInt64()).UtcDateTime
                : throw new JsonException("CreatedAt is null or invalid");
            var updatedAt = rowArray[5].ValueKind == JsonValueKind.Number
                ? DateTimeOffset.FromUnixTimeSeconds(rowArray[5].GetInt64()).UtcDateTime
                : throw new JsonException("UpdatedAt is null or invalid");
            var vin1To10 = rowArray[8].GetString() ?? throw new JsonException("Vin1To10 is null");
            var county = rowArray[9].GetString() ?? string.Empty;
            var city = rowArray[10].GetString() ?? string.Empty;
            var state = rowArray[11].GetString() ?? string.Empty;
            var zipCode = rowArray[12].GetString() ?? string.Empty;
            var modelYear = rowArray[13].ValueKind == JsonValueKind.String && int.TryParse(rowArray[13].GetString(), out var year)
                ? year
                : throw new JsonException("ModelYear is null or invalid");
            var manufacturer = rowArray[14].GetString() ?? throw new JsonException("Manufacturer is null");
            var modelName = rowArray[15].GetString() ?? throw new JsonException("ModelName is null");
            var evType = rowArray[16].GetString() ?? throw new JsonException("EvType is null");
            var cafvType = rowArray[17].GetString() ?? throw new JsonException("CafvType is null");
            var electricRange = rowArray[18].ValueKind == JsonValueKind.String && int.TryParse(rowArray[18].GetString(), out var range)
                ? range
                : 0;
            var baseMsrp = rowArray[19].ValueKind == JsonValueKind.String && int.TryParse(rowArray[19].GetString(), out var msrp)
                ? msrp
                : 0;
            var legislativeDistrict = rowArray[20].GetString() ?? string.Empty;
            var dolVehicleId = rowArray[21].GetString() ?? string.Empty;
            var vehicleLocation = rowArray[22].GetString() != null
                ? ParsePoint(rowArray[22].GetString()!)
                : null;
            var electricUtility = rowArray[23].GetString() ?? string.Empty;
            var censusTract2020 = rowArray[24].GetString() ?? string.Empty;
            var counties = rowArray[25].ValueKind == JsonValueKind.Number ? rowArray[25].GetInt64() : 0;
            var districts = rowArray[26].ValueKind == JsonValueKind.Number ? rowArray[26].GetInt64() : 0;
            var legislativeDistrictBoundary = rowArray[27].ValueKind == JsonValueKind.Number ? rowArray[27].GetInt64() : 0;

            Make newMake = new Make() { Manufacturer = manufacturer };
            // Add Make if unique
            if (!uniqueMakes.TryGetValue(manufacturer, out var curMake))
            {
                uniqueMakes.Add(manufacturer, newMake);
            }
            else
            {
                newMake = curMake;
            }

            // Add Model if unique - or overwrite existing model if the prior model is 'bad'
            var newModel = new Model
            {
                ModelName = modelName,
                ModelYear = modelYear,
                EvType = EnumHelper.GetEnumValueFromDescription<EvType>(evType),
                CafvType = EnumHelper.GetEnumValueFromDescription<CafvType>(cafvType),
                ElectricRange = electricRange,
                BaseMSRP = baseMsrp,
                Make = newMake
            };

            if (!uniqueModels.TryGetValue(manufacturer + "_" + modelName, out var storedModel))
            {
                uniqueModels.Add(manufacturer + "_" + modelName, newModel);
            }
            else
            {
                if (storedModel != null &&
                    (storedModel.EvType == EvType.None || storedModel.CafvType == CafvType.None || storedModel.ElectricRange == 0 || storedModel.BaseMSRP == 0))
                {
                    // Update the existing model with new values if the existing values are 'bad'
                    storedModel.EvType = newModel.EvType;
                    storedModel.CafvType = newModel.CafvType;
                    storedModel.ElectricRange = newModel.ElectricRange;
                    storedModel.BaseMSRP = newModel.BaseMSRP;
                }
                newModel = storedModel;
            }

            var newCar = new Car
            {
                Sid = sid,
                Uuid = Guid.Parse(uuid),
                CreatedAt = createdAt,
                UpdatedAt = updatedAt,
                Vin1To10 = vin1To10,
                County = county,
                City = city,
                State = state,
                ZipCode = zipCode,
                Model = newModel!,
                LegislativeDistrict = legislativeDistrict,
                DolVehicleId = dolVehicleId,
                VehicleLocation = vehicleLocation,
                ElectricUtility = electricUtility,
                CensusTract2020 = censusTract2020,
                Counties = counties,
                Districts = districts,
                LegislativeDistrictBoundary = legislativeDistrictBoundary
            };

            // Add Car if unique
            if (!uniqueCars.TryGetValue(newCar.Uuid, out var curCar))
            {
                uniqueCars.Add(newCar.Uuid, newCar);                
            }
            else
            {
                newCar = curCar; // but this never gets used or updated so ...
            }
        }

        return (uniqueMakes.Values.ToList(),
                uniqueModels.Values.ToList(), 
                uniqueCars.Values.ToList());
    }

    private NpgsqlPoint? ParsePoint(string pointString)
    {
        // Use the NpgsqlPointJsonConverter logic to parse the POINT string
        if (pointString.StartsWith("POINT (") && pointString.EndsWith(")"))
        {
            var pointData = pointString[7..^1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (pointData.Length == 2 &&
                double.TryParse(pointData[0], out var x) &&
                double.TryParse(pointData[1], out var y))
            {
                return new NpgsqlPoint(x, y);
            }
        }

        throw new JsonException("Invalid POINT format.");
    }
}
