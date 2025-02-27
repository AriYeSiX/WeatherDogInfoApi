using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FactPopupView : MonoBehaviour
{
    [SerializeField, Inject] private FactsTabController _factsTabController;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private Button _button;

    private CompositeDisposable _disposables = new();

    private bool _needFirstEnterLayoutFix;
    private void Awake()
    {
        _button.onClick.AddListener(HidePopUp);
        _disposables.Add(_factsTabController.OnCancelFactByIdAsObservable().Subscribe(HidePopUp));
        _needFirstEnterLayoutFix = true;
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(HidePopUp);
        _disposables.Dispose();
        _disposables = new CompositeDisposable();
    }

    private void HidePopUp()
    {
        gameObject.SetActive(false);
    }

    private void HidePopUp(string temp)
    {
        HidePopUp();
    }

    public void ShowPopup(string titleText, string descriptionText)
    {
        _title.text = titleText;
        _description.text = descriptionText;
        gameObject.SetActive(true);

        if (!_needFirstEnterLayoutFix)
        {
            return;
        }
        
        _needFirstEnterLayoutFix = false;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}