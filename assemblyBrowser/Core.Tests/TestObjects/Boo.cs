namespace Core.Tests.TestObjects;

public class Boo
{
  public const int Amount = 5;
  protected int field;
  private int Field {
    get => field;
  }

  private void SomeMethod()
  {}

  protected string SomeMethodWithParameters(int a)
  {
    return a.ToString();
  }
}
