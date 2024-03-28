namespace Core.Compilers;

internal class XUnitTestsCompiler : ComplexTestsCompiler
{
    protected override string TestAttributeIdentifier => "Fact";

    protected override string SetupMethodIdentifier => "";

    protected override string SetupAttributeIdentifier => "";
}
