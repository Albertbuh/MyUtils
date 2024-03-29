namespace Core.Models;

internal class Dependency
{
    private Type dependency;
    private List<Implementation> implementations = new();

    public IEnumerable<object> Implementations => implementations.Select(impl => impl.Object);

    public Dependency(Type dependency, Type implementation, LifeTimeEnum lifeTime)
        : this(dependency)
    {
        if(implementations == null)
            implementations = new List<Implementation>();
        
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
