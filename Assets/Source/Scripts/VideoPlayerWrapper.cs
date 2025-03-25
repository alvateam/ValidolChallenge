using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerWrapper : MonoBehaviour
{
    private const float RawImageAfterDelay = 0.1f;
    
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RawImage _rawImage;

    [SerializeField] private InputActionsHandler _inputActionsHandler;
    [SerializeField] private FilledProgressHandler _filledProgressHandler;

    private VideoClip _loopedVideoClip;
    private VideoClip _finalVideoClip;
    private Texture _placeholderTexture;
    private RenderTexture _renderTexture;

    public void Initialize(VideoClip loopedVideo, VideoClip finalVideo)
    {
        _loopedVideoClip = loopedVideo;
        _finalVideoClip = finalVideo;
        
        _videoPlayer.clip = _loopedVideoClip;
        _videoPlayer.sendFrameReadyEvents = true;
        
        _filledProgressHandler.Filled += OnFilled;
        _inputActionsHandler.Clicked += OnClicked;
        _inputActionsHandler.RestartButtonClicked += OnRestartButtonClicked;
        
        InitializeRenderTexture();
        
        _videoPlayer.targetTexture = _renderTexture;
        _rawImage.texture = _renderTexture;
        
        _videoPlayer.frameReady += OnFrameReady;
        _videoPlayer.prepareCompleted += OnPrepareCompleted;
        _videoPlayer.Prepare();
    }
    
    private void InitializeRenderTexture()
    {
        int width = (int)_videoPlayer.clip.width;
        int height = (int)_videoPlayer.clip.height;
        
        _renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32)
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
        if (frameIndex == 0 && _placeholderTexture == null)
        {
            videoPlayer.frameReady -= OnFrameReady;
            _placeholderTexture = CreateTextureFromFrame(videoPlayer.targetTexture);
            _rawImage.texture = _placeholderTexture;
            _videoPlayer.Stop();
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
        _rawImage.texture = _placeholderTexture;
        _videoPlayer.Stop();
        _videoPlayer.time = 0;
        _videoPlayer.clip = _loopedVideoClip;
        _videoPlayer.Prepare();
        _videoPlayer.prepareCompleted += OnVideoPrepared;
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
        _inputActionsHandler.Clicked += OnClicked;

        _videoPlayer.frameReady -= OnFrameReady;
        _videoPlayer.loopPointReached -= OnLoopVideoFinished;
        _videoPlayer.prepareCompleted -= OnLoopVideoFinished;
        _videoPlayer.prepareCompleted -= OnPrepareCompleted;
        _videoPlayer.prepareCompleted -= OnFinalVideoPrepared;
        
        RestartVideo();
    }

    private void OnFilled()
    {
        _inputActionsHandler.Clicked -= OnClicked;
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
        if (_finalVideoClip == null)
        {
            Debug.LogError("Final video clip is not assigned!");
            return;
        }
        
        _videoPlayer.prepareCompleted += OnFinalVideoPrepared; 
        _videoPlayer.Prepare();
    }
    
    private void OnFinalVideoPrepared(VideoPlayer vp)
    { 
        _videoPlayer.prepareCompleted -= OnFinalVideoPrepared;
        StartFinalVideoPlayback();
    }
    
    private void StartFinalVideoPlayback()
    {
        PlayVideo(_finalVideoClip);
        StartCoroutine(UpdateRawImageAfterDelay(RawImageAfterDelay));
    }
    
    private void OnClicked()
    {
        if (_placeholderTexture != null) 
            RestartVideo();
    }

    private void PlayVideo(VideoClip videoClip)
    {
        _videoPlayer.clip = videoClip;
        _videoPlayer.Play();
    }
    
    private void OnDestroy()
    {
        if (_placeholderTexture != null) 
            Destroy(_placeholderTexture);
        
        if (_renderTexture != null) 
            _renderTexture.Release();
    }
}