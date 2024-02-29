namespace Core.Tests;

public class TracerCore_SmokeTests
{
  const int ms_100 = 100;
  Foo foo;

  public TracerCore_SmokeTests()
  {
    foo = new Foo();
  }
  
  [Fact]
  public void CorrectWork_100ms()
  {
    foo.RunMethodForTrace(ms_100);
    var result = foo.Tracer.GetTraceResult();
    var strTime = result.Threads[0].Time;
    long.TryParse(strTime.Substring(0, strTime.Length - 2), out var time);

    // 4 - is max stopwatch inaccuracy
    Assert.True(Math.Abs(time - ms_100) <= 4, $"Test of correct work finished with {strTime}");
  }

  [Theory]
  [InlineData(2)]
  [InlineData(5)]
  [InlineData(10)]
  public void CorrectWork_Multithread(int threadsAmount)
  {
    var tasks = new Task[threadsAmount];
    for(int i = 1; i <= threadsAmount; i++)
      tasks[i-1] = new Task(() => foo.RunMethodForTrace(ms_100 * i)); 
    Parallel.ForEach(tasks, t => t.Start());
    Task.WaitAll(tasks);
    var result = foo.Tracer.GetTraceResult();
    // Amount of all queries
    // var methodInfoAmount = result.Threads.Select(dto => dto.Methods.Count()).Sum();
    // Amount of all methods
    var methodInfoAmount = result.Threads.Select(dto => dto.Methods.Select(m => m.Count()).Sum()).Sum();
    
    Assert.Equal(threadsAmount, methodInfoAmount);
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
  }
}
