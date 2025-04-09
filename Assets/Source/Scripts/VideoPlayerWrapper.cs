using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerWrapper : MonoBehaviour
{
    private const float MaxPlayerPlaybackSpeed = 10;
    private const float RawImageAfterDelay = 0.1f;
    
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RawImage _rawImage;

    [SerializeField] private VideoUIPresenter _videoUIPresenter;
    [SerializeField] private FilledProgressHandler _filledProgressHandler;

    private string _loopedVideoClipUrl;
    private string _finalVideoClipUrl;
    
    private Texture _placeholderTexture;
    private RenderTexture _renderTexture;

    public event Action FinalVideoStarted;
    public event Action FinalVideoFinished; 

    public void Initialize(string loopedVideo, string finalVideo)
    {
        _loopedVideoClipUrl = loopedVideo;
        _finalVideoClipUrl = finalVideo;
        
        _videoPlayer.url = _finalVideoClipUrl;
        _videoPlayer.sendFrameReadyEvents = true;
        
        _filledProgressHandler.Filled += OnFilled;
        _videoUIPresenter.Tapped += OnTapped;
        _videoUIPresenter.RestartButtonClicked += OnRestartButtonClicked;
        
        InitializeRenderTexture();
        
        _videoPlayer.targetTexture = _renderTexture;
        _rawImage.texture = _renderTexture;
        
        _videoPlayer.frameReady += OnFrameReady;
        _videoPlayer.prepareCompleted += OnPrepareCompleted;
        _videoPlayer.Prepare();
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
            _rawImage.texture = _placeholderTexture;
            _videoPlayer.Stop();
            _videoPlayer.url = _loopedVideoClipUrl;
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
        _rawImage.texture = _videoPlayer.targetTexture;
    }

    private void RestartVideo()
    {
        if (_videoPlayer.isPlaying)
        {
            if (_videoPlayer.playbackSpeed < MaxPlayerPlaybackSpeed)
            {
                _videoPlayer.playbackSpeed = MaxPlayerPlaybackSpeed;
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
        _videoPlayer.playbackSpeed = 1;
        _videoPlayer.loopPointReached -= OnRestartVideoPointReached;
        _rawImage.texture = _placeholderTexture;
        _videoPlayer.url = _loopedVideoClipUrl;
        _videoPlayer.Prepare();
        _videoPlayer.prepareCompleted += OnVideoPrepared;
        /*_videoPlayer.Stop();
        _videoPlayer.time = 0;*/
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
        _videoPlayer.url = _finalVideoClipUrl;
        FinalVideoStarted?.Invoke();
        _videoPlayer.Play();
        StartCoroutine(UpdateRawImageAfterDelay(RawImageAfterDelay));
    }

    private void OnFinalVideoFinished(VideoPlayer source)
    {
        _videoPlayer.loopPointReached -= OnFinalVideoFinished;
        FinalVideoFinished?.Invoke();
    }

    private void OnTapped()
    {
        if (_placeholderTexture != null) 
            RestartVideo();
    }
    
    private void OnDestroy()
    {
        if (_placeholderTexture != null) 
            Destroy(_placeholderTexture);
        
        if (_renderTexture != null) 
            _renderTexture.Release();
    }
}