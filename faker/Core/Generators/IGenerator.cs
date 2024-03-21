namespace Core.Generators;

public interface IGenerator
{
  public object Generate(Type typeToGenerate);
}
