namespace Core.Compilers;

internal class NUnitTestsCompiler: TestsCompiler
{
  protected override string TestAttributeIdentifier => "Test";
  protected virtual string ClassAttributeIdentifier => "TestFixture";

  protected override ClassDeclarationSyntax GenerateClass(Core.Models.GenerateItem item)
  {
    return base.GenerateClass(item)
      .WithAttributeLists(GenerateAttributeList(ClassAttributeIdentifier));
  }
}
