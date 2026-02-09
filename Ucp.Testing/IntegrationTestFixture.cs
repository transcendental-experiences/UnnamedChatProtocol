using Microsoft.AspNetCore.Mvc.Testing;
using Ucp.Server;

namespace Ucp.Testing;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class IntegrationTestFixture
{
    protected WebApplicationFactory<Program> Factory;
    
    [SetUp]
    public async Task Setup()
    {
        Factory = new WebApplicationFactory<Program>();
    }

    [TearDown]
    public async Task Teardown()
    {
        await Factory.DisposeAsync();
    }
}