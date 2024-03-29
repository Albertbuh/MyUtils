using Core.Models;
using Core.Utils;

namespace Core;

public class DependencyProvider
{
    internal Dictionary<Type, Dependency> services = new();
    DependenciesConfiguration config;

    public DependencyProvider(DependenciesConfiguration config)
    {
        this.config = config;
        services = config.services;
    }

    public T Resolve<T>()
    {
        bool isEnumerable = EnumerableUtils.IsEnumerable(typeof(T));
        Type serviceType = isEnumerable ? EnumerableUtils.GetTypeOfEnumerable(typeof(T)) : typeof(T);

        if (!services.ContainsKey(serviceType))
            throw new InvalidOperationException(
                $"Unable to find such type implementation in dependencies {serviceType.ToString()}"
            );

        var implementations = services[serviceType].GetImplementations(this);
        if (isEnumerable)
            return (T)EnumerableUtils.ConvertToTypedEnumerable(implementations.ToList(), serviceType);
        else
            return (T)services[serviceType].GetImplementations(this).Last();//Implementations.Last();
    }

    
}
