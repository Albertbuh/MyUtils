namespace Core.Utils;

internal static class EnumerableUtils
{
    /// <summary>
    /// Convert List of 'objects' to IEnumerable of 'elementType'
    /// </summary>
    public static object ConvertToTypedEnumerable(List<object> values, Type elementType)
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

    public static bool IsEnumerable(Type t) =>
        t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>);

    public static Type GetTypeOfEnumerable(Type t)
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
