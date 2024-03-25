namespace Core.Compilers;

internal class MSTestsCompiler : TestsCompiler
{
  protected override string TestAttributeIdentifier => "TestMethod";
  protected virtual string ClassAttributeIdentifier => "TestClass";

  protected override ClassDeclarationSyntax GenerateClass(Core.Models.GenerateItem item)
  {
    return base.GenerateClass(item)
      .WithAttributeLists(GenerateAttributeList(ClassAttributeIdentifier));
  }

}
