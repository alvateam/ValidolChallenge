using System;
using UnityEngine;

public class ClicksCounter : MonoBehaviour
{
    [SerializeField] private InputActionsHandler _inputActionsHandler;
    [SerializeField] private FilledProgressHandler _filledProgressHandler;

    private int _count;
    
    public event Action<int> Changed;

    private void Awake()
    {
        _inputActionsHandler.Clicked += OnClicked;
        _filledProgressHandler.Filled += OnFilled;
        _inputActionsHandler.RestartButtonClicked += OnRestartButtonClicked;
    }

    private void OnRestartButtonClicked()
    {
        _inputActionsHandler.Clicked -= OnClicked;
        _inputActionsHandler.Clicked += OnClicked;
        _count = 0;
        Changed?.Invoke(_count);
    }

    private void OnFilled()
    {
        _inputActionsHandler.Clicked -= OnClicked;
    }

    private void OnClicked()
    {
        _count++;
        Changed?.Invoke(_count);
    }
}
