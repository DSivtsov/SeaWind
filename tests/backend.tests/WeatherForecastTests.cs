using System;
using Xunit;
using backend;

namespace backend.tests;

public class WeatherForecastTests
{
    [Theory]
    [InlineData(0, 32)]
    [InlineData(25, 77)]
    [InlineData(30, 86)]
    public void TemperatureF_IsCalculated_From_TemperatureC(int c, int expected)
    {
        var wf = new WeatherForecast(DateOnly.FromDateTime(DateTime.UtcNow), c, "Any");
        Assert.InRange(wf.TemperatureF, expected - 1, expected + 1);
    }
}
