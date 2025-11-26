using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.UnitTests.Integration.Shared;

public class TestMainDbFixture : IDisposable
{
    public readonly HttpClient Client;
    private readonly WebApplicationFactory<Program> _factory;

    public TestMainDbFixture()
    {
        _factory = new TestMainDbWebApplicationFactory();
        Client = _factory.CreateClient();
    }

    public void Dispose()
    {
        Client.Dispose();
        _factory.Dispose();
    }
}
