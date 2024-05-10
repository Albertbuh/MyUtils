namespace Tracer.Core.Models;


///<summary>
///Store flow trace data
///</summary>
internal class ThreadTrace
{
    internal int Id;
    internal long TotalTime { get; private set; }
    private int _previousDepth = Int32.MaxValue;
    internal Stack<TraceDiagnostic> Diagnostics;
    private Stack<long> _savedTime = new Stack<long>();
    private Stopwatch _stopwatch;

    //all methods traced in thread
    private List<Queue<MethodInfo>> _methodsList = new List<Queue<MethodInfo>>();
    
    //queue per cycle (from 0 depth to n) and after that it is cleared
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

    internal int SaveMethodInfo(MethodInfo method)
    {
        //depth changed, need to save child methods
        if (Diagnostics.Count < _previousDepth)
        {
            method.ChildMethods = _traceQueue;
            _traceQueue = new Queue<MethodInfo>();
        }

        //if depth not changed just continue to add methods to queue
        _traceQueue.Enqueue(method);

        //depth = 0 means that user end his trace cycle, so add all trace queue, which 
        //store methodInfo tree to methodsList
        if (Diagnostics.Count == 0)
        {
            _methodsList.Add(_traceQueue);
            _traceQueue = new Queue<MethodInfo>();
        }
        
        _previousDepth = Diagnostics.Count;
        return Diagnostics.Count;
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
