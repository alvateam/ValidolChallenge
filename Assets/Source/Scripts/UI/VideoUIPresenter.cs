using System;
using UnityEngine;

public class VideoUIPresenter : MonoBehaviour, IDisposable
{
    [SerializeField] private FilledProgressHandler _filledProgressHandler;
    [SerializeField] private TapsCounter _tapsCounter;

    [SerializeField] private TapScreenView _tapScreenView;
    [SerializeField] private FinalScreenView _finalScreenView;
    [SerializeField] private VideoPlayerWrapper _videoPlayer;
    [SerializeField] private GradeHandler _gradeHandler;

    public event Action RestartButtonClicked;
    public event Action Tapped;
    
    public void Initialize(int videoId)
    {
        _videoPlayer.FinalVideoFinished += OnFinalVideoFinished;
        
        _finalScreenView.RestartButtonClicked += OnRestartButtonClicked;
        _finalScreenView.Initialize();
        
        _tapScreenView.Tapped += OnTapped;
        _tapScreenView.Initialize(videoId);
        _tapScreenView.Display();
        
        _tapsCounter.Changed += OnTapsChanged;
        _filledProgressHandler.ProgressChanged += OnFilledProgressChanged;
        _filledProgressHandler.Filled += OnProgressFilled;

        _gradeHandler.GradeCalculated += OnGradeCalculated;
    }

    private void OnGradeCalculated(int value) => 
        _finalScreenView.SetGrade(value);

    private void OnProgressFilled() => 
        _tapScreenView.Hide();

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