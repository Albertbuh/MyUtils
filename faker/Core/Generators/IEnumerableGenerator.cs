namespace Core.Generators;

public class IEnumerableGenerator : IGenerator
{
    Random random = new();
    Dictionary<Type[], IGenerator> generators;

    public IEnumerableGenerator(Dictionary<Type[], IGenerator> generators)
    {
        this.generators = generators;
    }

    public object Generate(Type typeToGenerate)
    {
        if (!IsIEnumerable(typeToGenerate))
            throw new InvalidOperationException($"Type {typeToGenerate} is not IEnumerable");

        var elementType = typeToGenerate.GetGenericArguments()[0];
        var generator = GetGeneratorByType(elementType);
        if (generator != null)
        {
            var data = Enumerable
                .Range(0, random.Next(5, 10))
                .Select(_ => generator.Generate(elementType))
                .ToList();
            return ConvertToTypedEnumerable(data, elementType);
        }
        else
            throw new InvalidOperationException(
                $"Unable to cast collection of type {typeToGenerate}"
            );
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

    private IEnumerable<T> GenerateCollection<T>(Type elementType)
    {
        var count = random.Next(5, 10);
        var generator = GetGeneratorByType(elementType);
        if (generator != null)
        {
            for (int i = 0; i < count; i++)
            {
                yield return (T)generator.Generate(elementType);
            }
        }
        else
            throw new InvalidOperationException($"Unable to fill collection");
    }

    private IGenerator? GetGeneratorByType(Type t)
    {
        foreach (var key in generators.Keys)
        {
            if (key.Contains(t))
                return generators[key];
        }
        return null;
    }

    private bool IsIEnumerable(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return true;
        }

        return typeof(IEnumerable<>).IsAssignableFrom(type);
    }
}
