using System;
using UnityEngine;

public class TapsCounter : MonoBehaviour
{
    [SerializeField] private FilledProgressHandler _filledProgressHandler;
    [SerializeField] private VideoUIPresenter _videoUIPresenter;

    public int Count { get; private set; }
    
    public event Action<int> Changed;

    private void Awake()
    {
        _videoUIPresenter.Tapped += OnTapped;
        _filledProgressHandler.Filled += OnFilled;
        _videoUIPresenter.RestartButtonClicked += OnRestartButtonClicked;
    }

    private void OnRestartButtonClicked()
    {
        _videoUIPresenter.Tapped += OnTapped;
        Count = 0;
        Changed?.Invoke(Count);
    }

    private void OnFilled()
    {
        _videoUIPresenter.Tapped -= OnTapped;
    }

    private void OnTapped()
    {
        Count++;
        Changed?.Invoke(Count);
    }
}
