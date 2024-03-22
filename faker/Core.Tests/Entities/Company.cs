namespace Core.Tests.Entities;

#pragma warning disable CS8618  // Non-nullable field is uninitialized.
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value.

internal class Company
{
  public IEnumerable<Person> staff;
  string Name {get; set;}

  public Company()
  {}

  public Company(IEnumerable<Person> staff, string name)
  {
    this.staff = staff;
    Name = name;
  }
}
