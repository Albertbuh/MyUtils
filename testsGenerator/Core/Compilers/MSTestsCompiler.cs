namespace Core.Compilers;

internal class MSTestsCompiler : ComplexTestsCompiler
{
    protected override string TestAttributeIdentifier => "TestMethod";
    protected virtual string ClassAttributeIdentifier => "TestClass";

    protected override string SetupMethodIdentifier => "Initialize";

    protected override string SetupAttributeIdentifier => "TestInitialize";

    protected override ClassDeclarationSyntax GenerateClass(Core.Models.GenerateItem item)
    {
        return base.GenerateClass(item)
            .WithAttributeLists(GenerateAttributeList(ClassAttributeIdentifier));
    }
}
