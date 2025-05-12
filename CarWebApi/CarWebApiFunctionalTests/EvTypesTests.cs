using CarWebApi.EvDataModels.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace CarWebApiFunctionalTests;

public class EvTypesTests : IDisposable
{
    private FunctionalTestServer _testServer;
    private HttpClient _client;

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
        _testServer?.Dispose();
    }        

    [Test]
    public async Task GetAll()
    {
        var result = await _client.GetAsync($"EvTypes");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await result.Content.ReadFromJsonAsync<List<EnumIdentifier>>();

        Assert.That(data, Is.Not.Null);
        Assert.That(data.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task GetOne()
    {
        var result = await _client.GetAsync($"EvTypes/1");

        Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var data = await result.Content.ReadFromJsonAsync<EnumIdentifier>();

        Assert.That(data, Is.Not.Null);
        Assert.That(data.Id, Is.EqualTo(1));    
        Assert.That(data.Description, Is.EqualTo("Plug-in Hybrid Electric Vehicle (PHEV)"));
    }
}
