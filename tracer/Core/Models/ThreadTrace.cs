namespace Tracer.Core.Models;

internal class ThreadTrace
{
	internal int Id;
  internal long TotalTime {get; private set;}
	internal int Depth => Diagnostics.Count;
	internal Stack<TraceDiagnostic> Diagnostics;
	private Stack<long> _savedTime = new Stack<long>();
	private Stopwatch _stopwatch;

	private List<Queue<MethodInfo>> _methodsList = new List<Queue<MethodInfo>>();
	private Queue<MethodInfo> _traceQueue = new Queue<MethodInfo>();

	public ThreadTrace(int id)
	{
		Id = id;
		Diagnostics = new Stack<TraceDiagnostic>();
		_stopwatch = new Stopwatch();
	}

  public IEnumerable<Queue<MethodInfo>> GetListOfTracedMethods()
  {
    return _methodsList;
  }

	internal void SaveMethodInfo(MethodInfo method, ref int lastDepth)
	{
		if (Depth < lastDepth)
		{
			method.ChildMethods = _traceQueue;
			_traceQueue = new Queue<MethodInfo>();
		}

		_traceQueue.Enqueue(method);
		lastDepth = Depth;

		if (Depth == 0)
		{
			_methodsList.Add(_traceQueue);
      _traceQueue = new Queue<MethodInfo>();
		}
	}

	internal void CreateNewDiagnostic(TraceDiagnostic traceDiagnostic)
	{
		Diagnostics.Push(traceDiagnostic);
	}

	internal void TryToStartStopwatch()
	{
		if (!_stopwatch.IsRunning)
			_stopwatch.Start();
	}

	internal void StopwatchMark()
	{
		_savedTime.Push(_stopwatch.ElapsedMilliseconds);
	}

	internal void TryToStopTimeMeasurment()
	{
		if (_stopwatch.IsRunning)
			_stopwatch.Stop();
	}

	internal long GetTimeMeasurmentOfLastMethod()
	{
    TotalTime = Math.Max(TotalTime, _stopwatch.ElapsedMilliseconds);
		return _savedTime.Any() ? _stopwatch.ElapsedMilliseconds - _savedTime.Pop() : 0;
	}
}
