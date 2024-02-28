namespace Tracer.Core;

public class Tracer : ITracer
{
	Dictionary<int, ThreadTrace> threads;

	private int currentThreadId => System.Environment.CurrentManagedThreadId;

	public Tracer()
	{
		threads = new Dictionary<int, ThreadTrace>();
	}

	public TraceResult GetTraceResult()
	{
    var dtoList = new List<ThreadTraceDTO>();
    foreach(var thread in threads.Values)
    {
      dtoList.Add(new ThreadTraceDTO(thread.Id, thread.TotalTime, thread.GetListOfTracedMethods()));
    }
		return new TraceResult() {Threads = dtoList};
	}

	public void StartTrace()
	{
		if (!threads.ContainsKey(currentThreadId))
			threads.Add(currentThreadId, new ThreadTrace(currentThreadId));

		var frame = new StackTrace(true).GetFrame(1);
		if (frame == null)
			throw new NullReferenceException($"No frame for trace (thread: {currentThreadId})");

		var thread = threads[currentThreadId];
		thread.CreateNewDiagnostic(new TraceDiagnostic(frame));
		thread.StopwatchMark();
		thread.TryToStartStopwatch();
	}

	public void StopTrace()
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
			var sb = new System.Text.StringBuilder();
			if (methodInfo != null)
			{
				thread.SaveMethodInfo(
					new MethodInfo(
						methodInfo.Name,
						methodInfo.ReflectedType!.Name,
						thread.GetTimeMeasurmentOfLastMethod()
					),
					ref lastDepth
				);
			}
		}
		thread.TryToStartStopwatch();
	}

	private int lastDepth = Int32.MinValue;
}
