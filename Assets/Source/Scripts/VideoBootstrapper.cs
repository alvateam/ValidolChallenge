using Data;
using UnityEngine;
using UnityEngine.Serialization;

public class VideoBootstrapper : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private VideoPlayerWrapper _videoPlayer;
    [SerializeField] private FilledProgressHandler _filledProgressHandler;
    [FormerlySerializedAs("videoUIView")] [FormerlySerializedAs("_videoUI")] [SerializeField] private VideoUIPresenter videoUIPresenter;

    public RectTransform RectTransform => _rectTransform;

    public void Initialize(VideoData data, int videoNumber)
    {
        _videoPlayer.Initialize(data.LoopedVideo, data.FinalVideo);
        _filledProgressHandler.Initialize(data.EnergyGain, data.DifficultyMultiplier);
        videoUIPresenter.Initialize(videoNumber);
    }
    
}