namespace Core.Models;

internal class Implementation
{
    public LifeTimeEnum LifeTime;
    object? implementation;
    public object Object =>
        implementation != null ? implementation : Activator.CreateInstance(type)!;

    Type type;

    public Implementation(Type type, LifeTimeEnum lifeTime)
    {
        this.type = type;
        this.LifeTime = lifeTime;
        
        if (lifeTime is LifeTimeEnum.Singleton)
            implementation = Activator.CreateInstance(type);
    }
}
