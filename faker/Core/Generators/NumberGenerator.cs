
namespace Core.Generators;

public abstract class NumberGenerator : IGenerator
{
  protected Random random = new();
  protected Dictionary<Type, Func<object>> generationHandler { get; }
  
  public virtual object Generate(Type typeToGenerate)
  {
    if(generationHandler.ContainsKey(typeToGenerate))
      return generationHandler[typeToGenerate]();
    else
      throw new InvalidOperationException($"Unsupported type: {typeToGenerate}");
  }

  public NumberGenerator()
  {
    generationHandler = new();
  }
}
