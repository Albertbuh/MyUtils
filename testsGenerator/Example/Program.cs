using Core;
var generator = new TestsGenerator();
var currentDir = Directory.GetCurrentDirectory();
var files = new List<string>()
{
  @"./Foo.cs"
};

generator.GenerateAsync(files, @"./test");
