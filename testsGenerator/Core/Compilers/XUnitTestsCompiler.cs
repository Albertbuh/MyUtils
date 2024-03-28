namespace Core.Compilers;

internal class XUnitTestsCompiler : ComplexTestsCompiler
{
    protected override string TestAttributeIdentifier => "Fact";

    protected override string SetupMethodIdentifier => "SetUp";

    protected override string SetupAttributeIdentifier => "SetUp";
}
