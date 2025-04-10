using System;
using TMPro;
using UnityEngine;

public class LocalizationScreenView : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _dropdown;
    
    public event Action<int> LocalesChanged;

    private void OnEnable()
    {
        _dropdown.onValueChanged.AddListener(OnValueChanged);
    }
    
    private void OnDisable()
    {
        _dropdown.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(int arg0)
    {
        LocalesChanged?.Invoke(arg0);
    }
}