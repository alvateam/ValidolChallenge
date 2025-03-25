using System;
using UnityEngine;

public class FilledProgressHandler : MonoBehaviour
{
    private const float MaxEnergy = 100f;
    
    [SerializeField] private InputActionsHandler _inputActionsHandler;

    private float _energyGain;
    private float _difficultyMultiplier;
    private float _currentEnergy;
    private bool _isFilled;

    public event Action Filled;
    public event Action<float> ProgressChanged;

    public void Initialize(float energyGain, float difficultyMultiplier)
    {
        _energyGain = energyGain;
        _difficultyMultiplier = difficultyMultiplier;
        
        _inputActionsHandler.Clicked += OnClicked;
        _inputActionsHandler.RestartButtonClicked += OnRestartButtonClicked;
    }

    private void OnRestartButtonClicked()
    {
        _inputActionsHandler.Clicked -= OnClicked;
        _inputActionsHandler.Clicked += OnClicked;
        _currentEnergy = 0;
        _isFilled = false;
        UpdateProgress();
    }

    private void Update()
    {
        if (!_isFilled)
            UpdateEnergyDrain();
    }

    private void OnClicked()
    {
        _currentEnergy = Mathf.Min(_currentEnergy + _energyGain, MaxEnergy);
        UpdateProgress();
        
        if (_currentEnergy >= MaxEnergy)
        {
            _inputActionsHandler.Clicked -= OnClicked;
            _isFilled = true;
            Filled?.Invoke();
        }
    }

    private void UpdateEnergyDrain()
    {
        if (_currentEnergy > 0)
        {
            float difficulty = GetDifficultyFactor();
            _currentEnergy = Mathf.Max(_currentEnergy - difficulty * Time.deltaTime * 10f, 0f);

            UpdateProgress();
        }
    }

    private float GetDifficultyFactor() =>
        1f + _currentEnergy / MaxEnergy * (_difficultyMultiplier - 1f);

    private void UpdateProgress()
    {
        var progress = _currentEnergy / MaxEnergy;
        ProgressChanged?.Invoke(progress);
    }
}