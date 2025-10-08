using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace Api.Tests.Weather
{
    public class WeatherExceptionTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public WeatherExceptionTests(WebApplicationFactory<Program> factory)
            => _client = factory.CreateClient();

        [Fact]
        public async Task WeatherForecast_Returns_Ok_And_Array()
        {
            var resp = await _client.GetAsync("/api/weatherexception/get/1");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            resp = await _client.GetAsync("/api/weatherexception/get/-1");
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

            resp = await _client.GetAsync("/api/unknown/get");
            Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        }
    }
}
