using System;
using TMPro;
using UnityEngine;

public class FactButtonView : AbstractButtonView
{
    [SerializeField] private TMP_Text _factNumber;
    [SerializeField] private TMP_Text _factName;
    
    private string _id;

    public event Action<string> OnFactClick;
    
    protected override void OnClickEvent()
    {
        OnFactClick?.Invoke(_id);
    }

    public void WriteText(string factNumber, string factName, string id)
    {
        _factNumber.text = factNumber;
        _factName.text = factName;
        _id = id;
    }
}