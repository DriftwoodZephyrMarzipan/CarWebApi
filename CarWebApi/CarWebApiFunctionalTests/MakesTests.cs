using CarWebApi.EvDataModels.DTOs;
using CarWebApi.EvDataModels.Models;
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
public class MakesTests : IDisposable
{
    private FunctionalTestServer _testServer;
    private HttpClient _client;

    private List<Make> madeMakes = new List<Make>();

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _testServer = new FunctionalTestServer();
    }

    [SetUp]
    public void Setup()
    {
        _client = _testServer.CreateClient();
    }

    public void Dispose()
    {
        foreach(var make in madeMakes)
        {
            TestDataManager.Repository.DeleteMakeAsync(make.Id).Wait();
        }
        _testServer?.Dispose();
    }        

    [Test]
    public async Task GetAllId()
    {
        Make make1 = new Make() { Manufacturer = "GetAllIdTestManufacturer" },
             make2 = new Make() { Manufacturer = "GetAllIdTestManufacturer2" };
        madeMakes.Add(make1);
        madeMakes.Add(make2);

        TestDataManager.Repository.CreateMakeAsync(make1).Wait();
        TestDataManager.Repository.CreateMakeAsync(make2).Wait();

        var result = await _client.GetAsync($"Makes");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await result.Content.ReadFromJsonAsync<IdList>();

        Assert.That(data, Is.Not.Null);
        Assert.That(data.Ids.Count, Is.GreaterThanOrEqualTo(2));
        Assert.That(data.Ids.Contains(make1.Id), Is.True);
        Assert.That(data.Ids.Contains(make2.Id), Is.True);
    }

    [Test]
    public async Task GetOne()
    {
        Make make1 = new Make() { Manufacturer = "GetOneManufacturer" };
        madeMakes.Add(make1);

        TestDataManager.Repository.CreateMakeAsync(make1).Wait();

        var result = await _client.GetAsync($"Makes/{make1.Id}");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await result.Content.ReadFromJsonAsync<Make>();

        Assert.That(data, Is.Not.Null);
        Assert.That(data.Id, Is.EqualTo(make1.Id));    
        Assert.That(data.Manufacturer, Is.EqualTo(make1.Manufacturer));
    }

    [Test]
    public async Task GetAll()
    {
        Make make1 = new Make() { Manufacturer = "GetAllTestManufacturer" };
        madeMakes.Add(make1);

        TestDataManager.Repository.CreateMakeAsync(make1).Wait();

        var result = await _client.GetAsync($"Makes/all");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await result.Content.ReadFromJsonAsync<List<Make>>();

        Assert.That(data, Is.Not.Null);
        Assert.That(data.Count, Is.GreaterThanOrEqualTo(1));
        Assert.That(data.Any(d => d.Id == make1.Id && d.Manufacturer == make1.Manufacturer), Is.True);
    }

    [Test]
    public async Task Create()
    {
        Make make1 = new Make() { Manufacturer = "CreateTestManufacturer" };
        madeMakes.Add(make1);

        var result = await _client.PostAsJsonAsync($"Makes",make1);
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        var data = await result.Content.ReadFromJsonAsync<Make>();
        Assert.That(data, Is.Not.Null);
        make1.Id = data.Id;

        var createdMake = await TestDataManager.Repository.GetMakeByIdAsync(data.Id);

        Assert.That(createdMake, Is.Not.Null);
        Assert.That(createdMake.Manufacturer, Is.EqualTo(data.Manufacturer));
    }

    [Test]
    public async Task Update()
    {
        Make make1 = new Make() { Manufacturer = "UpdateTestManufacturer" };
        madeMakes.Add(make1);

        TestDataManager.Repository.CreateMakeAsync(make1).Wait();

        make1.Manufacturer = "UpdatedTestManufacturer";

        var result = await _client.PutAsJsonAsync($"Makes",make1);

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var createdMake = await TestDataManager.Repository.GetMakeByIdAsync(make1.Id);

        Assert.That(createdMake, Is.Not.Null);
        Assert.That(createdMake.Manufacturer, Is.EqualTo(make1.Manufacturer));
    }

    [Test]
    public async Task Delete()
    {
        Make make1 = new Make() { Manufacturer = "DeleteTestManufacturer" };
        madeMakes.Add(make1);
        TestDataManager.Repository.CreateMakeAsync(make1).Wait();

        var result = await _client.DeleteAsync($"Makes/{make1.Id}");
        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var deletedMake = await TestDataManager.Repository.GetMakeByIdAsync(make1.Id);
        
        Assert.That(deletedMake, Is.Null);
    }
}
