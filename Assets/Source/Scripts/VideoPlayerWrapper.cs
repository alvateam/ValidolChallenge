using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerWrapper : MonoBehaviour
{
    private const float MinPlayerPlaybackSpeed = 1;
    private const float MaxPlayerPlaybackSpeed = 10;
    private const float RawImageAfterDelay = 0.1f;
    
    [SerializeField] private RawImage _rawImage;
    [SerializeField] private VideoUIPresenter _videoUIPresenter;
    [SerializeField] private FilledProgressHandler _filledProgressHandler;
    [SerializeField] private AspectRatioFitter _aspectRatioFitter;

    private VideoPlayer _videoPlayer;
    
    private string _loopedVideoClipUrl;
    private string _finalVideoClipUrl;
    
    private Texture _placeholderTexture;
    private RenderTexture _renderTexture;

    public bool IsReady {get; private set;}
    
    public event Action FinalVideoStarted;
    public event Action FinalVideoFinished; 
    
    public void Initialize(string loopedVideo, string finalVideo, VideoPlayer videoPlayer)
    {
        _videoPlayer = videoPlayer;
        _loopedVideoClipUrl = loopedVideo;
        _finalVideoClipUrl = finalVideo;
    }

    public void Prepare()
    {
        SetVideoPlayerUrl(_finalVideoClipUrl);
        _videoPlayer.sendFrameReadyEvents = true;
        
        InitializeRenderTexture();
        
        _videoPlayer.targetTexture = _renderTexture;
        SetRawImageTexture(_renderTexture);
        
        _videoPlayer.frameReady += OnFrameReady;
        _videoPlayer.prepareCompleted += OnPrepareCompleted;
        _videoPlayer.Prepare();
    }

    public void RestartPlayer()
    {
        StopPlayer();
        StartPlayer();
    }
    
    public void StartPlayer()
    {
        _filledProgressHandler.Filled += OnFilled;
        _videoUIPresenter.Tapped += OnTapped;
        _videoUIPresenter.RestartButtonClicked += OnRestartButtonClicked;
        
        SetVideoPlayerUrl(_loopedVideoClipUrl);
        _videoPlayer.targetTexture = _renderTexture;
    }

    public void StopPlayer()
    {
        _filledProgressHandler.Filled -= OnFilled;
        _videoUIPresenter.Tapped -= OnTapped;
        _videoUIPresenter.RestartButtonClicked -= OnRestartButtonClicked;
    }
    
    private void InitializeRenderTexture()
    {
        _renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32)
        {
            useMipMap = false,
            autoGenerateMips = false,
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
        };
    }
    
    private void OnPrepareCompleted(VideoPlayer source)
    {
        _videoPlayer.prepareCompleted -= OnPrepareCompleted;
        _videoPlayer.Play();
    }

    private void OnFrameReady(VideoPlayer videoPlayer, long frameIndex)
    {
        if (frameIndex == 1 && _placeholderTexture == null)
        {
            videoPlayer.frameReady -= OnFrameReady;
            _placeholderTexture = CreateTextureFromFrame(videoPlayer.targetTexture);
            SetRawImageTexture(_placeholderTexture);
            _videoPlayer.Stop();
            IsReady = true;
        }
    }
    
    private Texture CreateTextureFromFrame(RenderTexture renderTexture)
    {
        RenderTexture.active = renderTexture;
        
        Texture2D texture = new Texture2D(
            renderTexture.width,
            renderTexture.height,
            TextureFormat.RGBA32,
            false
        );
        
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        return texture;
    }
    
    private System.Collections.IEnumerator UpdateRawImageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetRawImageTexture(_videoPlayer.targetTexture);
    }

    private void RestartVideo()
    {
        if (_videoPlayer.isPlaying)
        {
            if (_videoPlayer.playbackSpeed < MaxPlayerPlaybackSpeed)
            {
                SetPlaybackSpeed(MaxPlayerPlaybackSpeed);
                _videoPlayer.loopPointReached += OnRestartVideoPointReached;  
            }
        }
        else
        {
            OnRestartVideoPointReached(_videoPlayer);
        }
    }

    private void OnRestartVideoPointReached(VideoPlayer source)
    {
        SetPlaybackSpeed(MinPlayerPlaybackSpeed);
        _videoPlayer.loopPointReached -= OnRestartVideoPointReached;
        SetRawImageTexture(_placeholderTexture);
        
        SetVideoPlayerUrl(_loopedVideoClipUrl);
        
        if (_videoPlayer.isPrepared)
        {
            OnVideoPrepared(_videoPlayer);
        }
        else
        {
            _videoPlayer.prepareCompleted += OnVideoPrepared;
            _videoPlayer.Prepare(); 
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        _videoPlayer.prepareCompleted -= OnVideoPrepared;
        _videoPlayer.Play();
        StartCoroutine(UpdateRawImageAfterDelay(RawImageAfterDelay));
    }

    private void OnRestartButtonClicked()
    {
        _filledProgressHandler.Filled += OnFilled;
        _videoUIPresenter.Tapped += OnTapped;

        _videoPlayer.frameReady -= OnFrameReady;
        _videoPlayer.loopPointReached -= OnLoopVideoFinished;
        _videoPlayer.prepareCompleted -= OnLoopVideoFinished;
        _videoPlayer.prepareCompleted -= OnPrepareCompleted;
        _videoPlayer.prepareCompleted -= OnFinalVideoPrepared;
        
        RestartVideo();
    }

    private void OnFilled()
    {
        _videoUIPresenter.Tapped -= OnTapped;
        _filledProgressHandler.Filled -= OnFilled;

        if (_videoPlayer.isPlaying)
            _videoPlayer.loopPointReached += OnLoopVideoFinished;
        else
            PlayFinalVideo();
    }

    private void OnLoopVideoFinished(VideoPlayer source)
    {
        _videoPlayer.loopPointReached -= OnLoopVideoFinished;
        PlayFinalVideo();
    }

    private void PlayFinalVideo()
    {
        if (_videoPlayer.isPrepared)
        {
            OnFinalVideoPrepared(_videoPlayer);
        }
        else
        {
            _videoPlayer.prepareCompleted += OnFinalVideoPrepared; 
            _videoPlayer.Prepare(); 
        }
     
    }
    
    private void OnFinalVideoPrepared(VideoPlayer vp)
    { 
        _videoPlayer.prepareCompleted -= OnFinalVideoPrepared;
        StartFinalVideoPlayback();
    }
    
    private void StartFinalVideoPlayback()
    {
        _videoPlayer.loopPointReached += OnFinalVideoFinished;
        SetVideoPlayerUrl(_finalVideoClipUrl);
        FinalVideoStarted?.Invoke();
        _videoPlayer.Play();
        StartCoroutine(UpdateRawImageAfterDelay(RawImageAfterDelay));
    }

    private void SetVideoPlayerUrl(string url) => _videoPlayer.url = url;

    private void OnFinalVideoFinished(VideoPlayer source)
    {
        _videoPlayer.loopPointReached -= OnFinalVideoFinished;
        FinalVideoFinished?.Invoke();
    }

    private void OnTapped()
    {
        if (_placeholderTexture != null)
        {
            if (_videoPlayer.isPlaying)
            {
                if (_videoPlayer.playbackSpeed < MaxPlayerPlaybackSpeed)
                {
                    SetPlaybackSpeed(MaxPlayerPlaybackSpeed);
                    _videoPlayer.loopPointReached += OnTappedVideoPointReached;  
                }
            }
            else
            {
                OnTappedVideoPointReached(_videoPlayer);
            }
        }
    }

    private void OnTappedVideoPointReached(VideoPlayer videoPlayer)
    {
        SetPlaybackSpeed(MinPlayerPlaybackSpeed);
        _videoPlayer.loopPointReached -= OnTappedVideoPointReached;
        SetRawImageTexture(_placeholderTexture);
        
        if (_videoPlayer.isPrepared)
        {
            OnVideoPrepared(_videoPlayer);
        }
        else
        {
            _videoPlayer.prepareCompleted += OnVideoPrepared;
            _videoPlayer.Prepare();
        }
    }
    
    private void SetPlaybackSpeed(float value) => _videoPlayer.playbackSpeed = value;
    private void SetRawImageTexture(Texture value) => _rawImage.texture = value;

    private void OnDestroy()
    {
        if (_placeholderTexture != null) 
            Destroy(_placeholderTexture);

        if (_renderTexture != null)
        {
            _renderTexture.Release();
            Destroy(_renderTexture);
        }
        
        SetRawImageTexture(null);
    }
}