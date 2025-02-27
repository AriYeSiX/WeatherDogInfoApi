using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class FactsTabController : MonoBehaviour
{
    private const string FACTS_REQUEST = "https://dogapi.dog/api/v2/breeds";
    private const string FACT_BY_ID_REQUEST = "https://dogapi.dog/api/v2/breeds/{id}";
    
    [SerializeField, Inject] private RequestQueueManager _requestQueueManager;
    
    private CancellationTokenSource _factsCts = new();
    
    private CancellationTokenSource _cts = new CancellationTokenSource();

    private Subject<FactsArray> _onGetFacts;

    public IObservable<FactsArray> OnGetFactsAsObservable()
    {
        return _onGetFacts ??= new Subject<FactsArray>();
    }
    
    private Subject<Fact> _onGetFactById;

    public IObservable<Fact> OnGetFactByIdAsObservable()
    {
        return _onGetFactById ??= new Subject<Fact>();
    }
    
    private Subject<string> _onCancelFactById;

    public IObservable<string> OnCancelFactByIdAsObservable()
    {
        return _onCancelFactById ??= new Subject<string>();
    }
    
    public void OnTabSelected()
    {
        FetchFactsList();
    }

    public void OnTabDeselected()
    {
        _factsCts.Cancel();
        _requestQueueManager.CancelAllRequests();
    }

    private void FetchFactsList()
    {
        _factsCts = new CancellationTokenSource();
        _requestQueueManager.EnqueueRequest(() => GetFacts().AttachExternalCancellation(_factsCts.Token));
    }

    public void OnFactSelected(string factId)
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        _onCancelFactById?.OnNext(factId);
        _requestQueueManager.CancelAllRequests();
        _requestQueueManager.EnqueueRequest(() => GetFactById(factId, _cts.Token));
    }

    private async UniTask<FactsArray> GetFacts()
    {
        using var www = UnityWebRequest.Get(FACTS_REQUEST);
        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching breeds: " + www.error);
            throw new Exception("Failed to fetch breeds data");
        }

        var jsonData = www.downloadHandler.text;
        var factsResponse = JsonConvert.DeserializeObject<FactsArray>(jsonData);
        _onGetFacts?.OnNext(factsResponse);

        return factsResponse;
    }

    private async UniTask<SingleFactResponse> GetFactById(string id, CancellationToken cancellationToken)
    {
        var endpoint = FACT_BY_ID_REQUEST.Replace("{id}", id);
        using var www = UnityWebRequest.Get(endpoint);
        await www.SendWebRequest().WithCancellation(cancellationToken);

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching breed details: " + www.error);
            throw new Exception("Failed to fetch breed details");
        }

        var jsonData = www.downloadHandler.text;
        var factDetails = JsonConvert.DeserializeObject<SingleFactResponse>(jsonData);
        _onGetFactById?.OnNext(factDetails.Data);

        return factDetails;
    }
}

[Serializable]
public class SingleFactResponse
{
    public Fact Data { get; set; }
}

[Serializable]
public class FactsArray
{
    public List<Fact> Data { get; set; }
}

[Serializable]
public class Fact
{
    public string Id { get; set; }
    public Attributes Attributes { get; set; }
}

[Serializable]
public class Attributes
{
    public string Name { get; set; }
    public string Description { get; set; }
}