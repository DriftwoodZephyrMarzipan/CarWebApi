using CarWebApi.EvDataModels.Models;
using CarWebApi.EvDataModels.DTOs;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;
using CarWebApi.Services;
using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;

namespace CarWebApi.Data.Repositories;

public class PostGreSQLCarRepository : ICarRepository
{
    private readonly string _connectionString;
    private readonly IDatabaseTelemetryService _telemetryService = null!;

    public PostGreSQLCarRepository(string connectionString, IDatabaseTelemetryService telemetryService = null)
    {
        _connectionString = connectionString;
        _telemetryService = telemetryService;
        if(_telemetryService == null)
        {
            // fine, just ignore it.
            _telemetryService = new DatabaseTelemetryService(new NullLogger<DatabaseTelemetryService>());
        }
    }

    // Could use IOptionsMonitor pattern, but I don't foresee this changing in real time
    // much.
    public PostGreSQLCarRepository(IOptions<ConnectionStrings> dbSettings, IDatabaseTelemetryService telemetryService)
    {
        _connectionString = dbSettings.Value.DefaultConnection;
        _telemetryService = telemetryService;
    }

    public async Task<Make> GetMakeByIdAsync(int id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        Make make = null!;
        using (var command = new NpgsqlCommand("SELECT * FROM evs.makes WHERE id = @id", connection))
        {
            command.Parameters.AddWithValue("id", id);

            var stopwatch = Stopwatch.StartNew();

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    make = new Make
                    {
                        Id = reader.GetInt32(0),
                        Manufacturer = reader.GetString(1)
                    };
                }
            }
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);

        }
        return make;
    }

    public async Task<IdList> GetMakeIdListAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var idList = new IdList() { DataType = "make" };
        using (var command = new NpgsqlCommand("SELECT id FROM evs.makes", connection))
        {
            var stopwatch = Stopwatch.StartNew();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var makeId = reader.GetInt32(0);
                    idList.Ids.Add(makeId);
                }
            }
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
        }
        return idList;
    }

    public async Task<List<Make>> GetAllMakesAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var makes = new List<Make>();
        using (var command = new NpgsqlCommand("SELECT * FROM evs.makes", connection))
        {
            var stopwatch = Stopwatch.StartNew();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var make = new Make
                    {
                        Id = reader.GetInt32(0),
                        Manufacturer = reader.GetString(1)
                    };
                    makes.Add(make);
                }
            }
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
        }
        return makes;
    }

    public async Task<bool> CreateMakeAsync(Make make)
    {
        if(make.Id != 0)
        {
            throw new ArgumentException("Cannot create a Make with a non-0 Id");
        }
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        using (var command = new NpgsqlCommand($"INSERT INTO evs.makes (manufacturer) VALUES (@name) RETURNING id", connection))
        {
            command.Parameters.AddWithValue("name", make.Manufacturer);

            // Use ExecuteScalarAsync to fetch the returned ID
            var result = await command.ExecuteScalarAsync();

            var stopwatch = Stopwatch.StartNew();
            if (result != null && int.TryParse(result.ToString(), out var id))
            {
                make.Id = id; // Set the ID on the Make object
                return true;
            }
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
            return false; // Return false if the ID could not be retrieved
        }
    }

    public async Task<bool> UpdateMakeAsync(Make make)
    {
        if(make.Id == 0)
        {
            throw new ArgumentException("Cannot update a Make with a 0 Id");
        }

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        using (var command = new NpgsqlCommand("UPDATE evs.makes SET manufacturer = @name WHERE id = @id", connection))
        {
            command.Parameters.AddWithValue("name", make.Manufacturer);
            command.Parameters.AddWithValue("id", make.Id);

            var stopwatch = Stopwatch.StartNew();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
            return rowsAffected > 0;
        }
    }

    public async Task<bool> DeleteMakeAsync(int id)
    {
        if (id == 0)
        {
            throw new ArgumentException("Cannot delete a Make with a 0 Id");
        }

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        using (var command = new NpgsqlCommand("delete from evs.makes WHERE id = @id", connection))
        {
            command.Parameters.AddWithValue("id", id);
            var stopwatch = Stopwatch.StartNew();
            var rowsAffected = await command.ExecuteNonQueryAsync(); 
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
            return rowsAffected > 0;
        }
    }

    public async Task<Model> GetModelByIdAsync(int modelId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        Model model = null!;
        using (var command = new NpgsqlCommand("SELECT * FROM evs.models WHERE id = @modelId", connection))
        {
            command.Parameters.AddWithValue("modelId", modelId);
            var stopwatch = Stopwatch.StartNew();
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    model = new Model
                    {
                        Id = reader.GetInt32(0),
                        Make = await GetMakeByIdAsync(reader.GetInt32(1)),  // more efficient to have joined
                        ModelName = reader.GetString(2),
                        ModelYear = reader.GetInt32(3),
                        EvType = (EvType)reader.GetInt32(4),
                        CafvType = (CafvType)reader.GetInt32(5),
                        ElectricRange = reader.GetInt32(6),
                        BaseMSRP = reader.GetDecimal(7)
                    };
                }
            }
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
        }
        return model;
    }

    public async Task<IdList> GetModelIdListAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var idList = new IdList() { DataType = "model" };
        using (var command = new NpgsqlCommand("SELECT id FROM evs.models", connection))
        {
            var stopwatch = Stopwatch.StartNew();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var modelId = reader.GetInt32(0);
                    idList.Ids.Add(modelId);
                }
            }
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
        }
        return idList;
    }

    public async Task<List<Model>> GetAllModelsAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var models = new List<Model>();
        using (var command = new NpgsqlCommand("SELECT * FROM evs.models", connection))
        {
            var stopwatch = Stopwatch.StartNew();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var model = new Model
                    {
                        Id = reader.GetInt32(0),
                        Make = await GetMakeByIdAsync(reader.GetInt32(1)),  // more efficient to have joined
                        ModelName = reader.GetString(2),
                        ModelYear = reader.GetInt32(3),
                        EvType = (EvType)reader.GetInt32(4),
                        CafvType = (CafvType)reader.GetInt32(5),
                        ElectricRange = reader.GetInt32(6),
                        BaseMSRP = reader.GetDecimal(7)
                    };
                    models.Add(model);
                }
            }
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
        }
        return models;
    }

    public async Task<List<Model>> GetModelsByMakeIdAsync(int make_id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var models = new List<Model>();
        using (var command = new NpgsqlCommand("SELECT * FROM evs.models join evs.makes on makes.id = models.make_id WHERE make_id = @makeId", connection))
        {
            command.Parameters.AddWithValue("makeId", make_id); 
            var stopwatch = Stopwatch.StartNew();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var model = new Model
                    {
                        Id = reader.GetInt32(0),
                        Make = await GetMakeByIdAsync(reader.GetInt32(1)),  // more efficient to have joined
                        ModelName = reader.GetString(2),
                        ModelYear = reader.GetInt32(3),
                        EvType = (EvType)reader.GetInt32(4),
                        CafvType = (CafvType)reader.GetInt32(5),
                        ElectricRange = reader.GetInt32(6),
                        BaseMSRP = reader.GetDecimal(7)
                    };
                    models.Add(model);
                }
            }
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
        }
        return models;
    }

    public async Task<bool> CreateModelAsync(Model model)
    {
        if (model.Id != 0)
        {
            throw new ArgumentException("Cannot create a Model with a non-0 Id");
        }

        if (model.Make.Id == 0)
        {
            await CreateMakeAsync(model.Make);
        }
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        using (var command = new NpgsqlCommand("INSERT INTO evs.models (make_id, model_name, model_year, ev_type, cafv_type, electric_range, base_msrp) " +
                                                     "VALUES (@makeId, @modelName, @modelYear, @evType, @cafvType, @electricRange, @baseMsrp) RETURNING id", connection))
        {
            command.Parameters.AddWithValue("makeId", model.Make.Id);
            command.Parameters.AddWithValue("modelName", model.ModelName);
            command.Parameters.AddWithValue("modelYear", model.ModelYear);
            command.Parameters.AddWithValue("evType", (int)model.EvType);
            command.Parameters.AddWithValue("cafvType", (int)model.CafvType);
            command.Parameters.AddWithValue("electricRange", model.ElectricRange);
            command.Parameters.AddWithValue("baseMsrp", model.BaseMSRP);
            var stopwatch = Stopwatch.StartNew();
            // Use ExecuteScalarAsync to fetch the returned ID
            var result = await command.ExecuteScalarAsync(); 
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
            if (result != null && int.TryParse(result.ToString(), out var id))
            {
                model.Id = id; // Set the ID on the Model object
                return true;
            }
            return false; // Return false if the ID could not be retrieved
        }
    }

    public async Task<bool> UpdateModelAsync(Model model)
    {
        if (model.Id == 0)
        {
            throw new ArgumentException("Cannot update a Model with a 0 Id");
        }

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        using (var command = new NpgsqlCommand("UPDATE evs.models SET make_id = @makeId, model_name = @modelName, model_year = @modelYear, " +
                                                                 "ev_type = @evType, cafv_type = @cafvType, electric_range = @electricRange, " +
                                                                 "base_msrp = @baseMsrp WHERE id = @id", connection))
        {
            command.Parameters.AddWithValue("makeId", model.Make.Id);
            command.Parameters.AddWithValue("modelName", model.ModelName);
            command.Parameters.AddWithValue("modelYear", model.ModelYear);
            command.Parameters.AddWithValue("evType", (int)model.EvType);
            command.Parameters.AddWithValue("cafvType", (int)model.CafvType);
            command.Parameters.AddWithValue("electricRange", model.ElectricRange);
            command.Parameters.AddWithValue("baseMsrp", model.BaseMSRP);
            command.Parameters.AddWithValue("id", model.Id);
            var stopwatch = Stopwatch.StartNew();
            var rowsAffected = await command.ExecuteNonQueryAsync(); 
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
            return rowsAffected > 0;
        }
    }

    public async Task<bool> DeleteModelAsync(int id)
    {
        if (id == 0)
        {
            throw new ArgumentException("Cannot delete a Model with a 0 Id");
        }

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        using (var command = new NpgsqlCommand("delete from evs.models WHERE id = @id", connection))
        {
            command.Parameters.AddWithValue("id", id);
            var stopwatch = Stopwatch.StartNew();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
            return rowsAffected > 0;
        }
    }

    public async Task<Car> GetCarByIdAsync(int carId)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        Car car = null!;
        using (var command = new NpgsqlCommand("SELECT * FROM evs.cars WHERE id = @carId", connection))
        {
            command.Parameters.AddWithValue("carId", carId);
            var stopwatch = Stopwatch.StartNew();
            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    car = new Car
                    {
                        Id = reader.GetInt32(0),
                        Uuid = reader.GetGuid(1),
                        Sid = reader.GetString(2),
                        CreatedAt = reader.GetFieldValue<DateTime>(3).ToUniversalTime(), // Ensure UTC
                        UpdatedAt = reader.IsDBNull(4) ? null : reader.GetFieldValue<DateTime>(4).ToUniversalTime(), // Handle nulls and ensure UTC
                        Vin1To10 = reader.GetString(5),
                        County = reader.GetString(6),
                        City = reader.GetString(7),
                        State = reader.GetString(8),
                        ZipCode = reader.GetString(9),
                        Model = await GetModelByIdAsync(reader.GetInt32(10)),  // not super efficient, but the code is faster to write
                        LegislativeDistrict = reader.GetString(11),
                        DolVehicleId = reader.GetString(12),
                        VehicleLocation = reader.IsDBNull(13) ? null : reader.GetFieldValue<NpgsqlPoint>(13),
                        ElectricUtility = reader.GetString(14),
                        CensusTract2020 = reader.GetString(15),
                        Counties = reader.GetInt64(16),
                        Districts = reader.GetInt64(17),
                        LegislativeDistrictBoundary = reader.GetInt64(18),

                    };
                }
            }
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
        }
        return car;
    }

    public async Task<IdList> GetCarIdListAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var idList = new IdList() { DataType = "car" };
        using (var command = new NpgsqlCommand("SELECT id FROM evs.cars", connection))
        {
            var stopwatch = Stopwatch.StartNew();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var modelId = reader.GetInt32(0);
                    idList.Ids.Add(modelId);
                }
            }
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
        }
        return idList;
    }

    public async Task<bool> CreateCarAsync(Car car)
    {
        if (car.Id != 0)
        {
            throw new ArgumentException("Cannot create a Car with a non-0 Id");
        }

        if (car.Model.Id == 0)
        {
            await CreateModelAsync(car.Model);
        }

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        using (var command = new NpgsqlCommand("INSERT INTO evs.cars (uuid, sid, created_at, updated_at, vin_1_10, county, city, state, " +
                                                                   "zip_code, model_id, legislative_district, dol_vehicle_id, vehicle_location, " +
                                                                   "electric_utility, census_tract_2020, counties, districts, legislative_district_boundary) " +
                                                           "VALUES (@uuid, @sid, @created_at, @updated_at, @vin_1_10, @county, @city, @state, " +
                                                                   "@zip_code, @model_id, @legislative_district, @dol_vehicle_id, @vehicle_location, " +
                                                                   "@electric_utility, @census_tract_2020, @counties, @districts, @legislative_district_boundary) " +
                                                                   "RETURNING id", connection))
        {
            command.Parameters.AddWithValue("uuid", car.Uuid);
            command.Parameters.AddWithValue("sid", car.Sid);
            command.Parameters.AddWithValue("created_at", car.CreatedAt);
            command.Parameters.AddWithValue("updated_at", car.UpdatedAt.HasValue ? car.UpdatedAt.Value : DBNull.Value);
            command.Parameters.AddWithValue("vin_1_10", car.Vin1To10);
            command.Parameters.AddWithValue("county", car.County ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("city", car.City ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("state", car.State ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("zip_code", car.ZipCode ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("model_id", car.Model.Id);
            command.Parameters.AddWithValue("legislative_district", car.LegislativeDistrict ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("dol_vehicle_id", car.DolVehicleId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("vehicle_location", car.VehicleLocation ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("electric_utility", car.ElectricUtility ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("census_tract_2020", car.CensusTract2020 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("counties", car.Counties.HasValue ? car.Counties.Value : DBNull.Value);
            command.Parameters.AddWithValue("districts", car.Districts.HasValue ? car.Districts.Value : DBNull.Value);
            command.Parameters.AddWithValue("legislative_district_boundary", car.LegislativeDistrictBoundary.HasValue ? car.LegislativeDistrictBoundary.Value : DBNull.Value);

            // Use ExecuteScalarAsync to fetch the returned ID
            var stopwatch = Stopwatch.StartNew();
            var result = await command.ExecuteScalarAsync();
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
            if (result != null && int.TryParse(result.ToString(), out var id))
            {
                car.Id = id; // Set the ID on the Model object
                return true;
            }
            return false; // Return false if the ID could not be retrieved
        }
    }

    public async Task<bool> UpdateCarAsync(Car car)
    {
        if (car.Id == 0)
        {
            throw new ArgumentException("Cannot update a Car with a 0 Id");
        }

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        using (var command = new NpgsqlCommand("UPDATE evs.cars SET uuid = @uuid, sid = @sid, created_at = @created_at, " +
                                                               "updated_at = @updated_at, vin_1_10 = @vin_1_10, county = @county, " +
                                                               "city = @city, state = @state, zip_code = @zip_code, model_id = @model_id, " +
                                                               "legislative_district = @legislative_district, dol_vehicle_id = @dol_vehicle_id, " +
                                                               "vehicle_location = @vehicle_location, electric_utility = @electric_utility, " +
                                                               "census_tract_2020 = @census_tract_2020, counties = @counties, districts = @districts, " +
                                                               "legislative_district_boundary = @legislative_district_boundary " +
                                                               "WHERE id = @id", connection))
        {
            command.Parameters.AddWithValue("uuid", car.Uuid);
            command.Parameters.AddWithValue("sid", car.Sid);
            command.Parameters.AddWithValue("created_at", car.CreatedAt);
            command.Parameters.AddWithValue("updated_at", car.UpdatedAt.HasValue ? car.UpdatedAt.Value : DBNull.Value);
            command.Parameters.AddWithValue("vin_1_10", car.Vin1To10);
            command.Parameters.AddWithValue("county", car.County ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("city", car.City ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("state", car.State ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("zip_code", car.ZipCode ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("model_id", car.Model.Id);
            command.Parameters.AddWithValue("legislative_district", car.LegislativeDistrict ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("dol_vehicle_id", car.DolVehicleId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("vehicle_location", car.VehicleLocation ?? (object)DBNull.Value );
            command.Parameters.AddWithValue("electric_utility", car.ElectricUtility ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("census_tract_2020", car.CensusTract2020 ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("counties", car.Counties.HasValue ? car.Counties.Value : DBNull.Value);
            command.Parameters.AddWithValue("districts", car.Districts.HasValue ? car.Districts.Value : DBNull.Value);
            command.Parameters.AddWithValue("legislative_district_boundary", car.LegislativeDistrictBoundary.HasValue ? car.LegislativeDistrictBoundary.Value : DBNull.Value);
            command.Parameters.AddWithValue("id", car.Id);

            var stopwatch = Stopwatch.StartNew();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
            return rowsAffected > 0;
        }
    }

    public async Task<bool> DeleteCarAsync(int id)
    {
        if (id == 0)
        {
            throw new ArgumentException("Cannot delete a Car with a 0 Id");
        }

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        using (var command = new NpgsqlCommand("delete from evs.cars WHERE id = @id", connection))
        {
            command.Parameters.AddWithValue("id", id);
            var stopwatch = Stopwatch.StartNew();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            stopwatch.Stop();
            _telemetryService.LogQuery(command.CommandText, stopwatch.ElapsedMilliseconds);
            return rowsAffected > 0;
        }
    }
}
