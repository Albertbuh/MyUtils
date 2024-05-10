using Tracer.Core;
using Tracer.Core.Serialization;

ITracer tracer = new Tracer.Core.Tracer();
var foo = new Foo(tracer);
int threadsAmount = 10;
var locker = new object();
var tasks = new Task[threadsAmount];
for (int i = 1; i <= threadsAmount; i++)
{
    tasks[i - 1] = new Task(() =>
    {
        foo.RunMethodForTrace(50);
    });
}
Parallel.ForEach(
    tasks,
    (t) =>
    {
        t.Start();
    }
);
Task.WaitAll(tasks);

foo.PrintTraceResults();

public class Foo
{
    private Bar _bar;
    private ITracer _tracer;
    public Tracer.Core.Models.TraceResult TraceResults => _tracer.GetTraceResult();

    public void RunMethodForTrace(int time)
    {
        _tracer.StartTrace();
        Thread.Sleep(time);
        MyMethod();
        _tracer.StopTrace();
    }

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
        if (path.Contains('.'))
            path = path.Substring(0, path.IndexOf('.'));
    }

    public void PrintTraceResults()
    {
        var result = _tracer.GetTraceResult();
        System.Console.WriteLine("Thread -> {0}", System.Environment.CurrentManagedThreadId);
        foreach (var t in result.Threads)
        {
            Console.WriteLine(t.ToString());
        }
        var s = new SerializeLoader();
        s.Serialize(result, "TraceResults");
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
