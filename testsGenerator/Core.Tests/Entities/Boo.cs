namespace Core.Tests.Entities;

public class Boo
{
    IDependency dependency;
    public Boo(IDependency dependency)
    {
        this.dependency = dependency;
    }

    public int SumOfTwo(int a, int b)
    {
        return a + b;
    }

    public string SomeString(Foo foo)
    {
        return foo.ToString() ?? "aloha";
    }
}
