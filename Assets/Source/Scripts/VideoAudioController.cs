using UnityEngine;

public class VideoAudioController : MonoBehaviour
{
    [SerializeField] private VideoUIPresenter _videoUIPresenter;
    [SerializeField] private VideoPlayerWrapper _videoPlayerWrapper;
    
    private AudioService _audioService;

    public void Initialize(AudioService audioService)
    {
        _audioService = audioService;
        _videoUIPresenter.Tapped += OnTapped;
        _videoPlayerWrapper.FinalVideoStarted += OnFinalVideoStarted;
    }

    private void OnFinalVideoStarted()
    {
        _audioService.PlayFilled();
    }

    private void OnTapped()
    {
        _audioService.PlayClick();
    }
}
