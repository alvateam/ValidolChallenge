using System;
using UnityEngine;

public class VideoUIPresenter : MonoBehaviour, IDisposable
{
    [SerializeField] private FilledProgressHandler _filledProgressHandler;
    [SerializeField] private TapsCounter _tapsCounter;

    [SerializeField] private TapScreenView _tapScreenView;
    [SerializeField] private FinalScreenView _finalScreenView;
    [SerializeField] private VideoPlayerWrapper _videoPlayer;

    public event Action RestartButtonClicked;
    public event Action Tapped;
    
    public void Initialize(int videoNumber)
    {
        _videoPlayer.FinalVideoFinished += OnFinalVideoFinished;
        
        _finalScreenView.RestartButtonClicked += OnRestartButtonClicked;
        _finalScreenView.Initialize();
        
        _tapScreenView.Tapped += OnTapped;
        _tapScreenView.Initialize(videoNumber);
        _tapScreenView.Display();
        
        _tapsCounter.Changed += OnTapsChanged;
        _filledProgressHandler.ProgressChanged += OnFilledProgressChanged;
        _filledProgressHandler.Filled += OnProgressFilled;
    }

    private void OnProgressFilled()
    {
        _tapScreenView.Hide();
    }

    private void OnFinalVideoFinished()
    {
        _finalScreenView.SetTapsCount(_tapsCounter.Count);
        _finalScreenView.Display();
    }

    private void OnRestartButtonClicked()
    {
        _finalScreenView.Hide();
        _tapScreenView.Display();
        RestartButtonClicked?.Invoke();
    }

    private void OnTapped() => Tapped?.Invoke();

    private void OnFilledProgressChanged(float value) => 
        _tapScreenView.UpdateFilledSlider(value);

    private void OnTapsChanged(int value) => 
        _tapScreenView.SetTapsCount(value);

    public void Dispose()
    {
        _videoPlayer.FinalVideoFinished -= OnFinalVideoFinished;
        _filledProgressHandler.ProgressChanged -= OnFilledProgressChanged;
        _finalScreenView.RestartButtonClicked -= OnRestartButtonClicked;
        _tapScreenView.Tapped -= OnTapped;
        _tapsCounter.Changed -= OnTapsChanged;
        _finalScreenView?.Dispose();
    }
}