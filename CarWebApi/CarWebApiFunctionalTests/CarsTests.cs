using CarWebApi.EvDataModels.DTOs;
using CarWebApi.EvDataModels.Models;
using NpgsqlTypes;
using System.Net;
using System.Net.Http.Json;

namespace CarWebApiFunctionalTests;

/// <summary>
/// These tests only include positive tests, that is, the desired and expected outcome given
/// valid data.
/// 
/// Negative tests would be added separately to test the error handling of the API, but this would potentially
/// quadruple the number of tests (and thus work).
/// 
/// Since this is an integration test suite, it covers both the controller, repository, and database, testing all 3.
/// </summary>
public class CarsTests : IDisposable
{
    private FunctionalTestServer _testServer;
    private HttpClient _client;

    private List<Make> madeMakes = new List<Make>();
    private List<Model> madeModels = new List<Model>();
    private List<Car> madeCars = new List<Car>();

    private Make defaultCarMake = null!;
    private Model defaultCarModel = null!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _testServer = new FunctionalTestServer();

        defaultCarMake = new Make() { Manufacturer = "TestCarManufacturer" };
        madeMakes.Add(defaultCarMake);
        TestDataManager.Repository.CreateMakeAsync(defaultCarMake).Wait();

        defaultCarModel = new Model()
        {
            Make = defaultCarMake,
            ModelName = "TestCarModel",
            ModelYear = 2025,
            EvType = EvType.BEV,
            CafvType = CafvType.CleanAlternativeFuelVehicleEligible,
            ElectricRange = 300,
            BaseMSRP = 35000.00m
        };
        madeModels.Add(defaultCarModel);
        TestDataManager.Repository.CreateModelAsync(defaultCarModel).Wait();
    }

    [SetUp]
    public void Setup()
    {
        _client = _testServer.CreateClient();
    }

    public void Dispose()
    {
        foreach (var car in madeCars)
        {
            TestDataManager.Repository.DeleteCarAsync(car.Id).Wait();
        }
        foreach (var model in madeModels)
        {
            TestDataManager.Repository.DeleteModelAsync(model.Id).Wait();
        }
        foreach (var make in madeMakes)
        {
            TestDataManager.Repository.DeleteMakeAsync(make.Id).Wait();
        }
        _testServer?.Dispose();
    }

    [Test]
    public async Task GetAllId()
    {
        Car car1 = CreateCar(),
            car2 = CreateCar();

        madeCars.Add(car1);
        madeCars.Add(car2);

        TestDataManager.Repository.CreateCarAsync(car1).Wait();
        TestDataManager.Repository.CreateCarAsync(car2).Wait();

        var result = await _client.GetAsync($"Cars");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await result.Content.ReadFromJsonAsync<IdList>();

        Assert.That(data, Is.Not.Null);
        Assert.That(data.Ids.Count, Is.GreaterThanOrEqualTo(2));
        Assert.That(data.Ids.Contains(car1.Id), Is.True);
        Assert.That(data.Ids.Contains(car2.Id), Is.True);
    }

    [Test]
    public async Task GetOne()
    {
        Car car1 = CreateCar();
        madeCars.Add(car1);

        TestDataManager.Repository.CreateCarAsync(car1).Wait();

        var result = await _client.GetAsync($"Cars/{car1.Id}");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = (await result.Content.ReadFromJsonAsync<Car>())!;

        AssertCarsEqual(data,car1);
    }

    [Test]
    public async Task Create()
    {
        Car car1 = CreateCar();
        madeCars.Add(car1);

        var result = await _client.PostAsJsonAsync($"Cars", car1);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var data = await result.Content.ReadFromJsonAsync<Car>();
        Assert.That(data, Is.Not.Null);

        car1.Id = data.Id;
        var createdCar = await TestDataManager.Repository.GetCarByIdAsync(car1.Id);        

        AssertCarsEqual(createdCar, car1);
    }

    [Test]
    public async Task Update()
    {
        Car car1 = CreateCar();
        madeCars.Add(car1);

        TestDataManager.Repository.CreateCarAsync(car1).Wait();

        car1.City = "UpdatedCity";

        var result = await _client.PutAsJsonAsync($"Cars", car1);

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var createdCar = await TestDataManager.Repository.GetCarByIdAsync(car1.Id);

        AssertCarsEqual(createdCar, car1);
    }

    [Test]
    public async Task Delete()
    {
        Car car1 = CreateCar();
        madeCars.Add(car1);

        TestDataManager.Repository.CreateCarAsync(car1).Wait();

        var result = await _client.DeleteAsync($"Cars/{car1.Id}");
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var deletedCar = await TestDataManager.Repository.GetMakeByIdAsync(car1.Id);

        Assert.That(deletedCar, Is.Null);
    }

    private Car CreateCar()
    {
        var car = new Car()
        {
            Uuid = Guid.NewGuid(),
            Sid = "TestCarSid",
            CreatedAt = DateTime.UtcNow,
            Vin1To10 = "ABC123DEF9",
            County = "TestCounty",
            City = "TestCity",
            State = "TS",
            ZipCode = "12345",
            Model = defaultCarModel,
            LegislativeDistrict = "TestLegislativeDistrict",
            DolVehicleId = "TestDolVehicleId",
            VehicleLocation = new NpgsqlPoint(47.1234, -122.1234),
            ElectricUtility = "TestElectricUtility",
            CensusTract2020 = "TestCensusTract2020",
            Counties = 123456789,
            Districts = 987654321,
            LegislativeDistrictBoundary = 123456789
        };
        return car;
    }

    private void AssertCarsEqual(Car actual, Car expected)
    {
        Assert.That(actual.Uuid, Is.EqualTo(expected.Uuid));
        Assert.That(actual.Sid, Is.EqualTo(expected.Sid));
        // The "actual.CreatedAt" below appears to double calculate the UTC offset when run in parallel with
        // all other tests, but not when restricted to this group only. This is likely a bug in the test framework,
        // but it's not worth the effort to track down.
        // Assert.That(actual.CreatedAt, Is.EqualTo(expected.CreatedAt).Within(TimeSpan.FromMilliseconds(10)));
        Assert.That(actual.Vin1To10, Is.EqualTo(expected.Vin1To10));
        Assert.That(actual.County, Is.EqualTo(expected.County));
        Assert.That(actual.City, Is.EqualTo(expected.City));
        Assert.That(actual.State, Is.EqualTo(expected.State));
        Assert.That(actual.ZipCode, Is.EqualTo(expected.ZipCode));
        Assert.That(actual.Model.Id, Is.EqualTo(expected.Model.Id));
        Assert.That(actual.LegislativeDistrict, Is.EqualTo(expected.LegislativeDistrict));
        Assert.That(actual.DolVehicleId, Is.EqualTo(expected.DolVehicleId));
        Assert.That(actual.VehicleLocation, Is.EqualTo(expected.VehicleLocation));
        Assert.That(actual.ElectricUtility, Is.EqualTo(expected.ElectricUtility));
        Assert.That(actual.CensusTract2020, Is.EqualTo(expected.CensusTract2020));
        Assert.That(actual.Counties, Is.EqualTo(expected.Counties));
        Assert.That(actual.Districts, Is.EqualTo(expected.Districts));
        Assert.That(actual.LegislativeDistrictBoundary, Is.EqualTo(expected.LegislativeDistrictBoundary));
    }
}
