using Core.Models;

namespace Core.Compilers;

internal abstract class ComplexTestsCompiler : TestsCompiler
{
    protected abstract string SetupMethodIdentifier { get; }
    protected abstract string SetupAttributeIdentifier { get; }

    public override CompilationUnitSyntax GenerateCodeByItem(Core.Models.GenerateItem item)
    {
        if(item.ConstructorParameters == null)
            return base.GenerateCodeByItem(item);
        
        return SyntaxFactory
            .CompilationUnit()
            .WithUsings(GenerateUsings(item))
            .WithMembers(
                SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                    GenerateNamespace(item)
                        .WithMembers(
                            SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                                GenerateClass(item)
                            )
                        )
                )
            )
            .NormalizeWhitespace();
    }

    protected override ClassDeclarationSyntax GenerateClass(GenerateItem item)
    {
        if (item.ConstructorParameters == null)
            return base.GenerateClass(item);

        var list = new List<MemberDeclarationSyntax>();
        
        list.Add(GenerateField(item.ClassName, item.ClassName));
        foreach (ParameterSyntax parameter in item.ConstructorParameters)
        {
            string name = '_' + parameter.Identifier.Text;
            string type = parameter.Type!.ToString();
            bool isDependency =
                parameter.Type is SimpleNameSyntax || parameter.Type is QualifiedNameSyntax;
            list.Add(GenerateField(type, name, isDependency));
        }
        list.Add(GenerateSetupMethod(item));
        
        return SyntaxFactory
            .ClassDeclaration($"{item.ClassName}_Tests")
            .WithModifiers(GenerateModifier(SyntaxKind.PublicKeyword))
            .WithMembers(new SyntaxList<MemberDeclarationSyntax>(list));
    }

    protected virtual SyntaxList<UsingDirectiveSyntax> GenerateUsings(Core.Models.GenerateItem item)
    {
        var usingList = new List<UsingDirectiveSyntax>();
        usingList.Add(GenerateUsing("Moq"));
        usingList.Add(GenerateUsing(item.NamespaceName!));
        return new SyntaxList<UsingDirectiveSyntax>(usingList);
    }

    protected UsingDirectiveSyntax GenerateUsing(string name) =>
        SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(name));

    protected FieldDeclarationSyntax GenerateField(
        string typeName,
        string fieldName,
        bool isDependency = false,
        SyntaxKind kind = SyntaxKind.PrivateKeyword
    ) =>
        SyntaxFactory
            .FieldDeclaration(
                GenerateVariableDeclaration(typeName, isDependency)
                    .WithVariables(
                        SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                            SyntaxFactory.VariableDeclarator(fieldName)
                        )
                    )
            )
            .WithModifiers(SyntaxFactory.TokenList(GenerateToken(kind)));

    protected VariableDeclarationSyntax GenerateVariableDeclaration(
        string name,
        bool isDependency
    ) => SyntaxFactory.VariableDeclaration(GenerateType(name, isDependency));

    protected TypeSyntax GenerateType(string name, bool isDependency)
    {
        if (!isDependency)
            return GenerateIdentifierName(name);

        return SyntaxFactory
            .GenericName(GenerateIdentifier("Mock"))
            .WithTypeArgumentList(
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SingletonSeparatedList<TypeSyntax>(GenerateIdentifierName(name))
                )
            );
    }

    protected MemberDeclarationSyntax GenerateSetupMethod(GenerateItem item)
    {
        var result = SyntaxFactory
            .MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                GenerateIdentifier(this.SetupMethodIdentifier)
            )
            .WithModifiers(GenerateModifier(SyntaxKind.PublicKeyword))
            .WithBody(GenerateSetupBlock(item));

        return !String.IsNullOrEmpty(this.SetupAttributeIdentifier)
            ? result.WithAttributeLists(GenerateAttributeList(this.SetupAttributeIdentifier))
            : result;
    }

    protected BlockSyntax GenerateSetupBlock(GenerateItem item)
    {
        var list = new List<ExpressionStatementSyntax>();
        if (item.ConstructorParameters != null)
        {
            var paramNames = new List<string>();
            foreach (var parameter in item.ConstructorParameters)
            {
                string name = '_' + parameter.Identifier.Text;
                string type = parameter.Type!.ToString();
                bool isDependency =
                    parameter.Type is SimpleNameSyntax || parameter.Type is QualifiedNameSyntax;
                list.Add(
                    SyntaxFactory.ExpressionStatement(GenerateAssignment(name, type, isDependency))
                );
                paramNames.Add(name);
            }
            list.Add(
                SyntaxFactory.ExpressionStatement(
                    GenerateAssignment(item.ClassName, item.ClassName, false, paramNames.ToArray())
                )
            );
        }

        return SyntaxFactory.Block().WithStatements(new SyntaxList<StatementSyntax>(list));
    }

    protected AssignmentExpressionSyntax GenerateAssignment(
        string name,
        string type,
        bool isDependency,
        string[]? argumentNames = null
    )
    {
        if (argumentNames == null)
            return SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                GenerateIdentifierName(name),
                SyntaxFactory
                    .ObjectCreationExpression(GenerateType(type, isDependency))
                    .WithArgumentList(SyntaxFactory.ArgumentList())
            );

        var list = new List<SyntaxNodeOrToken>();
        var length = argumentNames.Length;
        for (int i = 0; i < length - 1; i++)
        {
            list.Add(GenerateArgument(argumentNames[i]));
            list.Add(GenerateToken(SyntaxKind.CommaToken));
        }
        list.Add(GenerateArgument(argumentNames[length - 1]));

        return SyntaxFactory.AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            GenerateIdentifierName(name),
            SyntaxFactory
                .ObjectCreationExpression(GenerateType(type, isDependency))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(list))
                )
        );
    }

    protected ArgumentSyntax GenerateArgument(string name) =>
        SyntaxFactory.Argument(
            SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                GenerateIdentifierName(name),
                GenerateIdentifierName("Object")
            )
        );
}
