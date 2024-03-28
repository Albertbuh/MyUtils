using Core.Models;

namespace Core.Compilers;

internal abstract class ComplexTestsCompiler : TestsCompiler
{
    protected abstract string SetupMethodIdentifier { get; }
    protected abstract string SetupAttributeIdentifier { get; }

    public override CompilationUnitSyntax GenerateCodeByItem(Core.Models.GenerateItem item)
    {
        if (item.ConstructorParameters == null)
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

        foreach (var method in item.Methods)
        {
            var name = item.GetMethodName(method);
            if (name != null)
                list.Add(GenerateComplexTestMethod(name, method));
        }

        return SyntaxFactory
            .ClassDeclaration($"{item.ClassName}_Tests")
            .WithModifiers(GenerateModifier(SyntaxKind.PublicKeyword))
            .WithMembers(new SyntaxList<MemberDeclarationSyntax>(list));
    }

    protected MethodDeclarationSyntax GenerateComplexTestMethod(
        string name,
        MethodDeclarationSyntax method
    ) =>
        SyntaxFactory
            .MethodDeclaration(
                SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                GenerateIdentifier(name)
            )
            .WithAttributeLists(GenerateAttributeList(this.TestAttributeIdentifier))
            .WithModifiers(GenerateModifier(SyntaxKind.PublicKeyword))
            .WithBody(GenerateComplexBlock(method));

    protected BlockSyntax GenerateComplexBlock(MethodDeclarationSyntax method)
    {
        var list = new List<StatementSyntax>();
        var parameters = method.ParameterList.Parameters;
        var names = new List<string>();
        //Arrange part
        foreach (var parameter in parameters)
        {
            var name = parameter.Identifier.ValueText;
            names.Add(name);
            var typeStr = parameter.Type!.ToString();
            var type = Type.GetType(typeStr);
            list.Add(GenerateLocalDeclaration(name, typeStr, GenerateLiteral(parameter.Type!)));
        }

        //Act part
        var returnType = method.ReturnType;
        var classDeclaration = method.Ancestors().OfType<ClassDeclarationSyntax>().First();
        var className = classDeclaration.Identifier.ValueText;
        var methodName = method.Identifier.ValueText;
        list.Add(
            GenerateLocalDeclaration(
                "actual",
                returnType.ToString(),
                GenerateInvocation(className, methodName, names.ToArray())
            )
        );

        //Assert part
        list.Add(
            GenerateLocalDeclaration("expected", returnType.ToString(), GenerateLiteral(returnType))
        );
        list.Add(GenerateAssertStatement("Equals", new string[] { "actual", "expected" }));
        list.Add(GenerateAssertStatement("Fail", new string[] { "autogenerated" }));

        // return SyntaxFactory.Block(list);
        return SyntaxFactory.Block().WithStatements(new SyntaxList<StatementSyntax>(list));
    }

    protected ExpressionStatementSyntax GenerateAssertStatement(
        string methodName,
        string[]? parameters
    ) => SyntaxFactory.ExpressionStatement(GenerateInvocation("Assert", methodName, parameters));

    protected InvocationExpressionSyntax GenerateInvocation(
        string className,
        string methodName,
        string[]? parameters
    ) =>
        SyntaxFactory
            .InvocationExpression(GenerateMemberAccess(className, methodName))
            .WithArgumentList(GenerateArgumentList(parameters));

    protected LocalDeclarationStatementSyntax GenerateLocalDeclaration(
        string name,
        string type,
        ExpressionSyntax expression
    ) =>
        SyntaxFactory.LocalDeclarationStatement(
            GenerateVariableDeclaration(type, false)
                .WithVariables(
                    SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                        SyntaxFactory
                            .VariableDeclarator(GenerateIdentifier(name))
                            .WithInitializer(SyntaxFactory.EqualsValueClause(expression))
                    )
                )
        );

    protected string[] supportedValueTypes = new string[]
    {
        "byte",
        "sbyte",
        "short",
        "ushort",
        "int",
        "uint",
        "long",
        "ulong",
        "float",
        "double",
        "decimal"
    };

    protected LiteralExpressionSyntax GenerateLiteral(TypeSyntax ts)
    {
        var typeName = ts.ToString();
        if (supportedValueTypes.Contains(typeName))
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(0)
            );
        else if (typeName.Equals("string"))
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal("")
            );
        else if (typeName.Equals("char"))
            return SyntaxFactory.LiteralExpression(
                SyntaxKind.CharacterLiteralExpression,
                SyntaxFactory.Literal(' ')
            );
        else if (typeName.Equals("bool"))
            return SyntaxFactory.LiteralExpression(
                    SyntaxKind.FalseLiteralExpression
                    );
        else
            return SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
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
        string typeName,
        bool isDependency
    ) => SyntaxFactory.VariableDeclaration(GenerateType(typeName, isDependency));

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
    ) =>
        SyntaxFactory.AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            GenerateIdentifierName(name),
            SyntaxFactory
                .ObjectCreationExpression(GenerateType(type, isDependency))
                .WithArgumentList(GenerateArgumentList(argumentNames, "Object"))
        );

    protected ArgumentListSyntax GenerateArgumentList(
        string[]? argumentNames,
        string? paramName = null
    )
    {
        if (argumentNames == null || !argumentNames.Any())
            return SyntaxFactory.ArgumentList();

        var list = new List<SyntaxNodeOrToken>();
        list.Add(GenerateArgument(argumentNames[0], paramName));
        var length = argumentNames.Length;
        for (int i = 1; i < length; i++)
        {
            list.Add(GenerateToken(SyntaxKind.CommaToken));
            list.Add(GenerateArgument(argumentNames[i], paramName));
        }

        return SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList<ArgumentSyntax>(list));
    }

    protected ArgumentSyntax GenerateArgument(MemberAccessExpressionSyntax member) =>
        SyntaxFactory.Argument(member);

    protected ArgumentSyntax GenerateArgument(string name) =>
        SyntaxFactory.Argument(GenerateIdentifierName(name));

    protected ArgumentSyntax GenerateArgument(string firstName, string? secondName) =>
        String.IsNullOrEmpty(secondName)
            ? GenerateArgument(firstName)
            : GenerateArgument(GenerateMemberAccess(firstName, secondName));

    protected MemberAccessExpressionSyntax GenerateMemberAccess(string objName, string paramName) =>
        SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            GenerateIdentifierName(objName),
            GenerateIdentifierName(paramName)
        );
}
