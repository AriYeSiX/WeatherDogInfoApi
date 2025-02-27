using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;

public class WeatherPanelView : MonoBehaviour
{
    [SerializeField, Inject] private WeatherTabController _weatherTabController;
    
    [SerializeField] private TMP_Text _weatherText;
    [SerializeField] private Image _weatherImage;

    private CompositeDisposable _disposables = new();

    private void OnEnable()
    {
        _weatherTabController.OnTabSelected();
        _disposables.Add(_weatherTabController.OnGetWeatherAsObservable().Subscribe(FetchWeather));
    }

    private void OnDisable()
    {
        _weatherTabController.OnTabDeselected();
        _disposables.Dispose();
        _disposables = new CompositeDisposable();
    }

    private async void FetchWeather(WeatherData weatherData)
    {
        if (weatherData.Properties is {Periods: {Length: > 0}})
        {
            var currentTime = DateTime.Now;
            var todayPeriod = weatherData.Properties.Periods
                .FirstOrDefault(x=>Convert.ToDateTime(x.StartTime) < currentTime 
                                   && currentTime < Convert.ToDateTime(x.EndTime));
            
            var temperature = $"{todayPeriod.Temperature}{todayPeriod.TemperatureUnit}";
            var iconUrl = todayPeriod.Icon;

            _weatherText.text = $"Today - {temperature}";

            using var www = UnityWebRequestTexture.GetTexture(iconUrl);
            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                var iconTexture = DownloadHandlerTexture.GetContent(www);
                _weatherImage.sprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Error downloading icon: " + www.error);
            }
        }
        else
        {
            _weatherText.text = "No weather data available.";
        }
    }
}