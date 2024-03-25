internal class CustomWalker: CSharpSyntaxWalker
{
  public List<Core.Models.GenerateItem> Items { get; } = new();

  public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
  {
    if(node.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
    {
      var parent = node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
      if(parent != null)
      {
        var parentItem = Items.FirstOrDefault(i => CompareClassNames(i.ClassItem, parent) == 0);
        parentItem?.AddMethod(node);
      }
    }
    base.VisitMethodDeclaration(node);
  }

  public override void VisitClassDeclaration(ClassDeclarationSyntax node)
  {
    Items.Add(new Core.Models.GenerateItem(node));
    base.VisitClassDeclaration(node);
  }

  private int CompareClassNames(ClassDeclarationSyntax cl1, ClassDeclarationSyntax cl2)
    => string.Compare(cl1.Identifier.ValueText, cl2.Identifier.ValueText, true);
}
