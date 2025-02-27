using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class FactsTabView : MonoBehaviour
{
    [SerializeField, Inject] private FactsTabController _factsTabController;
    [SerializeField, Inject] private FactPopupView _factPopup;
    
    [SerializeField] private GameObject _loader;
    [SerializeField] private Transform _factsParent;
    [SerializeField] private FactButtonView _factButtonPrefab;
    
    private List<FactButtonView> _factButtons = new List<FactButtonView>();
    private List<Fact> _facts = new List<Fact>();
    private CompositeDisposable _disposables = new CompositeDisposable();
    
    private void OnEnable()
    {
        _loader.SetActive(true);
        _factsTabController.OnTabSelected();
        _disposables.Add(_factsTabController.OnGetFactsAsObservable().Subscribe(FetchFacts));
        _disposables.Add(_factsTabController.OnGetFactByIdAsObservable().Subscribe(ShowBreedDetails));
    }

    private void OnDisable()
    {
        _factsTabController.OnTabDeselected();
        _disposables.Dispose();
        _disposables = new CompositeDisposable();
        DestroyAllButtons();
    }

    private void FetchFacts(FactsArray factsArray)
    {
        _facts = factsArray.Data;
        DisplayBreeds(10);
        _loader.SetActive(false);
    }
    private void DestroyAllButtons()
    {
        if (_factButtons.Count <= 0) return;

        for (int i = _factButtons.Count-1; i >= 0; i--)
        {
            _factButtons[i].OnFactClick -= RequestShowBreed;
            DestroyImmediate(_factButtons[i].gameObject);
        }
        _factButtons.Clear();
    }
    
    private void DisplayBreeds(int length)
    {
        for (int i = 0; i < length; i++)
        {
            var btn = Instantiate(_factButtonPrefab, _factsParent);
            
            btn.WriteText((i + 1).ToString() , " - " + _facts[i].Attributes.Name, _facts[i].Id);
            btn.OnFactClick+=RequestShowBreed;
            _factButtons.Add(btn);
        }
    }

    private void RequestShowBreed(string id)
    {
        _loader.SetActive(true);
        _factsTabController.OnFactSelected(id);
    }
    
    private void ShowBreedDetails(Fact fact)
    {
        _factPopup.ShowPopup(fact.Attributes.Name,fact.Attributes.Description);
        _loader.SetActive(false);
    }
}