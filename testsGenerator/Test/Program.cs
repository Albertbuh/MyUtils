using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var code = SyntaxFactory.CompilationUnit()
.WithMembers(
    SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.ClassDeclaration("SomeClass")
        .WithModifiers(
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
        .WithMembers(
            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(
                        SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                    SyntaxFactory.Identifier("SomeMethod"))
                .WithAttributeLists(
                    SyntaxFactory.SingletonList<AttributeListSyntax>(
                        SyntaxFactory.AttributeList(
                            SyntaxFactory.SingletonSeparatedList<AttributeSyntax>(
                                SyntaxFactory.Attribute(
                                    SyntaxFactory.IdentifierName("SomeAttribute"))))))
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                .WithBody(
                    SyntaxFactory.Block())))))
.NormalizeWhitespace();

var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(@"./Foo.cs"));
System.Console.WriteLine(code);
var walker = new PublicMethodWalker();
System.Console.WriteLine("Root: " + syntaxTree.GetRoot());
walker.Visit(syntaxTree.GetRoot());
foreach (MethodDeclarationSyntax method in walker.PublicMethods)
{
    Console.WriteLine(method.Identifier.ValueText);
}

public class PublicMethodWalker : CSharpSyntaxWalker
{
    public List<MethodDeclarationSyntax> PublicMethods { get; } = new List<MethodDeclarationSyntax>();
    public List<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
      if (node.Modifiers.Any(modifier => modifier.Kind() == SyntaxKind.PublicKeyword))
      {
        PublicMethods.Add(node);
        System.Console.WriteLine(node.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault().Identifier.ValueText);
      }

      base.VisitMethodDeclaration(node);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
      Classes.Add(node);
      base.VisitClassDeclaration(node);
    }
}
