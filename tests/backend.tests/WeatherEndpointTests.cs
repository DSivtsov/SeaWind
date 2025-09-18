using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace backend.tests;

public class WeatherEndpointTests : IClassFixture<WebApplicationFactory<backend.Program>>
{
    private readonly HttpClient _client;

    public WeatherEndpointTests(WebApplicationFactory<backend.Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task WeatherForecast_Returns_Ok_And_Array()
    {
        var resp = await _client.GetAsync("/weatherforecast");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var payload = await resp.Content.ReadFromJsonAsync<object[]>();
        Assert.NotNull(payload);
        Assert.NotEmpty(payload!);
    }
}