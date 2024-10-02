using System.Net;
using System.Net.Http.Json;
using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Customers.Api.Tests.Integration.CustomerControllers;



public class CreateCustomerControllerTests : IClassFixture<CustomerApiFactory>
{
    private readonly HttpClient _client;

    private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
        .RuleFor(x => x.Email, fake => fake.Person.Email)
        .RuleFor(x => x.FullName, fake => fake.Person.FullName)
        .RuleFor(x => x.DateOfBirth, fake => fake.Person.DateOfBirth.Date)
        .RuleFor(x => x.GitHubUsername, fake => CustomerApiFactory.ValidGithubUser);

    public CreateCustomerControllerTests(CustomerApiFactory apiFactory)
    {
        _client = apiFactory.CreateClient();
    }
    [Fact]
    public async Task Create_user_when_data_valid()
    {
        // Arrange
        var customer = _customerGenerator.Generate();

        // Act
        var response = await _client.PostAsJsonAsync("customers", customer);
        var customerResult = await response.Content.ReadFromJsonAsync<CustomerResponse>();

        // Assert
        customerResult.Should().BeEquivalentTo(customer, options => options
            .Including(x => x.Email)
            .Including(x => x.DateOfBirth.Date)
            .Including(x => x.GitHubUsername)
            .Including(x => x.FullName));

       // customerResult.Should().BeEquivalentTo(customer, options => options.ExcludingMissingMembers());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().Be($"http://localhost/customers/{customerResult!.Id}");
    }
    
    
    [Fact]
    public async Task Create_ReturnsValidationError_WhenEmailIsInvalid()
    {
        // Arrange
        const string invalidEmail = "dasdja9d3j";
        var customer = _customerGenerator.Clone()
            .RuleFor(x => x.Email, invalidEmail).Generate();

        // Act
        var response = await _client.PostAsJsonAsync("customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        error!.Status.Should().Be(400);
        error.Title.Should().Be("One or more validation errors occurred.");
        error.Errors["Email"][0].Should().Be($"{invalidEmail} is not a valid email address");
    }
    
    [Fact]
    public async Task Create_ReturnsValidationError_WhenGitHubUserDoestNotExist()
    {
        // Arrange
        const string invalidGitHubUser = "dasdja9d3j";
        var customer = _customerGenerator.Clone()
            .RuleFor(x => x.GitHubUsername, invalidGitHubUser).Generate();

        // Act
        var response = await _client.PostAsJsonAsync("customers", customer);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        error!.Status.Should().Be(400);
        error.Title.Should().Be("One or more validation errors occurred.");
        error.Errors["Customer"][0].Should().Be($"There is no GitHub user with username {invalidGitHubUser}");
    }


    [Fact]
    public async Task Create_returns_internal_server_error_when_github_is_throttled()
    {
        // Arrange
        var customer = _customerGenerator.Clone()
            .RuleFor(x=> x.GitHubUsername,CustomerApiFactory.ThrottledUser)
            .Generate();

        // Act
        var response = await _client.PostAsJsonAsync("customers", customer);

        // Assert

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

}