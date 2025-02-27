using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TogglePanelView : MonoBehaviour
{
    [SerializeField] private List<ToggleButtonView> _toggleButtonViews = new List<ToggleButtonView>();

    [Inject]
    private void Construct(List<ToggleButtonView> toggleButtonViews)
    {
        _toggleButtonViews = toggleButtonViews;
        foreach (var toggleButtonView in toggleButtonViews)
        {
            toggleButtonView.OnClick += OnToggleButtonCLick;
        }
    }
    
    private void OnDestroy()
    {
        foreach (var toggleButtonView in _toggleButtonViews)
        {
            toggleButtonView.OnClick -= OnToggleButtonCLick;
        }
    }

    private void OnToggleButtonCLick(ToggleButtonView toggleButtonView)
    {
        if (toggleButtonView.IsActive)
        {
            return;
        }
        
        toggleButtonView.ShowAnimation();

        foreach (var buttonView in _toggleButtonViews)
        {
            if (buttonView!=toggleButtonView && buttonView.IsActive)
            {
                buttonView.HideAnimation();
            }
        }

    }
}