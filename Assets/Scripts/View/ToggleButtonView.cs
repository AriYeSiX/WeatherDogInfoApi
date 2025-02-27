using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonView : AbstractButtonView
{
    [SerializeField] private bool _isActive;
    public bool IsActive => _isActive;

    [SerializeField] private GameObject _tabObject;
    
    private Image _image;
    public event Action<ToggleButtonView> OnClick;
    protected override void OnValidate()
    {
        base.OnValidate();
        var image = GetComponent<Image>();
        if (image!=null)
        {
            _image = image;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (_isActive)
        {
            ShowAnimation();   
        }
        else
        {
            HideAnimation();
        }
    }

    protected override void OnClickEvent()
    {
        OnClick?.Invoke(this);
    }
    
    public void HideAnimation()
    {
        FadeAnimation(0.5f, 0.1f);
        _isActive = false;
        _tabObject.SetActive(_isActive);
    }
    
    public void ShowAnimation()
    {
        FadeAnimation(1f, 0.1f);
        _isActive = true;
        _tabObject.SetActive(_isActive);
    }

    private void FadeAnimation(float endValue, float duration)
    {
        _image.DOFade(endValue, duration);
    }
}