using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteMonitoringClient.Base;

public class ByteArrayPool
{
    private static readonly ConcurrentQueue<byte[]> Pool = new();
    private const int MaxQueue = 30;
    private static readonly SemaphoreSlim Semaphore = new(0, MaxQueue);

    public static async Task<byte[]> RentAsync(int size, CancellationToken cancellationToken = default)
    {
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
        if (await Semaphore.WaitAsync(0, cancellationToken))
        {
            if (Pool.TryDequeue(out var arr))
                return arr;
        }
        return new byte[size];
    }

    public static byte[] Rent(int size)
    {
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));
        if (Semaphore.Wait(0))
        {
            if (Pool.TryDequeue(out var arr))
                return arr;
        }
        return new byte[size];
    }

    public static void Return(byte[] arr)
    {
        if (arr == null) throw new ArgumentNullException(nameof(arr));
        Array.Clear(arr, 0, arr.Length);
        if (Pool.Count < MaxQueue)
        {
            Pool.Enqueue(arr);
            Semaphore.Release();
        }
    }
}