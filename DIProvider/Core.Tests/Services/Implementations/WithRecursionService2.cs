namespace Core.Tests.Services.Implementations;

public class WithRecursionService2 : AbstractService2
{
    IService1? service;

    public WithRecursionService2()
    {}
    
    public WithRecursionService2(IService1 service)
    {
        this.service = service;
    }

    public override bool DoSmth()
    {
        return true;
    }

    public override int GetSomeNumber()
    {
        return service != null ? service.GetNumber() : Int32.MinValue;
    }
}
