using System.Collections;
using UnityEngine;

namespace Source.Scripts
{
    public class AudioService : MonoBehaviour
    {
        [SerializeField] private AudioSource _clicksSource;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _filledSource;
        [SerializeField] private AudioClip[] _musicClips;
        [SerializeField] private AudioClip _clickSound;
        [SerializeField] private AudioClip _filledSound;
    
        private IEnumerator _musicCoroutine;
    
        private int _musicIndex;

        private void Awake()
        {
            _filledSource.clip = _filledSound;
            _musicIndex = GetRandomMusicIndex();
            _musicCoroutine = PlayMusic();
            StartCoroutine(_musicCoroutine);
        }

        public void PlayClick() => _clicksSource.PlayOneShot(_clickSound);
        public void PlayFilled() => _filledSource.Play();

        private IEnumerator PlayMusic()
        {
            _musicSource.clip = GetAudioClip();
            _musicSource.Play();
            yield return new WaitWhile(()=> _musicSource.isPlaying);
            UpdateMusicIndex();
            StartCoroutine(_musicCoroutine);
        }

        private void UpdateMusicIndex()
        {
            _musicIndex += 1;
            if(_musicIndex == _musicClips.Length)
                _musicIndex = 0;
        }

        private AudioClip GetAudioClip() => _musicClips[_musicIndex];
        private int GetRandomMusicIndex() => _musicIndex = Random.Range(0, _musicClips.Length);
    }
}
