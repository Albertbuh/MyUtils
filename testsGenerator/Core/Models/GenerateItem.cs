namespace Core.Models;

internal class GenerateItem
{
  public string? PathToClass { get; }
  public ClassDeclarationSyntax ClassItem { get; }
  public List<MethodDeclarationSyntax> Methods { get; }

  public string ClassName => ClassItem.Identifier.ValueText;

  public string? GetMethodName(MethodDeclarationSyntax method) 
    => Methods.FirstOrDefault(m => m.Equals(method))?.Identifier.ValueText;

  public GenerateItem(ClassDeclarationSyntax classItem)
  {
    ClassItem = classItem;
    Methods = new();
  }

  public void AddMethod(MethodDeclarationSyntax method)
  {
    Methods.Add(method);
  }

}
