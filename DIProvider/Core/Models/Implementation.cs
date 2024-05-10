using System.Reflection;
using Core.Exceptions;
using Core.Utils;

namespace Core.Models;

internal class Implementation
{
    Type _type;
    public LifeTimeEnum LifeTime;
    object? _implementation;

    private object locker = new();

    public Implementation(Type type, LifeTimeEnum lifeTime)
    {
        _type = type;
        LifeTime = lifeTime;
    }

    public object GetObjectWithGeneric(Core.DependencyProvider provider, Type generic)
    {
        if (!generic.IsGenericType)
            return GetObject(provider);

        var genericArgument = generic.GetGenericArguments()[0];
        var constructedType = _type.GetGenericTypeDefinition().MakeGenericType(genericArgument);

        var result = InitializeImplementation(provider, constructedType);
        return result != null ? result : GetObject(provider);
    }

    private IEnumerable<ConstructorInfo> GetConstructors(Type type) =>
        type.GetConstructors(
                BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.Public
                    | BindingFlags.Static
            )
            .OrderByDescending(constructorInfo => constructorInfo.GetParameters().Length);

    private object? InitializeImplementation(Core.DependencyProvider provider, Type type)
    {
        var constructors = GetConstructors(type);
        object? result = null;
        foreach (var constructor in constructors)
        {
            if (result != null)
                break;

            try
            {
                var parameters = FillConstructorParameters(constructor, provider);
                result = constructor.Invoke(parameters);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw new DependencyImplementationException("Cant fill constructor");
            }
        }
        return result;
    }

    public object GetObject(Core.DependencyProvider provider)
    {
        if (_implementation != null)
            return _implementation;

        var result = InitializeImplementation(provider, _type);

        if (LifeTime is LifeTimeEnum.Singleton)
        {
            lock (locker)
            {
                if (_implementation == null)
                {
                    lock (locker)
                        _implementation = result;
                }
            }
        }

        return result!;
    }

    private object?[] FillConstructorParameters(
        ConstructorInfo constructor,
        Core.DependencyProvider provider
    )
    {
        var parameters = constructor.GetParameters();
        var length = parameters.Length;
        var filled = new object?[length];
        for (int i = 0; i < length; i++)
        {
            var pType = parameters[i].ParameterType;
            var dependentService = provider.services.Keys.FirstOrDefault(k => k.Equals(pType));
            if (dependentService != null)
            {
                var innerImplementations = provider
                    .services[dependentService]
                    .GetImplementations(provider);
                if (EnumerableUtils.IsEnumerable(pType))
                    filled[i] = innerImplementations;
                else
                    filled[i] = innerImplementations.Last();
            }
            else
                filled[i] = GetDefaultValue(pType);
        }
        return filled;
    }

    private static object? GetDefaultValue(Type t) =>
        t.IsValueType ? Activator.CreateInstance(t) : null;
}
