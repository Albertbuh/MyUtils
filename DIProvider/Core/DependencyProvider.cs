namespace Core;

public class DependencyProvider
{
    Dictionary<Type, object> services = new();
    DependenciesConfiguration config;

    public DependencyProvider(DependenciesConfiguration config)
    {
      this.config = config;   
      services = config.services;
    }

    public T Resolve<T>()
    {
        if(!services.ContainsKey(typeof(T)))
            throw new InvalidOperationException("Unable to find such type implementation in dependencies");

        return (T)services[typeof(T)];
    }
}
