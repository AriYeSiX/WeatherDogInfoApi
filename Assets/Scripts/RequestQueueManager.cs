using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class RequestQueueManager : MonoBehaviour
{
    private readonly Queue<Func<UniTask>> _requestQueue = new();
    private CancellationTokenSource _cts = new();
    
    public void EnqueueRequest(Func<UniTask> request)
    {
        _requestQueue.Enqueue(request);
        TryExecuteNextRequest();
    }

    public void CancelAllRequests()
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        _requestQueue.Clear();
    }

    private async void TryExecuteNextRequest()
    {
        if (_requestQueue.Count == 0)
            return;

        var request = _requestQueue.Dequeue();
        try
        {
            await request().AttachExternalCancellation(_cts.Token);
            TryExecuteNextRequest();
        }
        catch (OperationCanceledException)
        {
            // Request was canceled
        }
    }
}