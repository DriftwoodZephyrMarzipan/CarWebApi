using CarWebApi.EvDataModels.DTOs;
using CarWebApi.EvDataModels.Models;
using System.Net;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
public class ModelTests : IDisposable
{
    private FunctionalTestServer _testServer;
    private HttpClient _client;

    private List<Make> madeMakes = new List<Make>();
    private List<Model> madeModels = new List<Model>();

    private Make defaultModelMake = new Make() { Manufacturer = "TestModelManufacturer" };

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _testServer = new FunctionalTestServer();
        madeMakes.Add(defaultModelMake);
        TestDataManager.Repository.CreateMakeAsync(defaultModelMake).Wait();
    }

    [SetUp]
    public void Setup()
    {
        _client = _testServer.CreateClient();
    }

    public void Dispose()
    {
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
        Model model1 = new Model()
        {
            Make = defaultModelMake,
            ModelName = "GetAllIdTestModel1",
            ModelYear = 2025,
            EvType = EvType.BEV,
            CafvType = CafvType.CleanAlternativeFuelVehicleEligible,
            ElectricRange = 300,
            BaseMSRP = 35000.00m
        },
        model2 = new Model()
        {
            Make = defaultModelMake,
            ModelName = "GetAllIdTestModel2",
            ModelYear = 2023,
            EvType = EvType.BEV,
            CafvType = CafvType.CleanAlternativeFuelVehicleEligible,
            ElectricRange = 100,
            BaseMSRP = 65000.00m
        };

        madeModels.Add(model1);
        madeModels.Add(model2);

        TestDataManager.Repository.CreateModelAsync(model1).Wait();
        TestDataManager.Repository.CreateModelAsync(model2).Wait();

        var result = await _client.GetAsync($"Models");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await result.Content.ReadFromJsonAsync<IdList>();

        Assert.That(data, Is.Not.Null);
        Assert.That(data.Ids.Count, Is.GreaterThanOrEqualTo(2));
        Assert.That(data.Ids.Contains(model1.Id), Is.True);
        Assert.That(data.Ids.Contains(model2.Id), Is.True);
    }

    [Test]
    public async Task GetAll()
    {
        Model model1 = new Model()
        {
            Make = defaultModelMake,
            ModelName = "GetAllTestModel1",
            ModelYear = 2025,
            EvType = EvType.BEV,
            CafvType = CafvType.CleanAlternativeFuelVehicleEligible,
            ElectricRange = 300,
            BaseMSRP = 35000.00m
        },
        model2 = new Model()
        {
            Make = defaultModelMake,
            ModelName = "GetAllTestModel2",
            ModelYear = 2023,
            EvType = EvType.BEV,
            CafvType = CafvType.CleanAlternativeFuelVehicleEligible,
            ElectricRange = 100,
            BaseMSRP = 65000.00m
        };

        madeModels.Add(model1);
        madeModels.Add(model2);

        TestDataManager.Repository.CreateModelAsync(model1).Wait();
        TestDataManager.Repository.CreateModelAsync(model2).Wait();

        var result = await _client.GetAsync($"Models/all");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await result.Content.ReadFromJsonAsync<List<Model>>();

        Assert.That(data, Is.Not.Null);
        Assert.That(data.Count, Is.GreaterThanOrEqualTo(2));

        var dataModel1 = data.Where(m => m.Id == model1.Id).FirstOrDefault()!;
        var dataModel2 = data.Where(m => m.Id == model2.Id).FirstOrDefault()!;

        AssertModelsMatch(dataModel1, model1);
        AssertModelsMatch(dataModel2, model2);
    }

    [Test]
    public async Task GetAllByMake()
    {
        Model model1 = new Model()
        {
            Make = defaultModelMake,
            ModelName = "GetAllByMakeTestModel1",
            ModelYear = 2025,
            EvType = EvType.BEV,
            CafvType = CafvType.CleanAlternativeFuelVehicleEligible,
            ElectricRange = 300,
            BaseMSRP = 35000.00m
        },
        model2 = new Model()
        {
            Make = defaultModelMake,
            ModelName = "GetAllByMakeTestModel2",
            ModelYear = 2023,
            EvType = EvType.BEV,
            CafvType = CafvType.CleanAlternativeFuelVehicleEligible,
            ElectricRange = 100,
            BaseMSRP = 65000.00m
        };

        madeModels.Add(model1);
        madeModels.Add(model2);

        TestDataManager.Repository.CreateModelAsync(model1).Wait();
        TestDataManager.Repository.CreateModelAsync(model2).Wait();

        var result = await _client.GetAsync($"Models/all/{defaultModelMake.Id}");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await result.Content.ReadFromJsonAsync<List<Model>>();

        Assert.That(data, Is.Not.Null);
        Assert.That(data.Count, Is.GreaterThanOrEqualTo(2));

        var dataModel1 = data.Where(m => m.Id == model1.Id).FirstOrDefault()!;
        var dataModel2 = data.Where(m => m.Id == model2.Id).FirstOrDefault()!;

        AssertModelsMatch(dataModel1, model1);
        AssertModelsMatch(dataModel2, model2);
    }

    [Test]
    public async Task GetOne()
    {
        var model1 = new Model()
        {
            Make = defaultModelMake,
            ModelName = "GetAllOneTestModel1",
            ModelYear = 2025,
            EvType = EvType.BEV,
            CafvType = CafvType.CleanAlternativeFuelVehicleEligible,
            ElectricRange = 300,
            BaseMSRP = 35000.00m
        };
        madeModels.Add(model1);

        TestDataManager.Repository.CreateModelAsync(model1).Wait();

        var result = await _client.GetAsync($"Models/{model1.Id}");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = (await result.Content.ReadFromJsonAsync<Model>())!;

        AssertModelsMatch(data, model1);
    }

    [Test]
    public async Task Create()
    {
        var model1 = new Model()
        {
            Make = defaultModelMake,
            ModelName = "CreateTestModel",
            ModelYear = 2025,
            EvType = EvType.BEV,
            CafvType = CafvType.CleanAlternativeFuelVehicleEligible,
            ElectricRange = 300,
            BaseMSRP = 35000.00m
        };
        madeModels.Add(model1);

        var result = await _client.PostAsJsonAsync($"Models", model1);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var data = await result.Content.ReadFromJsonAsync<Model>();
        Assert.That(data, Is.Not.Null);
        model1.Id = data.Id;

        var createdModel = await TestDataManager.Repository.GetModelByIdAsync(data.Id);

        AssertModelsMatch(createdModel, model1);
    }

    /// <summary>
    /// Only attempting a single update test; there is not enough time or purpose to
    /// write exhaustive tests.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task Update()
    {
        var model1 = new Model()
        {
            Make = defaultModelMake,
            ModelName = "UpdateTestModel",
            ModelYear = 2025,
            EvType = EvType.BEV,
            CafvType = CafvType.CleanAlternativeFuelVehicleEligible,
            ElectricRange = 300,
            BaseMSRP = 35000.00m
        };
        madeModels.Add(model1);

        TestDataManager.Repository.CreateModelAsync(model1).Wait();

        model1.ModelName = "UpdatedTestModel";

        var result = await _client.PutAsJsonAsync($"Models",model1);

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var createdModel = await TestDataManager.Repository.GetModelByIdAsync(model1.Id);

        AssertModelsMatch(createdModel, model1);
    }

    [Test]
    public async Task Delete()
    {
        var model1 = new Model()
        {
            Make = defaultModelMake,
            ModelName = "UpdateTestModel",
            ModelYear = 2025,
            EvType = EvType.BEV,
            CafvType = CafvType.CleanAlternativeFuelVehicleEligible,
            ElectricRange = 300,
            BaseMSRP = 35000.00m
        };
        madeModels.Add(model1);

        TestDataManager.Repository.CreateModelAsync(model1).Wait();

        var result = await _client.DeleteAsync($"Models/{model1.Id}");
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var deletedModel = await TestDataManager.Repository.GetModelByIdAsync(model1.Id);
        
        Assert.That(deletedModel, Is.Null);
    }

    private void AssertModelsMatch(Model model1, Model model2)
    {
        Assert.That(model1, Is.Not.Null);
        Assert.That(model2, Is.Not.Null);
        Assert.That(model1.Id, Is.EqualTo(model2.Id));
        Assert.That(model1.Make.Id, Is.EqualTo(model2.Make.Id));
        Assert.That(model1.ModelName, Is.EqualTo(model2.ModelName));
        Assert.That(model1.ModelYear, Is.EqualTo(model2.ModelYear));
        Assert.That(model1.EvType, Is.EqualTo(model2.EvType));
        Assert.That(model1.CafvType, Is.EqualTo(model2.CafvType));
        Assert.That(model1.ElectricRange, Is.EqualTo(model2.ElectricRange));
        Assert.That(model1.BaseMSRP, Is.EqualTo(model2.BaseMSRP));
    }
}
