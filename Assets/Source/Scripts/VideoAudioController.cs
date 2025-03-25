using UnityEngine;

public class VideoAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clickClip;
    [SerializeField] private AudioClip _filledClip;
    
    [SerializeField] private ClicksCounter _clicksCounter;
    [SerializeField] private FilledProgressHandler _filledProgressHandler;

    private void Awake()
    {
        _clicksCounter.Changed += OnClicked;
        _filledProgressHandler.Filled += OnFilled;
    }

    private void OnFilled()
    {
        _audioSource.PlayOneShot(_filledClip);
    }

    private void OnClicked(int obj)
    {
        _audioSource.PlayOneShot(_clickClip);
    }
}
