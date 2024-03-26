using Core.Models;

namespace Core;

internal class GenerationPipeline
{
    private ExecutionDataflowBlockOptions defaultExecutionOptions;
    private DataflowLinkOptions defaultLinkOptions;

    private TransformManyBlock<string, GenerateItem> loadClassInMemoryBlock;
    private TransformBlock<GenerateItem, GenerateItem> generateTestClassBlock;
    private ActionBlock<GenerateItem> loadTestClassToFileBlock;

    public GenerationPipeline(
        Func<string, List<GenerateItem>> loadClassInMemoryFunc,
        Func<GenerateItem, GenerateItem> generateTestClassFunc,
        Func<GenerateItem, Task> loadTestClassToFileFunc,
        DataflowLinkOptions? linkOptions = null,
        int parallelTasksAmount = 5
    )
    {
        defaultExecutionOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = parallelTasksAmount
        };
        defaultLinkOptions = linkOptions ?? new DataflowLinkOptions { PropagateCompletion = false };

        loadClassInMemoryBlock = new(loadClassInMemoryFunc, defaultExecutionOptions);
        generateTestClassBlock = new(generateTestClassFunc, defaultExecutionOptions);
        loadTestClassToFileBlock = new(loadTestClassToFileFunc, defaultExecutionOptions);

        loadClassInMemoryBlock.LinkTo(generateTestClassBlock, defaultLinkOptions);
        generateTestClassBlock.LinkTo(loadTestClassToFileBlock, defaultLinkOptions);
    }

    public async Task SendAsync(string path)
    {
        System.Console.WriteLine("Send to pipeline {0}", path);
        await loadClassInMemoryBlock.SendAsync(path);
    }

    public bool Post(string path)
    {
        return loadClassInMemoryBlock.Post(path);
    }

    public async Task<bool> CompleteAsync()
    {
        var result = false;
        loadClassInMemoryBlock.Complete();
        System.Console.WriteLine("Wait of completion");
        try
        {
            await loadClassInMemoryBlock.Completion.WaitAsync(CancellationToken.None);
            result = true;
            System.Console.WriteLine("Completed");
        }
        catch(Exception e) 
        { 
            throw new InvalidOperationException($"Cant complete this fking pipeline {e.ToString()}"); 
        }
        return result;
    }

    public bool Complete()
    {
        var result = false;
        loadClassInMemoryBlock.Complete();
        try
        {
            loadClassInMemoryBlock.Completion.Wait();
            result = true;
        }
        catch { }
        return result;
    }
}
