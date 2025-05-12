using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CarWebApiFunctionalTests;

public class FunctionalTestServer : WebApplicationFactory<CarWebApi.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration((host, configurationBuilder) => { 
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddLogging(builder => builder.ClearProviders().AddConsole().AddDebug());
        });
    }
}

