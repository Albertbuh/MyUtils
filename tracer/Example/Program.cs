using Tracer.Core;
using Tracer.Serialization;

ITracer tracer = new Tracer.Core.Tracer();
var foo = new Foo(tracer);
var t2 = Task.Run(() => foo.MyMethod());
var t1 = Task.Run(() => foo.MyMethod());
Task.WaitAll(t1, t2);

foo.Serialize(@"trace");

public class Foo
{
	private Bar _bar;
	private ITracer _tracer;

  ITraceResultSerializer serializer = new YamlSerializer();
  
	internal Foo(ITracer tracer)
	{
		_tracer = tracer;
		_bar = new Bar(_tracer);
	}

	public void MyMethod()
	{
		_tracer.StartTrace();
    Thread.Sleep(100);
		_bar.InnerMethod();
		_tracer.StopTrace();
	}

  public void Serialize(string path)
  {
    serializer.Serialize(_tracer.GetTraceResult(), path);
  }

  public void PrintTraceResults()
  {
    System.Console.WriteLine("Thread -> {0}", System.Environment.CurrentManagedThreadId);
    var result = _tracer.GetTraceResult();
    foreach(var t in result.Threads)
    {
      Console.WriteLine(t.ToString());
    }
  }
}

public class Bar
{
	private ITracer _tracer;

	internal Bar(ITracer tracer)
	{
		_tracer = tracer;
	}

	public void InnerMethod()
	{
		_tracer.StartTrace();
		Thread.Sleep(200);
		Aboba();
    Biboba();
		_tracer.StopTrace();
	}

	private void Aboba()
	{
		_tracer.StartTrace();
		Thread.Sleep(300);
		_tracer.StopTrace();
	}

  public void Biboba()
  {
    _tracer.StartTrace();
    Thread.Sleep(400);
    _tracer.StopTrace();
  }
}
