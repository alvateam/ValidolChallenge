using System;
using UnityEngine;

public class FilledProgressHandler : MonoBehaviour
{
    private const float MaxEnergy = 100f;
    
    [SerializeField] private VideoUIPresenter _videoUIPresenter;

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
        
        _videoUIPresenter.Tapped += OnTapped;
        _videoUIPresenter.RestartButtonClicked += OnRestartButtonClicked;
    }

    private void OnRestartButtonClicked()
    {
        _videoUIPresenter.Tapped += OnTapped;
        _currentEnergy = 0;
        _isFilled = false;
        UpdateProgress();
    }

    private void Update()
    {
        if (!_isFilled)
            UpdateEnergyDrain();
    }

    private void OnTapped()
    {
        _currentEnergy = Mathf.Min(_currentEnergy + _energyGain, MaxEnergy);
        UpdateProgress();
        
        if (_currentEnergy >= MaxEnergy)
        {
            _videoUIPresenter.Tapped -= OnTapped;
            _isFilled = true;
            Filled?.Invoke();
        }
    }

    private void UpdateEnergyDrain()
    {
        if (_currentEnergy > 0)
        {
            float drainRate = GetDrainRate();
            _currentEnergy = Mathf.Max(_currentEnergy - drainRate, 0f);

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

    private float GetDrainRate()
    {
        return GetDifficultyFactor() * Time.deltaTime * 10f;
    }
}