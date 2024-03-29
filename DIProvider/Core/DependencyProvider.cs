using Core.Models;

namespace Core;

public class DependencyProvider
{
    Dictionary<Type, Dependency> services = new();
    DependenciesConfiguration config;

    public DependencyProvider(DependenciesConfiguration config)
    {
        this.config = config;
        services = config.services;
    }

    public T Resolve<T>()
    {
        bool isEnumerable = IsEnumerable(typeof(T));
        Type serviceType = isEnumerable ? GetTypeOfEnumerable(typeof(T)) : typeof(T);

        if (!services.ContainsKey(serviceType))
            throw new InvalidOperationException(
                $"Unable to find such type implementation in dependencies {serviceType.ToString()}"
            );

        if (isEnumerable)
        {
            var data = services[serviceType].Implementations.ToList();
            return (T)ConvertToTypedEnumerable(data, serviceType);
        }
        else
            return (T)services[serviceType].Implementations.Last();
    }

    private object ConvertToTypedEnumerable(List<object> values, Type elementType)
    {
        var genericListType = typeof(List<>).MakeGenericType(elementType);
        var typedCollection = Activator.CreateInstance(genericListType);

        var addMethod = genericListType.GetMethod("Add");
        foreach (var value in values)
        {
            addMethod!.Invoke(typedCollection, new[] { value });
        }

        return typedCollection!.GetType().GetMethod("AsReadOnly")!.Invoke(typedCollection, null)!;
    }

    private bool IsEnumerable(Type t) =>
        t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>);

    private Type GetTypeOfEnumerable(Type t)
    {
        if (IsEnumerable(t))
        {
            Type[] genericArguments = t.GetGenericArguments();
            return genericArguments[0];
        }
        else
            return t;
    }
}
