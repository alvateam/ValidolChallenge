using UnityEngine;

public class SettingsScreenPresenter : MonoBehaviour
{
    [SerializeField] private SettingsScreenView _view;
    [SerializeField] private LocalizationSystem _localizationSystem;

    private void OnEnable()
    {
        _view.LocalesChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        _view.LocalesChanged -= OnLocaleChanged;
    }
    
    private void OnLocaleChanged(int id)
    {
        _localizationSystem.SetLocale(id);
    }
}