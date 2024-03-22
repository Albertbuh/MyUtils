namespace Core.Tests;

[TestClass]
public class Faker_Tests
{
  private Faker faker;
  private Faker fakerWithConfig;
  private static Faker staticFaker = new Faker();

  public Faker_Tests()
  {
    var config = new FakerConfig();
    config.Add<Company, IEnumerable<Person>, PersonGenerator>(c => c.staff);
    fakerWithConfig = new Faker(config);
    faker = new Faker();
  }

  [TestMethod]
  public void FakeSimpleObjectWithoutRecursion()
  {
    var person = faker.Create<Person>();
    Assert.IsNotNull(person);

    var members = person.GetType().GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    foreach(var member in members)
    {
      switch(member.MemberType)
      {
        case MemberTypes.Field:
          var field = member as FieldInfo;
          var value = field!.GetValue(person);
          Assert.IsNotNull(value, $"value of {field} is null");
          Assert.AreNotEqual(value, GetDefaultValueByType(field.FieldType));
        break;
        case MemberTypes.Property:
          var prop = member as PropertyInfo;
          value = prop!.GetValue(person);
          Assert.IsNotNull(value, $"value of {prop} is null");
          Assert.AreNotEqual(value, GetDefaultValueByType(prop.PropertyType));
        break;
      }
    }
    Assert.IsTrue(person.ConstructorId > 0, $"oops {person.ConstructorId} is less than zero");
  }

  [TestMethod]
  public void FakeComplexObject()
  {
    var company = fakerWithConfig.Create<Company>();
    Assert.IsNotNull(company);

    var members = company.GetType().GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
    foreach(var member in members)
    {
      switch(member.MemberType)
      {
        case MemberTypes.Field:
          var field = member as FieldInfo;
          var value = field!.GetValue(company);
          Assert.IsNotNull(value, $"value of {field} is null");
          Assert.AreNotEqual(value, GetDefaultValueByType(field.FieldType));
        break;
        case MemberTypes.Property:
          var prop = member as PropertyInfo;
          value = prop!.GetValue(company);
          Assert.IsNotNull(value, $"value of {prop} is null");
          Assert.AreNotEqual(value, GetDefaultValueByType(prop.PropertyType));
        break;
      }
    }
  }

  private object? GetDefaultValueByType(Type t)
  {
    if(t.IsValueType)
      return Activator.CreateInstance(t);
    else
      return null;
  }

  class PersonGenerator : IGenerator
  {
    public object Generate(Type typeToGenerate)
    {
      var random = new Random();
      List<object> data = new();
      for(int i = 0; i < 5; i++)
        data.Add(staticFaker.Create<Person>()!);
      return ConvertToTypedEnumerable(data, typeof(Person));
    }

    private object ConvertToTypedEnumerable(List<object> values, Type elementType)
    {
      var genericListType = typeof(List<>).MakeGenericType(elementType);
      var typedCollection = Activator.CreateInstance(genericListType);

      var addMethod = genericListType.GetMethod("Add");
      foreach (var value in values)
      {
        addMethod!.Invoke(typedCollection, new[] { value });
      }

      return typedCollection!.GetType().GetMethod("AsReadOnly")!.Invoke(typedCollection, null)!;
    }
  }
}


