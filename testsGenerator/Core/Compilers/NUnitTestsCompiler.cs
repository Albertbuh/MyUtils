namespace Core.Compilers;

internal class NUnitTestsCompiler : ComplexTestsCompiler
{
    protected override string TestAttributeIdentifier => "Test";
    protected virtual string ClassAttributeIdentifier => "TestFixture";

    protected override string SetupMethodIdentifier => "SetUp";

    protected override string SetupAttributeIdentifier => "SetUp";

    protected override ClassDeclarationSyntax GenerateClass(Core.Models.GenerateItem item)
    {
        return base.GenerateClass(item)
            .WithAttributeLists(GenerateAttributeList(ClassAttributeIdentifier));
    }
}
