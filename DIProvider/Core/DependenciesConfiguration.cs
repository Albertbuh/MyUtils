using Core.Models;

namespace Core;

public class DependenciesConfiguration
{
    internal Dictionary<Type, Dependency> services = new();

    public void Register<TDependency, TImplementation>()
        where TDependency : class
        where TImplementation : TDependency, new()
    {
        var dependencyType = typeof(TDependency);
        var implementationType = typeof(TImplementation);
        
        if (!services.ContainsKey(dependencyType))
            services.Add(dependencyType, new Dependency(dependencyType));
        
        services[dependencyType].AddImplementation(implementationType, LifeTimeEnum.Singleton);
    }
}
