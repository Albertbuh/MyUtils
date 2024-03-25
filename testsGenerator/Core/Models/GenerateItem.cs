namespace Core.Models;

internal class GenerateItem
{
  public string? PathToClass { get; }
  public ClassDeclarationSyntax ClassItem { get; }
  public BaseNamespaceDeclarationSyntax? NamespaceItem { get; }
  public List<MethodDeclarationSyntax> Methods { get; }

  public string ClassName => ClassItem.Identifier.ValueText;
  public string? NamespaceName 
    => NamespaceItem?.Name.ToString();

  public string? GetMethodName(MethodDeclarationSyntax method) 
    => Methods.FirstOrDefault(m => m.Equals(method))?.Identifier.ValueText;

  public GenerateItem(ClassDeclarationSyntax classItem)
  {
    ClassItem = classItem;
    NamespaceItem = ClassItem.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
    Methods = new();
  }

  public void AddMethod(MethodDeclarationSyntax method)
  {
    Methods.Add(method);
  }

}
