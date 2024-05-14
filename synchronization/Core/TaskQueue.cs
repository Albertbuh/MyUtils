namespace MyUtils.Synchronization.Core;

public class TaskQueue
{
    private List<Thread> _threads = new();
    private Queue<Action?> _tasks = new();
    private object _locker = new();

    public TaskQueue(int threadsAmount)
    {
        for (int i = 0; i < threadsAmount; i++)
        {
            var thread = new Thread(ExecuteThread) { IsBackground = true };
            thread.Start();
            _threads.Add(thread);
        }
    }

    public void EnqueueTask(Action? task)
    {
        lock (_locker)
        {
            _tasks.Enqueue(task);
            Monitor.Pulse(_tasks);
        }
    }

    private Action? DequeueTask()
    {
        lock (_locker)
        {
            while (_tasks.Count == 0)
                Monitor.Wait(_tasks);
            return _tasks.Dequeue();
        }
    }

    private void ExecuteThread()
    {
        while (true)
        {
            var task = DequeueTask();
            if (task != null)
            {
                try
                {
                    task();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                break;
            }
        }
    }

    public void Close()
    {
        foreach (var t in _threads)
        {
            EnqueueTask(null);
            t.Join();
        }
    }
}
