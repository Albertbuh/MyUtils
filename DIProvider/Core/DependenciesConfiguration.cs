namespace Core;

public class DependenciesConfiguration
{
    internal Dictionary<Type, object> services = new();
    
    public void Register<TDependency, TImplementation>()
        where TDependency: class
        where TImplementation: TDependency, new()
    {
       if(services.ContainsKey(typeof(TDependency))) 
           throw new NotImplementedException("At the moment there are no ability to connect more than one implementation for some dependency");

       services.Add(typeof(TDependency), new TImplementation());
    }
}
