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
  
  public T? Create<T>()
  {
    var result = Create(typeof(T));
    return result != null ? (T)result : default(T);
  }

  private object? Create(Type t)
  {
    object? result;
    if(TryToCreateValueType(t, out result) || TryToCreateByConstructors(t, out result))
      return result;
    return null;
  }

  private bool TryToCreateValueType(Type t, out object? result)
  {
    result = GetGeneratorByType(t)?.Generate(t) ?? null;
    return result != null;
  }

  private bool TryToCreateByConstructors(Type t, out object? result)
  {
    var constructors = t.GetConstructors(
      BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
    ).OrderByDescending((ConstructorInfo info) => info.GetParameters().Length);
    result = null;
    foreach(var constructor in constructors)
    {
      if(result != null)
        break;
      try 
      {
        result = FillWithConstructor(constructor);
      }
      catch{}
    }
    return result != null;
  }

  private object FillWithConstructor(ConstructorInfo constructorInfo)
  {
    var parameters = constructorInfo.GetParameters();
    var filledParams = new object[parameters.Length];
    for(int i = 0; i < parameters.Length; i++)
    {
      var pType = parameters[i].ParameterType;
      var generator = GetGeneratorByType(pType);
      if(generator != null)
      {
         filledParams[i] = generator.Generate(pType);
         System.Console.WriteLine($"Cast parameter({parameters[i].Name}) of type {pType}");
      }
      else //we has class type
      {
        if(constructorInfo.DeclaringType != pType)
           filledParams[i] = this.Create(pType) ?? new object();
        else
         System.Console.WriteLine($"Unable to cast parameter of type {pType}, it creates recursion");
      }
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
