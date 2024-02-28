using Tracer.Core;

ITracer tracer = new Tracer.Core.Tracer();
var foo = new Foo(tracer);
foo.MyMethod();

public class Foo
{
	private Bar _bar;
	private ITracer _tracer;

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
