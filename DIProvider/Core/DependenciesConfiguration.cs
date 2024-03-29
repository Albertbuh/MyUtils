using Core.Exceptions;
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

    private bool IsReferenceType(Type t) => t.IsClass || t.IsInterface;

    private bool IsImplementation(Type dependency, Type implementation)
    {
        if (dependency.IsAssignableFrom(implementation))
            return true;

        foreach (Type implementedInterface in implementation.GetInterfaces())
        {
            if (dependency.Name.Equals(implementedInterface.Name))
                return true;
        }

        return false;
    }

    private void Register(Type dependencyType, Type implementationType, LifeTimeEnum lifeTime)
    {
        if (
            !IsReferenceType(dependencyType)
            || !IsImplementation(dependencyType, implementationType)
        )
            throw new DependencyConfigurationException(
                $"Incorrect implementation type for {dependencyType}"
            );

        if (!services.ContainsKey(dependencyType))
            services.Add(dependencyType, new Dependency(implementationType));

        services[dependencyType].AddImplementation(implementationType, lifeTime);
    }

    public void Register(Type dependencyType, Type implementationType) =>
        Register(dependencyType, implementationType, LifeTimeEnum.Singleton);

    public void RegisterSingleton(Type dependencyType, Type implementationType) =>
        Register(dependencyType, implementationType, LifeTimeEnum.Singleton);

    public void RegisterTransient(Type dependencyType, Type implementationType) =>
        Register(dependencyType, implementationType, LifeTimeEnum.Transient);
}
