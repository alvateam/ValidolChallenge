using System;
using TMPro;
using UnityEngine;

namespace Source.Scripts.UI.Localization
{
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
}