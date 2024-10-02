using Customers.Api.Database;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Xunit;

namespace Customers.Api.Tests.Integration;

public class CustomerApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{

    public const string ValidGithubUser = "validuser";
    public const string ThrottledUser = "throttl";

    private static readonly TestcontainerDatabase _dbContainer =
        new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "db",
                Username = "waisa",
                Password = "Abc12345%"
            })
            .Build();

    private readonly GithubApiServer _githubApiServer = new();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IHostedService));

            services.RemoveAll(typeof(IDbConnectionFactory));
            services.AddSingleton<IDbConnectionFactory>(
                _ => new NpgsqlConnectionFactory(_dbContainer.ConnectionString));

            services.AddHttpClient("GitHub", httpClient =>
            {
                httpClient.BaseAddress = new Uri(_githubApiServer.Url);
                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.Accept, "application/vnd.github.v3+json");
                httpClient.DefaultRequestHeaders.Add(
                    HeaderNames.UserAgent, $"test-{Environment.MachineName}");
            });

            //services.RemoveAll(typeof(AppDbContext));
            //services.AddDbContext<AppDbContext>(
            //    optionsBuilder => optionsBuilder.UseSqlite(_dbContainer.ConnectionString));
        });
    }

    public async Task InitializeAsync()
    {
        _githubApiServer.Start();
        _githubApiServer.SetUpUser(ValidGithubUser);
        _githubApiServer.SetUpThrottledUser(ThrottledUser);
        await _dbContainer.StartAsync();


    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
        _githubApiServer.Dispose();
    }

}



public class AppDbContext : DbContext
{
    public AppDbContext()
    {
        
    }
}




//private readonly TestcontainersContainer _dbcontainer =
//    new TestcontainersBuilder<TestcontainersContainer>()
//        .WithImage("postgres:latest")
//        .WithEnvironment("POSTGRES_USER", "waisa")
//        .WithEnvironment("POSTGRES_PASSWORD", "changeme")
//        .WithEnvironment("POSTGRES_DB", "mydb")
//        .WithPortBinding("5432", "5432")
//        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
//        .Build();
