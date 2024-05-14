namespace MyUtils.Synchronization.Core;

public class ActionRunner
{
    int _runningCount;
    object _sync = new();

    public void RunAndWaitAll(Action[] actions)
    {
        _runningCount = actions.Length;
        foreach (var action in actions)
            ThreadPool.QueueUserWorkItem(ExecuteThread, action);

        // block thread until all actions finish execution
        lock (_sync)
            if (_runningCount > 0)
                Monitor.Wait(_sync);
    }

    public void ExecuteThread(object? state)
    {
        var action = state as Action;
        if (action != null)
        {
            action();
            lock (_sync)
            {
                _runningCount--;
                // when last action execution finished send signal to
                // unblock thread
                if (_runningCount == 0)
                    Monitor.Pulse(_sync);
            }
        }
    }
}
