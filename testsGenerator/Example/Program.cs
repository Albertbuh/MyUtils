using Core;
var generator = new TestsGenerator(Core.Compilers.GeneratorCompilersEnum.XUnit);
var currentDir = Directory.GetCurrentDirectory();
var files = new List<string>()
{
  @"../../../Foo.cs", @"../../../Loo.cs"
};

await generator.GenerateAsync(files, @"../../../test");
 //generator.Generate(files, @"../../../test");
