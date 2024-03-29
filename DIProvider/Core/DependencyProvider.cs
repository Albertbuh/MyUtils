using Core.Exceptions;
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
        Type serviceType = isEnumerable
            ? EnumerableUtils.GetTypeOfEnumerable(typeof(T))
            : typeof(T);

        IEnumerable<object> implementations;
        if (!services.ContainsKey(serviceType))
        {
            if (serviceType.IsGenericType)
                implementations = GetImplementationsForGeneric(serviceType);
            else
                throw new DependencyProviderException(
                    $"Unable to find such type implementation in dependencies: {serviceType.ToString()}"
                );
        }
        else
            implementations = services[serviceType].GetImplementations(this);

        return isEnumerable
            ? (T)EnumerableUtils.ConvertToTypedEnumerable(implementations.ToList(), serviceType)
            : (T)implementations.Last();
    }

    private IEnumerable<object> GetImplementationsForGeneric(Type type)
    {
        return services[type.GetGenericTypeDefinition()].GetImplementations(this, type);
    }
}
