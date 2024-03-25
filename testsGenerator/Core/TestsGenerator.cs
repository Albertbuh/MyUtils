namespace Core;

public class TestsGenerator
{
  TestsCompiler compiler;
  
  public TestsGenerator()
  {
    compiler = TestsCompilerFactory.Create(Compilers.GeneratorCompilersEnum.XUnit);
  }

  public TestsGenerator(Compilers.GeneratorCompilersEnum compilerType)
  {
    this.compiler = TestsCompilerFactory.Create(compilerType);
  }


  public void GenerateAsync(IEnumerable<string> pathsToFiles, string pathToLoad)
  {
    if(!Directory.Exists(pathToLoad))
      Directory.CreateDirectory(pathToLoad);
    foreach(var path in pathsToFiles)
    {
      ParseFile(path, pathToLoad);
    }
  }

  private void ParseFile(string path, string pathToLoad)
  {
    var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(path));
    var walker = new CustomWalker();
    walker.Visit(syntaxTree.GetRoot());
    var items = walker.Items;
    foreach(var item in walker.Items)
      GenerateTestClass(item, pathToLoad);
  }

  private void GenerateTestClass(Core.Models.GenerateItem item, string pathToLoad)
  {
    var code = compiler.GenerateCodeByItem(item);
    var path = Path.Combine(pathToLoad, $"{item.ClassName}Tests.cs");
    File.WriteAllText(path, code.ToString());
  }

  
  private MethodDeclarationSyntax GenerateMethod()
  {
    throw new NotImplementedException("Not yet"); 
  }
}
