﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pixsper.Cueordinator.Services;

public class LoopTask : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Task _task;

    public LoopTask(Func<CancellationToken, Task> loopFunc)
    {
        _task = loopImplAsync(loopFunc);
    }


    public async ValueTask DisposeAsync()
    {
        _cancellationTokenSource.Cancel();
        await _task.ConfigureAwait(false);
        ;           _cancellationTokenSource.Dispose();
    }

    private async Task loopImplAsync(Func<CancellationToken, Task> taskFunc)
    {
        try
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await taskFunc(_cancellationTokenSource.Token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }
}