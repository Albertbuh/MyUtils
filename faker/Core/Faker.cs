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

  readonly IGenerator collectionGenerator;
  Dictionary<Type[], IGenerator> simpleGeneratorsDictionary;
  
  readonly FakerConfig? config;
  
  public Faker(FakerConfig config)
    :this()
  {
    this.config = config;
  }
  
  public Faker()
  {
    simpleGeneratorsDictionary = new Dictionary<Type[], IGenerator>()
    {
      {intNumTypes, new IntegralNumberGenerator()},
      {floatNumTypes, new FloatPointNumberGenerator()},
      {datetimeTypes, new DateTimeGenerator()},
      {stringTypes, new StringGenerator()},
    };
    collectionGenerator = new IEnumerableGenerator(simpleGeneratorsDictionary);
  }
  
  public T? Create<T>()
  {
    var result = Create(typeof(T));
    return result != null ? (T)result : default(T);
  }

  private object? Create(Type t)
  {
    object? result;
    if(TryToCreateValueType(t, out result))
      return result;

    if(TryToCreateByConstructors(t, out result))
    {
      if(result != null)
      {
        UpdatePublicFields(result);
        UpdatePublicProperties(result);
      }
    }
    return result;
  }

  private bool TryToCreateValueType(Type t, out object? result)
  {
    result = GetGeneratorByType(t)?.Generate(t) ?? null;
    return result != null;
  }

  

  private object UpdatePublicProperties(object result)
  {
    UpdateByConfig<PropertyInfo>(result);
    var type = result.GetType();
    
    foreach(PropertyInfo prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
    {
      try
      {
        var pType = prop.PropertyType;
        var generator = GetGeneratorByType(pType);
        var value = prop.GetValue(result);
        var isUnprocessed = (value == null) || (value.Equals(GetDefaultValue(pType)));
        if(isUnprocessed)
        {
          System.Console.WriteLine($"{prop.Name} --> {pType}");
          if(generator != null)
          {
             prop.SetValue(result, generator.Generate(pType));
             System.Console.WriteLine($"Cast property({prop.Name}) of type {pType}");
          }
          else if(pType != type)
          {
            prop.SetValue(result, this.Create(pType));
          }
          else
             System.Console.WriteLine($"Unable to cast property({prop.Name}) of type {pType}, it creates recursion");
        }
      }
      catch{}
    }
    return result;
  }
  
  private void UpdateByConfig<T>(object result)
    where T: MemberInfo
  {
    if(config != null)
    {
      foreach(var item in config.items)
      {
        if(item.ObjectType.Equals(result.GetType())) 
        {
          var member = item.Expression.Member as T;   
          if(member != null)
          {
            try
            {
              if(member is FieldInfo field)
                field.SetValue(result, item.Generator.Generate(field.FieldType));
              else if(member is PropertyInfo prop)
                prop.SetValue(result, item.Generator.Generate(prop.PropertyType));
            }
            catch{}
          }
        }
      }
    }
  }
  
  private object UpdatePublicFields(object result)
  {
    UpdateByConfig<FieldInfo>(result);   
    var type = result.GetType();
    foreach(var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
    {
      try
      {
        var fType = field.FieldType;
        var generator = GetGeneratorByType(fType);
        var value = field.GetValue(result);
        bool isUnprocessed = (value == null) || (value.Equals(GetDefaultValue(fType)));
        if(isUnprocessed)
        {
          if(generator != null)
          {
             field.SetValue(result, generator.Generate(fType));
             System.Console.WriteLine($"Cast field({field.Name}) of type {fType}");
          }
          else if(fType != type)
          {
            field.SetValue(result, this.Create(fType));
          }
          else
            System.Console.WriteLine($"Unable to cast field({field.Name}) of type {fType}, it creates recursion");
        }  
      }
      catch{}
    }
    return result;
  }

  private static object? GetDefaultValue(Type t) => t.IsValueType ? Activator.CreateInstance(t) : null;
  
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
        if(!TryToUpdateByConfig(constructorInfo.DeclaringType!, parameters[i], ref filledParams[i]))
        {
         filledParams[i] = generator.Generate(pType);
        }
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
  
  private bool TryToUpdateByConfig(Type objType, ParameterInfo parameter, ref object filled)
  {
    if(config == null)
      return false;
    
    var obj = config.items.SingleOrDefault(ci => ci.ObjectType.Equals(objType));
    if(!obj.Equals(default(ConfigItem)))
    {
      var member = obj.Expression.Member as MemberInfo;
      if(parameter.Name != null && string.Compare(parameter.Name, member.Name, true) == 0)
      {
        filled = obj.Generator.Generate(parameter.ParameterType);
        return true;
      }
    }
    return false;
  }
  
  private IGenerator? GetGeneratorByType(Type t)
  {
    foreach(var key in simpleGeneratorsDictionary.Keys)
    {
      if(key.Contains(t))
        return simpleGeneratorsDictionary[key];
    }
    if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
      return collectionGenerator;
    return null;
  }
}
