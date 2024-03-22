namespace Core.Tests;

[TestClass]
public class Faker_Generators_Tests
{
  Dictionary<GeneratorEnum, IGenerator> generatorsCollection;

  public Faker_Generators_Tests()
  {
    generatorsCollection = new Dictionary<GeneratorEnum, IGenerator>()
    {
      {GeneratorEnum.Datetime, new DateTimeGenerator()},
      {GeneratorEnum.Float, new FloatPointNumberGenerator()},
      {GeneratorEnum.Integral, new IntegralNumberGenerator()},
      {GeneratorEnum.String, new StringGenerator()},
    };
    var ienumerableParameter = new Dictionary<Type[], IGenerator>()
    {
      {new Type[] {typeof(int), typeof(long)}, generatorsCollection[GeneratorEnum.Integral]},
      {new Type[] {typeof(float), typeof(double), typeof(decimal)}, generatorsCollection[GeneratorEnum.Float]},
      {new Type[] {typeof(DateTime)}, generatorsCollection[GeneratorEnum.Datetime]},
      {new Type[] {typeof(string), typeof(char)}, generatorsCollection[GeneratorEnum.String]},
    };
    generatorsCollection.Add(GeneratorEnum.IEnumerable, new IEnumerableGenerator(ienumerableParameter));
  }

  [TestMethod]
  [DataRow(GeneratorEnum.Datetime, typeof(DateTime))]
  [DataRow(GeneratorEnum.Float, typeof(float))]
  [DataRow(GeneratorEnum.Float, typeof(double))]
  [DataRow(GeneratorEnum.Float, typeof(decimal))]
  [DataRow(GeneratorEnum.Integral, typeof(byte))]
  [DataRow(GeneratorEnum.Integral, typeof(sbyte))]
  [DataRow(GeneratorEnum.Integral, typeof(short))]
  [DataRow(GeneratorEnum.Integral, typeof(ushort))]
  [DataRow(GeneratorEnum.Integral, typeof(int))]
  [DataRow(GeneratorEnum.Integral, typeof(uint))]
  [DataRow(GeneratorEnum.Integral, typeof(long))]
  [DataRow(GeneratorEnum.Integral, typeof(ulong))]
  [DataRow(GeneratorEnum.String, typeof(char))]
  [DataRow(GeneratorEnum.String, typeof(string))]
  [DataRow(GeneratorEnum.IEnumerable, typeof(IEnumerable<int>))]
  [DataRow(GeneratorEnum.IEnumerable, typeof(IEnumerable<string>))]
  public void SmokeTest(GeneratorEnum typeOfGen, Type expectedType)
  {
    var value = generatorsCollection[typeOfGen].Generate(expectedType);
    
    Assert.IsInstanceOfType(value, expectedType);
  }

  [TestMethod]
  public void CheckLengthOfStringGeneration()
  {
    var gen = generatorsCollection[GeneratorEnum.String];
    var min = gen.GetType().GetField("minLength", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
    var max = gen.GetType().GetField("maxLength", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);

    var value = (string)gen.Generate(typeof(string));
    var length = value.Length;
    
    Assert.IsTrue(max != null, "Unable to read max value of string generator");
    Assert.IsTrue(min != null, "Unable to read min value of string generator");
    
    Assert.IsTrue(length >= (int)min && length <= (int)max, "Incorrect length of result");
  }

  [TestMethod]
  public void CheckCountOfIEnumerableGeneration()
  {
    var gen = generatorsCollection[GeneratorEnum.IEnumerable];
    var min = gen.GetType().GetField("minLength", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
    var max = gen.GetType().GetField("maxLength", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);

    var value = (IEnumerable<int>)(gen.Generate(typeof(IEnumerable<int>)));
    var length = value.Count();
    
    Assert.IsTrue(max != null, "Unable to read max value of string generator");
    Assert.IsTrue(min != null, "Unable to read min value of string generator");
    
    Assert.IsTrue(length >= (int)min && length <= (int)max, "Incorrect length of result");

  }
}


