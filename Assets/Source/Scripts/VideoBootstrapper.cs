using Source.Scripts.Downloading;
using Source.Scripts.UI;
using UnityEngine;
using UnityEngine.Video;

namespace Source.Scripts
{
    public class VideoBootstrapper : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private VideoPlayerWrapper _videoPlayer;
        [SerializeField] private FilledProgressHandler _filledProgressHandler;
        [SerializeField] private VideoUIPresenter _videoUIPresenter;
        [SerializeField] private GradeHandler _gradeHandler;
        [SerializeField] private TapsCounter _tapsCounter;
        [SerializeField] private VideoAudioController _videoAudioController;
    
        public VideoJsonData Data {get; private set;}
        public TapsCounter TapsCounter => _tapsCounter;
        public VideoPlayerWrapper VideoPlayerWrapper => _videoPlayer;

        public RectTransform RectTransform => _rectTransform;
    
        public void Initialize(VideoJsonData data, string loopedVideo, string finalVideo, int videoId,
            AudioService audioService, VideoPlayer videoPlayer)
        {
            Data = data;
            _videoPlayer.Initialize(loopedVideo, finalVideo, videoPlayer);
            _filledProgressHandler.Initialize(data.EnergyGain, data.DifficultyMultiplier);
            _videoUIPresenter.Initialize(videoId);
            _gradeHandler.Initialize(data.IdealClicks);
            _videoAudioController.Initialize(audioService);
        }
    }
}