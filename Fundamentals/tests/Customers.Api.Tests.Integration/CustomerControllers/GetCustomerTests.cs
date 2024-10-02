using System.Net.Http;
using System.Net;
using FluentAssertions;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Customers.Api.Tests.Integration.CustomerControllers;

[Collection("CustomerApi Collection")]
public class GetCustomerTests //: IClassFixture<WebApplicationFactory<IApiMarker>>
{
    private readonly HttpClient _httpClient;


    public GetCustomerTests(WebApplicationFactory<IApiMarker> _applicationFactory)
    {
        _httpClient = _applicationFactory.CreateClient();
    }


    [Fact]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        // Act
        var response = await _httpClient.GetAsync($"customers/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        //var text = await response.Content.ReadAsStringAsync();
        //text.Should().Contain("404");

        //var R = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        //R.Status.Should().Be(404);
        //R.Title.Should().Be("Not Found");
        // response.Headers.Location!.ToString().Should().Be("");
    }
}