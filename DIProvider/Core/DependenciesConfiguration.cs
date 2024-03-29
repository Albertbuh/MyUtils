using Core.Models;

namespace Core;

public class DependenciesConfiguration
{
    internal Dictionary<Type, Dependency> services = new();

    private void Register<TDependency, TImplementation>(LifeTimeEnum lifeTime)
        where TDependency : class
        where TImplementation : TDependency, new()
    {
        var dependencyType = typeof(TDependency);
        var implementationType = typeof(TImplementation);

        if (!services.ContainsKey(dependencyType))
            services.Add(dependencyType, new Dependency(dependencyType));

        services[dependencyType].AddImplementation(implementationType, lifeTime);
    }

    
    public void Register<TDependency, TImplementation>()
        where TDependency : class
        where TImplementation : TDependency, new() =>
        Register<TDependency, TImplementation>(LifeTimeEnum.Singleton);

    public void RegisterSingleton<TDependency, TImplementation>()
        where TDependency : class
        where TImplementation : TDependency, new() =>
        Register<TDependency, TImplementation>(LifeTimeEnum.Singleton);

    public void RegisterTransient<TDependency, TImplementation>()
        where TDependency : class
        where TImplementation : TDependency, new() =>
        Register<TDependency, TImplementation>(LifeTimeEnum.Transient);
}
