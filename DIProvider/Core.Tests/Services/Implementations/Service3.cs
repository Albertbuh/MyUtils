namespace Core.Tests.Services.Implementations;

public class Service3
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
