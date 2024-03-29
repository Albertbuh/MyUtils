public class Program
{
    public static void Main()
    {
        Type genericTypeDefinition = typeof(ServiceImpl<>);
        Type genericTypeArgument = typeof(Repository);
        Type constructedType = genericTypeDefinition.MakeGenericType(genericTypeArgument);

        object instance = Activator.CreateInstance(constructedType);

        Console.WriteLine($"Instance: {instance}");
    }
}

public interface IRepository
{
    void Method();
}

public class Repository : IRepository
{
    public void Method()
    {
        Console.WriteLine("Repository Method");
    }
}

public class ServiceImpl<TRepository> where TRepository : IRepository, new()
{
    private TRepository repository;

    public ServiceImpl()
    {
        repository = new TRepository();
    }
}
