namespace Core.Tests.Entities;

#pragma warning disable CS8618  // Non-nullable field is uninitialized.
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value.

internal class Person
{
  public int Age {get; set;}
  public string Name {get; set;}
  public DateTime BirthDate;
  public float Growth;
  public double Weight;
  private decimal Budget;

  private int constructorId = -1;
  public int ConstructorId
  {
    get => constructorId;
    set {
      if(constructorId < 0)
        constructorId = value;
    }
  }
  

  public Person()
  {
    ConstructorId = 0;
  }

  public Person(string name)
  {
    this.Name = name;
    ConstructorId = 1;
  }

  public Person(string name, int age, decimal budget)
  {
    this.Name = name;
    this.Age = age;
    this.Budget = budget;
    ConstructorId = 2;
  }
}
