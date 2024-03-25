namespace Core.Compilers;

internal class XUnitTestsCompiler: TestsCompiler
{
  protected override string TestAttributeIdentifier => "Fact";
}
