using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Customers.Api.Tests.Integration.CustomerControllers;

[CollectionDefinition("CustomerApi Collection")]
public class TestCollection : ICollectionFixture<WebApplicationFactory<IApiMarker>>
{
}