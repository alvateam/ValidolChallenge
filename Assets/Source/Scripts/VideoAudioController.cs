using UnityEngine;
using UnityEngine.Serialization;

public class VideoAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clickClip;
    [SerializeField] private AudioClip _filledClip;
    
    [SerializeField] private VideoUIPresenter _videoUIPresenter;
    [SerializeField] private VideoPlayerWrapper _videoPlayerWrapper;

    private void Awake()
    {
        _videoUIPresenter.Tapped += OnTapped;
        _videoPlayerWrapper.FinalVideoStarted += OnFinalVideoStarted;
    }

    private void OnFinalVideoStarted()
    {
        _audioSource.PlayOneShot(_filledClip);
    }

    private void OnTapped()
    {
        _audioSource.PlayOneShot(_clickClip);
    }
}
