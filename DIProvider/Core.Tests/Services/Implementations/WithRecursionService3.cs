namespace Core.Tests.Services.Implementations;

public class WithRecursionService3
{
    AbstractService2? service;

    public WithRecursionService3()
    {}

    public WithRecursionService3(AbstractService2 service)
    {
        this.service = service;
    }
    
    public bool DoSmth()
    {
        return true;
    }
    
    public int GetSomeNumber()
    {
        if(service != null)
            return 1 + service.GetSomeNumber();
        else
            return Int32.MinValue;
    }
}
