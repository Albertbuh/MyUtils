namespace Core.Tests;

public class TracerCore_SmokeTests
{
  const int small_ms_value = 30;
  Foo foo;

  public TracerCore_SmokeTests()
  {
    foo = new Foo();
  }
  
  [Fact]
  public void CorrectWork_TraceOneMethod_100ms()
  {
    foo.RunMethodForTrace(small_ms_value);
    var result = foo.Tracer.GetTraceResult();
    var threadInfo = result.Threads[0];
    
    var methodInfo = threadInfo.Methods.First().Peek();
    long.TryParse(methodInfo.Time.Substring(0, methodInfo.Time.Length - 2), out var time);
    // 4 - is max stopwatch inaccuracy
    Assert.True(Math.Abs(time - small_ms_value) <= 4 && 
                methodInfo.Name.Equals("RunMethodForTrace") &&
                methodInfo.ClassName.Equals("Foo"),
                $"Test of correct work finished with {methodInfo.Time}");
  }

  [Theory]
  [InlineData(2)]
  [InlineData(5)]
  [InlineData(10)]
  public void CorrectWork_Multithread(int threadsAmount)
  {
    var tasks = new Task[threadsAmount];
    for(int i = 1; i <= threadsAmount; i++)
      tasks[i-1] = new Task(() => foo.RunMethodForTrace(small_ms_value * i)); 
    Parallel.ForEach(tasks, t => t.Start());
    Task.WaitAll(tasks);
    var result = foo.Tracer.GetTraceResult();
    // Amount of all queries
    // var methodInfoAmount = result.Threads.Select(dto => dto.Methods.Count()).Sum();
    // Amount of all methods
    var methodInfoAmount = result.Threads.Select(dto => dto.Methods.Select(m => m.Count()).Sum()).Sum();
    
    Assert.Equal(threadsAmount, methodInfoAmount);
  }

  [Fact]
  public void InnerMethods_100ms()
  {
    foo.RunMethodWithInnerMethod(small_ms_value);
    var result = foo.Tracer.GetTraceResult();
    var threadInfo = result.Threads[0];
    
    var innerMethodInfo = threadInfo.Methods.First().Peek().ChildMethods.First();
    int.TryParse(innerMethodInfo.Time, out var time);
    Assert.True(innerMethodInfo.Name.Equals("RunInnerMethodForTrace") &&
                innerMethodInfo.ClassName.Equals("Foo"),
                $"Test of correct work of method {innerMethodInfo.Name} ({innerMethodInfo.ClassName}) finished with {innerMethodInfo.Time}");

  }
  
  private class Foo
  {
    public ITracer Tracer = new Tracer.Core.Tracer();  
    public void RunMethodForTrace(int time)
    {
      Tracer.StartTrace();
      Thread.Sleep(time);
      Tracer.StopTrace();
    }

    public void RunMethodWithInnerMethod(int time)
    {
      Tracer.StartTrace();
      Thread.Sleep(time);
      RunInnerMethodForTrace(2 * time);
      Tracer.StopTrace();
    }

    private void RunInnerMethodForTrace(int time = 100)
    {
      Tracer.StartTrace();
      Thread.Sleep(time);
      Tracer.StopTrace();
    }
  }
}
