using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class WeatherTabController : MonoBehaviour
{
    [SerializeField, Inject] private RequestQueueManager _requestQueueManager;
    private bool _isTabActive;
    private Subject<WeatherData> _onGetWeather;

    public IObservable<WeatherData> OnGetWeatherAsObservable()
    {
        return _onGetWeather ??= new Subject<WeatherData>();
    }

    public void OnTabSelected()
    {
        _isTabActive = true;
        StartWeatherPolling();
    }

    public void OnTabDeselected()
    {
        _isTabActive = false;
        _requestQueueManager.CancelAllRequests();
    }

    private async void StartWeatherPolling()
    {
        var firstCall = true;
        while (_isTabActive)
        {
            if (!firstCall)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(5f));
            }

            firstCall = false;
            
            if (_isTabActive)
            {
                _requestQueueManager.EnqueueRequest(() => GetWeatherForecast());
            }
        }
    }

    private async UniTask<WeatherData> GetWeatherForecast()
    {
        using var www = UnityWebRequest.Get("https://api.weather.gov/gridpoints/TOP/32,81/forecast");
        await www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching weather data: " + www.error);
            throw new Exception("Failed to fetch weather data");
        }

        var jsonData = www.downloadHandler.text;
        var weatherData = JsonConvert.DeserializeObject<WeatherData>(jsonData);
        _onGetWeather?.OnNext(weatherData);
        
        return weatherData;
    }
}

[Serializable]
public class WeatherData
{
    public Properties Properties;
}

[Serializable]
public class Properties
{
    public Period[] Periods;
}

[Serializable]
public class Period
{
    public int Number;
    public string StartTime;
    public string EndTime;
    public int Temperature;
    public string TemperatureUnit;
    public string Icon;
}