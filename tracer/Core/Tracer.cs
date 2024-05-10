namespace Tracer.Core;

public class Tracer : ITracer
{
    Dictionary<int, ThreadTrace> threads = new();
    private Object locker = new Object();
    private int currentThreadId => System.Environment.CurrentManagedThreadId;

    public Tracer() { }

    public TraceResult GetTraceResult()
    {
        return new TraceResult()
        {
            Threads = threads
                .Values
                .Select(
                    thread =>
                        new ThreadTraceDTO(
                            thread.Id,
                            thread.TotalTime,
                            thread.GetListOfTracedMethods()
                        )
                )
                .ToList()
        };
    }

    public void StartTrace()
    {
        lock (locker)
        {
            if (!threads.ContainsKey(currentThreadId))
            {
                threads.Add(currentThreadId, new ThreadTrace(currentThreadId));
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
                var methodBase = diagnostic.Frame.GetMethod();
                if (methodBase != null)
                {
                    thread.SaveMethodInfo(
                        new MethodInfo(
                            methodBase.Name,
                            methodBase.ReflectedType!.Name,
                            thread.GetTimeMeasurmentOfLastMethod()
                        )
                    );
                }
            }
            thread.TryToStartStopwatch();
        }
    }
}
