using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TS.PageSlider;
using UnityEngine;

public class VideosFactory : MonoBehaviour
{
    private const int DownloadedVideosCount = 3;
    
    [SerializeField] private VideoBootstrapper _videoBootstrapper;
    [SerializeField] private Transform _container;
    [SerializeField] private PageSlider _pageSlider;
    [SerializeField] private PageScroller _pageScroller;
    [SerializeField] private JsonDownloader _jsonDownloader;
    [SerializeField] private VideoDownloader _videoDownloader;

    private List<VideoBootstrapper> _videoBootstrappers;

    public event Action<IReadOnlyList<VideoBootstrapper>> VideoContainersCreated;

    private void OnEnable()
    {
        _jsonDownloader.Downloaded += OnJsonDownloaded;
        _pageScroller.OnPageChangeEnded.AddListener(OnPageChangeEnded);
    }

    private void OnPageChangeEnded(int arg0, int arg1)
    {
        var s = 0;
    }


    private void OnDisable()
    {
        _jsonDownloader.Downloaded -= OnJsonDownloaded;
        _pageScroller.OnPageChangeEnded.RemoveListener(OnPageChangeEnded);
    }

    private void OnJsonDownloaded(VideoJsonWrapper value)
    {
        CreateVideoContainers(value.videos);
    }

    private async void CreateVideoContainers(List<VideoJsonData> value)
    {
        _videoBootstrappers = new List<VideoBootstrapper>(DownloadedVideosCount);
        
        for (var index = 0; index < DownloadedVideosCount; index++)
        {
            VideoJsonData data = value[index];
            List<Task<string>> tasks = new List<Task<string>>()
            {
                _videoDownloader.DownloadVideoAsync(data.LoopedVideoUrl, data.LoopedVideoType, data.Id),
                _videoDownloader.DownloadVideoAsync(data.FinalVideoUrl, data.FinalVideoType, data.Id)
            };
            
            string[] result = await Task.WhenAll(tasks);
            
            VideoBootstrapper bootstrapper = Instantiate(_videoBootstrapper);
            bootstrapper.Initialize(data, result[0], result[1],index + 1);
            _pageSlider.AddPage(bootstrapper.RectTransform);
            _videoBootstrappers.Add(bootstrapper);
        }
        
        VideoContainersCreated?.Invoke(_videoBootstrappers);
    }
}
