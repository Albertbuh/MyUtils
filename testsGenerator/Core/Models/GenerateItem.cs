namespace Core.Models;

internal class GenerateItem
{
    public string? PathToGeneratedClass { get; set; }
    public CompilationUnitSyntax? Code { get; set; }
    public ClassDeclarationSyntax ClassItem { get; }
    public BaseNamespaceDeclarationSyntax? NamespaceItem { get; }
    public List<MethodDeclarationSyntax> Methods { get; }
    
    public IEnumerable<ParameterSyntax>? ConstructorParameters { get; }
    public IEnumerable<TypeSyntax?>? ConstructorParametersTypes => ConstructorParameters?.Select(p => p.Type);
    public string ClassName => ClassItem.Identifier.ValueText;
    public string? NamespaceName => NamespaceItem?.Name.ToString();

    public string? GetMethodName(MethodDeclarationSyntax method) =>
        Methods.FirstOrDefault(m => m.Equals(method))?.Identifier.ValueText;
    public IEnumerable<string?>? GetMethodParamTypes(MethodDeclarationSyntax method) =>
        Methods.FirstOrDefault(m => m.Equals(method))?.ParameterList.Parameters.Select(p => p.Type?.ToString());

    public GenerateItem(ClassDeclarationSyntax classItem)
    {
        ClassItem = classItem;
        NamespaceItem = ClassItem
            .Ancestors()
            .OfType<BaseNamespaceDeclarationSyntax>()
            .FirstOrDefault();
        Methods = new();
        ConstructorParameters = GetClassParameters();
    }

    public void AddMethod(MethodDeclarationSyntax method)
    {
        Methods.Add(method);
    }

    
    private IEnumerable<ParameterSyntax>? GetClassParameters()
    {
        var constructors = ClassItem.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
        if(constructors != null && constructors.Count() > 0)
        {
          var maxParameteresValue = constructors.Max(c => c.ParameterList.Parameters.Count);
          var constructor = constructors.First(c => c.ParameterList.Parameters.Count == maxParameteresValue);
          return constructor.ParameterList.Parameters;
        }
        return null;
    }
}
