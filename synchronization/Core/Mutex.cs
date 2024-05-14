namespace MyUtils.Synchronization.Core;

public class Mutex
{
   Thread? thread;

   public void Lock()
   {
       var t = Thread.CurrentThread;
       while(Interlocked.CompareExchange(ref thread, t, null) != null)
           Thread.Yield();
       Thread.MemoryBarrier();
   }

   public void Unlock()
   {
       var t = Thread.CurrentThread;
       if(Interlocked.CompareExchange(ref thread, null, t) != t)
            throw new SynchronizationLockException();
        Thread.MemoryBarrier();
   }
}
