using System.Runtime.InteropServices;

namespace MyUtils.Memory.Core;

public class NativeBuffer : IDisposable
{
    private IntPtr _handler;
    public IntPtr Handler
    {
        get
        {
            if (!disposed)
                return _handler;
            else
                throw new ObjectDisposedException("handler");
        }
    }
    private bool disposed;

    public NativeBuffer(int size)
    {
        _handler = Marshal.AllocHGlobal(size);
    }

    ~NativeBuffer()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool state)
    {
        if (_handler != IntPtr.Zero)
            Marshal.FreeHGlobal(_handler);
    }

    public void Dispose()
    {
        if (!disposed)
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}
