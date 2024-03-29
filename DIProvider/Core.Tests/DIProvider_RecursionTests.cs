namespace Core.Tests;

[TestClass]
public class DIProvider_RecursionTests
{
    DependencyProvider _provider;

    public DIProvider_RecursionTests()
    {
        var config = new DependenciesConfiguration();
        config.Register<IService1, Service1>();
        config.Register<AbstractService2, WithRecursionService2>();

        config.Register<WithRecursionService3, WithRecursionService3>();
        _provider = new DependencyProvider(config);
    }

    [TestMethod]
    public void SimpleServiceRecursion()
    {
        var service = _provider.Resolve<AbstractService2>();

        int actual = service.GetSomeNumber();

        int expected = 1;
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ThreeLvlServiceRecursion()
    {
        var service = _provider.Resolve<WithRecursionService3>();

        int actual = service.GetSomeNumber();

        int expected = 2;
        Assert.AreEqual(expected, actual);
    }
    
}
