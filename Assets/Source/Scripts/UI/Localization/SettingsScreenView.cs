using System;
using TMPro;
using UnityEngine;

public class SettingsScreenView : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _localizationDropdown;
    
    public event Action<int> LocalesChanged;

    private void OnEnable()
    {
        _localizationDropdown.onValueChanged.AddListener(OnLocaleChanged);
    }
    
    private void OnDisable()
    {
        _localizationDropdown.onValueChanged.AddListener(OnLocaleChanged);
    }

    private void OnLocaleChanged(int arg0)
    {
        LocalesChanged?.Invoke(arg0);
    }
}