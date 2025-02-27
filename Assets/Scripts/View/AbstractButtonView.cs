using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class AbstractButtonView : MonoBehaviour
{
    [SerializeField] private Button _button;

    protected virtual void OnValidate()
    {
        if (Application.isEditor)
        {
            _button = GetComponent<Button>();
        }
    }

    protected virtual void Awake()
    {
        _button.onClick.AddListener(OnClickEvent);
    }

    protected void OnDestroy()
    {
        _button.onClick.RemoveListener(OnClickEvent);
    }

    protected abstract void OnClickEvent();
}