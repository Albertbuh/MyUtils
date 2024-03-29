using System.Reflection;
using Core.Utils;

namespace Core.Models;

internal class Implementation
{
    Type type;
    public LifeTimeEnum LifeTime;
    object? implementation;

    public Implementation(Type type, LifeTimeEnum lifeTime)
    {
        this.type = type;
        this.LifeTime = lifeTime;
    }

    public object GetObject(Core.DependencyProvider provider)
    {
        if(implementation != null)
            return implementation;
        
        var constructors = type.GetConstructors(
                BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.Public
                    | BindingFlags.Static
            )
            .OrderByDescending(constructorInfo => constructorInfo.GetParameters().Length);
        object? result = null;
        foreach (var constructor in constructors)
        {
            if (result != null)
                break;

            try
            {
                result = FillWithConstructor(constructor, provider);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw new Exception("Cant fill constructor");
            }
        }
        
        if(LifeTime is LifeTimeEnum.Singleton)
            implementation = result;
        
        return result!;
    }

    private object FillWithConstructor(
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
        return constructor.Invoke(filled);
    }

    private static object? GetDefaultValue(Type t) =>
        t.IsValueType ? Activator.CreateInstance(t) : null;
}
