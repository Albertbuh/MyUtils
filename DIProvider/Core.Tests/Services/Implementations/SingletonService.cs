namespace Core.Tests.Services.Implementations;

public class SingletonService
{
    private int num = new Random().Next();
    public bool DoSmth()
    {
        return true;
    }

    public int GetSomeNumber()
    {
        return num;
    }
}
