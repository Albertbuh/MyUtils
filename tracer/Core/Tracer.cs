namespace Tracer.Core;

public class Tracer : ITracer
{
    Dictionary<int, ThreadTrace> threads;

    private Object locker = new Object();
    private Dictionary<int, int> lastDepths = new();

    private int currentThreadId => System.Environment.CurrentManagedThreadId;

    public Tracer()
    {
        threads = new Dictionary<int, ThreadTrace>();
    }

    public TraceResult GetTraceResult()
    {
        var dtoList = new List<ThreadTraceDTO>();
        foreach (var thread in threads.Values)
        {
            dtoList.Add(new ThreadTraceDTO(thread.Id, thread.TotalTime, thread.GetListOfTracedMethods()));
        }
        return new TraceResult() { Threads = dtoList };
    }

    public void StartTrace()
    {
        lock (locker)
        {
            if (!threads.ContainsKey(currentThreadId))
            {
                threads.Add(currentThreadId, new ThreadTrace(currentThreadId));
                lastDepths.Add(currentThreadId, currentThreadId);
            }

            var frame = new StackTrace(true).GetFrame(1);
            if (frame == null)
                throw new NullReferenceException($"No frame for trace (thread: {currentThreadId})");

            var thread = threads[currentThreadId];
            thread.CreateNewDiagnostic(new TraceDiagnostic(frame));
            thread.StopwatchMark();
            thread.TryToStartStopwatch();
        }
    }

    public void StopTrace()
    {
        lock (locker)
        {
            if (!threads.ContainsKey(currentThreadId))
                throw new InvalidOperationException(
                  $"Attemp to stop trace for thread (id {currentThreadId}) without tracing methods"
                );

            var thread = threads[currentThreadId];
            thread.TryToStopTimeMeasurment();
            if (thread.Diagnostics.TryPop(out var diagnostic))
            {
                var methodInfo = diagnostic.Frame.GetMethod();
                if (methodInfo != null)
                {
                    lastDepths[currentThreadId] = thread.SaveMethodInfo(
                      new MethodInfo(
                        methodInfo.Name,
                        methodInfo.ReflectedType!.Name,
                        thread.GetTimeMeasurmentOfLastMethod()
                      ),
                      lastDepths[currentThreadId]
                    );
                }
            }
            thread.TryToStartStopwatch();
        }
    }

}
