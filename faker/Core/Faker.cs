using System.Reflection;
using Core.Generators;

namespace Core;

public class Faker
{
  readonly Type[] intNumTypes = new Type[] 
  {
    typeof(sbyte), typeof(byte), 
    typeof(short), typeof(ushort),
    typeof(int), typeof(uint),
    typeof(long), typeof(ulong)
  };

  readonly Type[] floatNumTypes = new Type[]
  {
    typeof(float), typeof(Single),
    typeof(double), typeof(decimal)
  };
  
  readonly Type[] datetimeTypes = new Type[] 
  {
    typeof(DateTime)
  };
  
  readonly Type[] stringTypes = new Type[]
  {
    typeof(char), typeof(string)
  };
  
  Dictionary<Type[], IGenerator> generatorsCollection;
  public Faker()
  {
    generatorsCollection = new Dictionary<Type[], IGenerator>()
    {
      {intNumTypes, new IntegralNumberGenerator()},
      {floatNumTypes, new FloatPointNumberGenerator()},
      {datetimeTypes, new DateTimeGenerator()},
      {stringTypes, new StringGenerator()}
    };
  }
  
  public T Create<T>()
  {
    return (T)Create(typeof(T));
  }

  private object? Create(Type t)
  {
    var constructors = t.GetConstructors(
      BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
    ).OrderByDescending((ConstructorInfo info) => info.GetParameters().Length);
    foreach(var constructor in constructors)
    {
      try {
        var result = FillByConstructor(constructor);
        return result;
      }
      catch(Exception e)
      {
        System.Console.WriteLine("Smthing went wrong in constructor with {0} params:{1}", constructor.GetParameters().Length, e);
      }
    }
    return null;
  }

  private object FillByConstructor(ConstructorInfo constructorInfo)
  {
    var parameters = constructorInfo.GetParameters();
    var filledParams = new object[parameters.Length];
    for(int i = 0; i < parameters.Length; i++)
    {
      var pType = parameters[i].ParameterType;
      var generator = GetGeneratorByType(pType);
      if(generator != null)
        filledParams[i] = generator.Generate(pType);
      else
        throw new NotImplementedException("When generator is null we need to do recursion descend");
    }
    return constructorInfo.Invoke(filledParams);
  }

  private IGenerator? GetGeneratorByType(Type t)
  {
    foreach(var key in generatorsCollection.Keys)
    {
      if(key.Contains(t))
        return generatorsCollection[key];
    }
    return null;
  }
}
