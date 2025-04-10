using UnityEngine;

public class LocalizationScreenPresenter : MonoBehaviour
{
    [SerializeField] private LocalizationScreenView _view;
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