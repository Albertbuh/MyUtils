namespace Core.Models;

internal class Dependency
{
    private Type dependency;
    public Type Type => dependency;
    private LifeTimeEnum lifeTime;
    private List<Implementation> implementations = new();

    public IEnumerable<object> GetImplementations(Core.DependencyProvider provider)
    {
        var list = new List<object>();
        foreach(var impl in implementations)
        {
            list.Add(impl.GetObject(provider));
        }
        return list;
    }

    public IEnumerable<object> GetImplementations(Core.DependencyProvider provider, Type generic)
    {
        if(!generic.IsGenericType)
            return GetImplementations(provider);
        
        var list = new List<object>();
        foreach(var impl in implementations)
        {
            list.Add(impl.GetObjectWithGeneric(provider, generic));
        }
        return list;
    }

    public Dependency(Type dependency, Type implementation, LifeTimeEnum lifeTime)
        : this(dependency)
    {
        if(implementations == null)
            implementations = new List<Implementation>();
        
        this.lifeTime = lifeTime;
        AddImplementation(implementation, lifeTime);
    }

    public Dependency(Type dependency)
    {
        this.dependency = dependency;
    }
    
    public void AddImplementation(Type implementation, LifeTimeEnum lifeTime)
    {
        implementations.Add(new Implementation(implementation, lifeTime));
    }
}
