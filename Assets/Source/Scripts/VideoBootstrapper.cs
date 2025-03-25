using Data;
using TS.PageSlider;
using UnityEngine;

public class VideoBootstrapper : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private VideoPlayerWrapper _videoPlayer;
    [SerializeField] private FilledProgressHandler _filledProgressHandler;
    [SerializeField] private VideoUIPresenter _videoUIPresenter;
    [SerializeField] private GradeHandler _gradeHandler;
    [SerializeField] private TapsCounter _tapsCounter;
    
    public TapsCounter TapsCounter => _tapsCounter;
    public VideoPlayerWrapper VideoPlayerWrapper => _videoPlayer;

    public RectTransform RectTransform => _rectTransform;

    public void Initialize(VideoData data, int videoNumber)
    {
        _videoPlayer.Initialize(data.LoopedVideo, data.FinalVideo);
        _filledProgressHandler.Initialize(data.EnergyGain, data.DifficultyMultiplier);
        _videoUIPresenter.Initialize(videoNumber);
        _gradeHandler.Initialize(data.IdealClicks);
    }
}