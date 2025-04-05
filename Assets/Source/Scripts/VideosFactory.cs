using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TS.PageSlider;
using UnityEngine;

public class VideosFactory : MonoBehaviour
{
    [SerializeField] private VideoBootstrapper _videoBootstrapper;
    [SerializeField] private PageSlider _pageSlider;
    [SerializeField] private VideoDownloader _videoDownloader;

    private List<VideoBootstrapper> _videoBootstrappers;

    public event Action<IReadOnlyList<VideoBootstrapper>> VideoContainersCreated;
    public event Action<VideoBootstrapper> VideoContainerCreated;
    public event Action<IReadOnlyCollection<DownloadedVideo>> VideosPreloaded;
    
    public async void CreateVideoContainers(List<VideoJsonData> value)
    {
        int count = value.Count;
        _videoBootstrappers ??= new List<VideoBootstrapper>(count);
        
        for (var index = 0; index < count; index++)
        {
            VideoJsonData data = value[index];
            await CreateVideoContainer(data, index + 1);
        }

        await UniTask.WaitForEndOfFrame();
        
        Debug.Log("Videos downloaded");
        VideoContainersCreated?.Invoke(_videoBootstrappers);
    }
    
    private async Task CreateVideoContainer(VideoJsonData value, int number)
    {
        List<Task<DownloadedVideo>> tasks = new List<Task<DownloadedVideo>>
        {
            _videoDownloader.DownloadVideoAsync(new DownloadedVideo(value.Id, value.LoopedVideoType, value.LoopedVideoId)),
            _videoDownloader.DownloadVideoAsync(new DownloadedVideo(value.Id, value.FinalVideoType, value.FinalVideoId))
        };
            
        DownloadedVideo[] result = await Task.WhenAll(tasks);
        VideosPreloaded?.Invoke(result);
            
        VideoBootstrapper bootstrapper = Instantiate(_videoBootstrapper);
        bootstrapper.gameObject.name += number.ToString();
        bootstrapper.Initialize(value, result[0].Url, result[1].Url,number);
        _pageSlider.AddPage(bootstrapper.RectTransform);
        _videoBootstrappers.Add(bootstrapper);
        
        VideoContainerCreated?.Invoke(bootstrapper);
    }
}
