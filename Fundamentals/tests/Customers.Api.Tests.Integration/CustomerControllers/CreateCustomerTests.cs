using System.Net.Http.Json;
using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Customers.Api.Tests.Integration.CustomerControllers;


[Collection("CustomerApi Collection")]
public class CreateCustomerTests : IAsyncLifetime //, IClassFixture<WebApplicationFactory<IApiMarker>>
{

    private readonly Faker<CustomerRequest> faker =
        new Faker<CustomerRequest>()
            .RuleFor(c => c.FullName, f => f.Name.FullName())
            .RuleFor(c => c.DateOfBirth, f => f.Date.Past(30))
            .RuleFor(c => c.Email, f => f.Internet.Email())
            .RuleFor(c => c.GitHubUsername, "alnaimi-github");

    private readonly HttpClient _httpClient;

    private readonly List<Guid> _createdIds = [];

    public CreateCustomerTests(WebApplicationFactory<IApiMarker> _applicationFactory)
    {
        _httpClient = _applicationFactory.CreateClient();
    }


    [Fact]
    public async Task PostCustomer()
    {
        var customer = faker.Generate();

        var response = await _httpClient.PostAsJsonAsync("customers", customer);

        // Ensure the response is successful
        response.EnsureSuccessStatusCode();

        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();

        customerResponse.Should().BeEquivalentTo(customer, options =>
            options.Using<DateTime>(ctx => ctx.Subject.Date.Should().Be(ctx.Expectation.Date))
                .WhenTypeIs<DateTime>());

        _createdIds.Add(customerResponse!.Id);

    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        foreach (var createdId in _createdIds)
        {
            await _httpClient.DeleteAsync($"customers/{createdId}");

        }
    }
}