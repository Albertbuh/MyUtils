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

    public async Task GenerateAsync(IEnumerable<string> pathsToFiles, string pathToLoad)
    {
        if (!Directory.Exists(pathToLoad))
            Directory.CreateDirectory(pathToLoad);

        var pipeline = new GenerationPipeline(LoadClassInfoInMemory, GenerateTestClass, LoadToFile);
        foreach (var path in pathsToFiles)
        {
            // await pipeline.SendAsync(path);
            pipeline.Post(path);
        }
        await pipeline.CompleteAsync();

        System.Console.WriteLine("Tests uploaded");
    }

    public void Generate(IEnumerable<string> pathsToFiles, string pathToLoad)
    {
        if (!Directory.Exists(pathToLoad))
            Directory.CreateDirectory(pathToLoad);

        foreach (var path in pathsToFiles)
        {
            //_ need to remove warning about lack of async/await
            _ = ParseFile(path, pathToLoad);
        }
        System.Console.WriteLine("Tests uploaded");
    }

    private async Task ParseFile(string path, string pathToLoad)
    {
        var items = LoadClassInfoInMemory(path);
        foreach (var item in items)
        {
            GenerateTestClass(item);
            item.PathToGeneratedClass = Path.Combine(pathToLoad, $"{item.ClassName}Tests.cs");
            await LoadToFile(item);
        }
        System.Console.WriteLine("End of parsing");
    }

    private List<Core.Models.GenerateItem> LoadClassInfoInMemory(string path)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(path));
        var walker = new CustomWalker();
        walker.Visit(syntaxTree.GetRoot());
        System.Console.WriteLine("End of loading");
        return walker.Items;
    }

    private Core.Models.GenerateItem GenerateTestClass(Core.Models.GenerateItem item)
    {
        item.Code = compiler.GenerateCodeByItem(item);
        System.Console.WriteLine("End of generation");
        return item;
    }

    private async Task LoadToFile(Core.Models.GenerateItem item)
    {
        if (!string.IsNullOrEmpty(item.PathToGeneratedClass) && item.Code != null)
            await File.WriteAllTextAsync(item.PathToGeneratedClass, item.Code.ToString());
        else
            System.Console.WriteLine("Path is null");
    }
}
