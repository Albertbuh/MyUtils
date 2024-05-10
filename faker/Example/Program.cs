var fconfig = new Core.FakerConfig();
fconfig.Add<Person, int, AgeGenerator>(p => p.Age);

var faker = new Core.Faker(fconfig);
var person = faker.Create<Person>();
System.Console.WriteLine(person.ToString());

person.PrintMoney();

class AgeGenerator : Core.Generators.IGenerator
{
    public object Generate(Type typeToGenerate)
    {
      return new Random().Next(1, 100);  
    }
}
struct Person
{
    public string Name { get; }
    public int Age { get; }
    public Nutrition nuts;
    public long gender {get; set;}
    public byte Test;
    public IEnumerable<long> money {get; set;}
    public int g {get; set;}
    public TimeOnly time;
    public Person(string name, int age, Nutrition n)
    {
        Name = name; Age = age; nuts = n;
    }
    public Person(string name) : this(name, 1, new Nutrition(0,0,0)){}

    public override string ToString()
    {
      return $"{Name} -> {Age} -> {gender}\nNutrient info: {nuts.protein} -> {nuts.carbs} -> {nuts.fats}\nTest: {Test} {g}";
    }

    public void PrintMoney()
    {
      foreach(var m in money)
        System.Console.WriteLine(m);
    }
}

record struct Nutrition(float protein, double carbs, decimal fats);


