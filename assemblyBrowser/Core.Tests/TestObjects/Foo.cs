namespace Core.Tests.TestObjects;

public class Foo
{
  private int field;
  public int Field => field;

  public void SomeMethod()
  {}

  protected string SomeMethodWithParameters(int a)
  {
    return a.ToString();
  }
}
