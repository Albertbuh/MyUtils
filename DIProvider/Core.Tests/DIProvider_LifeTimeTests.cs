namespace Core.Tests;

[TestClass]
public class DIProvider_LifeTimeTests
{
    DependencyProvider _provider;

    public DIProvider_LifeTimeTests()
    {
        var config = new DependenciesConfiguration();
        config.RegisterTransient<Service3, Service3>();
        config.RegisterSingleton<SingletonService, SingletonService>();

        _provider = new DependencyProvider(config);
    }

    [TestMethod]
    public void CheckTransientMethodsForDifferentValues()
    {
        var service1 = _provider.Resolve<Service3>();
        var service2 = _provider.Resolve<Service3>();

        Assert.AreNotEqual(service1.GetSomeNumber(), service2.GetSomeNumber());
    }

    [TestMethod]
    public void CheckSingletonMethodsForDifferentValues()
    {
        var service1 = _provider.Resolve<SingletonService>();
        var service2 = _provider.Resolve<SingletonService>();

        Assert.AreEqual(service1.GetSomeNumber(), service2.GetSomeNumber());
    }
}
