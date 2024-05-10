using System.Reflection;
using Core.Generators;

namespace Core;

public class Faker
{
    readonly Type[] intNumTypes = new Type[]
    {
        typeof(sbyte),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong)
    };

    readonly Type[] floatNumTypes = new Type[]
    {
        typeof(float),
        typeof(Single),
        typeof(double),
        typeof(decimal)
    };

    readonly Type[] datetimeTypes = new Type[] { typeof(DateTime) };

    readonly Type[] stringTypes = new Type[] { typeof(char), typeof(string) };

    readonly IGenerator collectionGenerator;
    Dictionary<Type[], IGenerator> valueTypeGeneratorsDictionary;

    readonly FakerConfig? config;

    public Faker(FakerConfig config)
        : this()
    {
        this.config = config;
    }

    public Faker()
    {
        valueTypeGeneratorsDictionary = new Dictionary<Type[], IGenerator>()
        {
            { intNumTypes, new IntegralNumberGenerator() },
            { floatNumTypes, new FloatPointNumberGenerator() },
            { datetimeTypes, new DateTimeGenerator() },
            { stringTypes, new StringGenerator() },
        };
        collectionGenerator = new IEnumerableGenerator(valueTypeGeneratorsDictionary);
    }

    public T? Create<T>()
    {
        var result = Create(typeof(T));
        return result != null ? (T)result : default(T);
    }

    private object? Create(Type t)
    {
        object? result;
        if (TryToFillAsValueType(t, out result))
            return result;

        if (TryToFillAsReferenceType(t, out result))
            return result;

        return result;
    }

    private bool TryToFillAsValueType(Type t, out object? result)
    {
        result = GetGeneratorByType(t)?.Generate(t) ?? null;
        return result != null;
    }

    private object FillPublicMembers(object result)
    {
        UpdateAllByConfig(result);

        var type = result.GetType();
        var members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public);
        foreach (var member in members)
        {
            switch (member)
            {
                case PropertyInfo prop:
                    TryToUpdateProperty(result, prop);
                    break;
                case FieldInfo field:
                    TryToUpdateField(result, field);
                    break;
            }
        }
        return result;
    }

    private void TryToUpdateProperty(object result, PropertyInfo prop)
    {
        try
        {
            var pType = prop.PropertyType;
            var value = prop.GetValue(result);
            var isUnprocessed = (value == null) || (value.Equals(GetDefaultValue(pType)));
            if (isUnprocessed)
            {
                var generator = GetGeneratorByType(pType);
                if (generator != null)
                {
                    prop.SetValue(result, generator.Generate(pType));
                    System.Console.WriteLine($"Cast property({prop.Name}) of type {pType}");
                }
                else if (pType != result.GetType())
                {
                    prop.SetValue(result, this.Create(pType));
                }
                else
                    Console.WriteLine($"Unable to cast property({prop.Name})");
            }
        }
        catch { }
    }

    private void TryToUpdateField(object result, FieldInfo field)
    {
        try
        {
            var fType = field.FieldType;
            var value = field.GetValue(result);
            var isUnprocessed = (value == null) || (value.Equals(GetDefaultValue(fType)));
            if (isUnprocessed)
            {
                var generator = GetGeneratorByType(fType);
                if (generator != null)
                {
                    field.SetValue(result, generator.Generate(fType));
                    Console.WriteLine($"Cast field ({field.Name}) of type {fType}");
                }
                else if (fType != result.GetType())
                {
                    field.SetValue(result, this.Create(fType));
                }
                else
                    Console.WriteLine($"Unable to cast field ({field.Name})");
            }
        }
        catch { }
    }

    private void UpdateAllByConfig(object result)
    {
        if (config == null)
            return;

        foreach (var item in config.items)
        {
            try
            {
                if (item.ObjectType.Equals(result.GetType()))
                {
                    var member = item.Expression.Member;
                    switch (member)
                    {
                        case FieldInfo field:
                            field.SetValue(result, item.Generator.Generate(field.FieldType));
                            break;
                        case PropertyInfo prop:
                            prop.SetValue(result, item.Generator.Generate(prop.PropertyType));
                            break;
                    }
                }
            }
            catch { }
        }
    }

    private static object? GetDefaultValue(Type t) =>
        t.IsValueType ? Activator.CreateInstance(t) : null;

    private bool TryToFillAsReferenceType(Type t, out object? result)
    {
        var constructors = t.GetConstructors(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            )
            .OrderByDescending((ConstructorInfo info) => info.GetParameters().Length);

        result = null;
        foreach (var constructor in constructors)
        {
            try
            {
                if (result != null)
                    break;

                result = FillWithConstructor(constructor);
                FillPublicMembers(result);
            }
            catch { }
        }
        return result != null;
    }

    private object FillWithConstructor(ConstructorInfo constructorInfo)
    {
        var parameters = constructorInfo.GetParameters();
        var filledParams = new object[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var pType = parameters[i].ParameterType;
            var generator = GetGeneratorByType(pType);
            if (generator != null)
            {
                if (
                    !TryToUpdateConstructorParamByConfig(
                        constructorInfo.DeclaringType!,
                        parameters[i],
                        ref filledParams[i]
                    )
                )
                    filledParams[i] = generator.Generate(pType);
                Console.WriteLine($"Cast parameter({parameters[i].Name}) of type {pType}");
            }
            else
            {
                if (constructorInfo.DeclaringType != pType)
                    filledParams[i] = this.Create(pType) ?? new object();
                else
                    Console.WriteLine(
                        $"Unable to cast parameter of type {pType}, it creates recursion"
                    );
            }
        }
        return constructorInfo.Invoke(filledParams);
    }

    private bool TryToUpdateConstructorParamByConfig(
        Type objType,
        ParameterInfo parameter,
        ref object filled
    )
    {
        if (config == null)
            return false;

        var items = config.items.Where(ci => ci.ObjectType.Equals(objType));
        foreach (var item in items)
        {
            var member = item.Expression.Member;
            if (string.Compare(parameter.Name, member.Name, true) == 0)
            {
                filled = item.Generator.Generate(parameter.ParameterType);
                return true;
            }
        }
        return false;
    }

    private IGenerator? GetGeneratorByType(Type t)
    {
        foreach (var key in valueTypeGeneratorsDictionary.Keys)
        {
            if (key.Contains(t))
                return valueTypeGeneratorsDictionary[key];
        }
        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return collectionGenerator;
        return null;
    }
}
