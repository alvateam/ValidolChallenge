using Data;
using UnityEngine;

public class VideoBootstrapper : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private VideoPlayerWrapper _videoPlayer;
    [SerializeField] private FilledProgressHandler _filledProgressHandler;
    [SerializeField] private VideoUI _videoUI;

    public RectTransform RectTransform => _rectTransform;

    public void Initialize(VideoData data, int videoNumber)
    {
        _videoPlayer.Initialize(data.LoopedVideo, data.FinalVideo);
        _filledProgressHandler.Initialize(data.EnergyGain, data.DifficultyMultiplier);
        _videoUI.Initialize(videoNumber);
    }
    
}