namespace Core.Tests;

[TestClass]
public class DIProvider_TemplateTests
{
    DependencyProvider _provider;

    public DIProvider_TemplateTests()
    {
        var config = new DependenciesConfiguration();
        config.Register<IRepository, SomeRepository>();
        // config.Register<IService<IRepository>, ServiceImpl<IRepository>>();

        config.Register(typeof(IService<>), typeof(ServiceImpl<>));

        _provider = new DependencyProvider(config);
    }

    [TestMethod]
    public void SmokeTestOfTemplateClasses()
    {
        var service = _provider.Resolve<IService<IRepository>>();

        int actual = service.GetSomeNumber();

        int expected = 2;
        Assert.AreEqual(expected, actual);
    }

    
    
}
