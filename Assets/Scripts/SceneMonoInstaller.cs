using UnityEngine;
using Zenject;

public class SceneMonoInstaller : MonoInstaller
{
    [SerializeField] private ToggleButtonView _weatherToggle;
    [SerializeField] private ToggleButtonView _factsToggle;
    
    public override void InstallBindings()
    {
        Container.Bind<RequestQueueManager>().FromComponentInHierarchy(true).AsSingle();
        Container.Bind<WeatherTabController>().FromComponentInHierarchy(true).AsSingle();
        Container.Bind<FactsTabController>().FromComponentInHierarchy(true).AsSingle();
        
        Container.Bind<ToggleButtonView>().FromInstance(_weatherToggle);
        Container.Bind<ToggleButtonView>().FromInstance(_factsToggle);
        
        Container.Bind<FactPopupView>().FromComponentInHierarchy(true).AsSingle();
    }
}