using System.Reflection;

var faker = new Core.Faker();
var person = faker.Create<Person>();
System.Console.WriteLine(person.ToString());
// PrintConstructors(typeof(Person));

void PrintConstructors(Type myType)
{
    Console.WriteLine("Конструкторы:");
    var constructors = myType.GetConstructors(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
        ).OrderByDescending((ConstructorInfo ci) => ci.GetParameters().Length);
    foreach (ConstructorInfo ctor in constructors)
    {
        string modificator = "";

        // получаем модификатор доступа
        if (ctor.IsPublic)
            modificator += "public";
        else if (ctor.IsPrivate)
            modificator += "private";
        else if (ctor.IsAssembly)
            modificator += "internal";
        else if (ctor.IsFamily)
            modificator += "protected";
        else if (ctor.IsFamilyAndAssembly)
            modificator += "private protected";
        else if (ctor.IsFamilyOrAssembly)
            modificator += "protected internal";

        Console.Write($"{modificator} {myType.Name}(");
        // получаем параметры конструктора
        ParameterInfo[] parameters = ctor.GetParameters();
        for (int i = 0; i < parameters.Length; i++)
        {
            var param = parameters[i];
            Console.Write($"{param.ParameterType.Name} {param.Name}");
            if (i < parameters.Length - 1)
                Console.Write(", ");
        }
        Console.WriteLine(")");
    }
}

struct Person
{
    public string Name { get; }
    public int Age { get; }
    public Nutrition nuts;
    public long gender {get; set;}
    public byte Test;
    public IEnumerable<int> money {get; set;}
    public Person(string name, int age, Nutrition n)
    {
        Name = name; Age = age; nuts = n;
    }
    public Person(string name) : this(name, 1, new Nutrition(0,0,0)){}

    public override string ToString()
    {
      return $"{Name} -> {Age} -> {gender}\nNutrient info: {nuts.protein} -> {nuts.carbs} -> {nuts.fats}\n{Test} {money}";
    }
}

record struct Nutrition(float protein, double carbs, decimal fats);


