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
    
    /*public async void CreateVideoContainers(List<VideoJsonData> value)
    {
        int count = value.Count;
        _videoBootstrappers ??= new List<VideoBootstrapper>(count);
        
        for (var index = 0; index < count; index++)
        {
            VideoJsonData data = value[index];
            await CreateVideoContainer(data, index + 1);
        }

        await UniTask.WaitForEndOfFrame();
        
        VideoContainersCreated?.Invoke(_videoBootstrappers);
    }*/
    
    public async void CreateVideoContainers(List<VideoJsonData> value, bool insertAtStart = false)
    {
        int count = value.Count;
        _videoBootstrappers ??= new List<VideoBootstrapper>(count);

        // Определяем начальный индекс для вставки
        int startIndex = insertAtStart ? 0 : _videoBootstrappers.Count;

        for (var index = 0; index < count; index++)
        {
            VideoJsonData data = value[index];
            await CreateVideoContainer(data, insertAtStart);
        }

        await UniTask.WaitForEndOfFrame();

        VideoContainersCreated?.Invoke(_videoBootstrappers);
    }
    
    private async Task CreateVideoContainer(VideoJsonData value, bool insertAtStart)
    {
        int videoId = value.Id;
        List<Task<DownloadedVideo>> tasks = new List<Task<DownloadedVideo>>
        {
            _videoDownloader.DownloadVideoAsync(new DownloadedVideo(videoId, value.LoopedVideoType, value.LoopedVideoId)),
            _videoDownloader.DownloadVideoAsync(new DownloadedVideo(videoId, value.FinalVideoType, value.FinalVideoId))
        };

        DownloadedVideo[] result = await Task.WhenAll(tasks);

        VideoBootstrapper bootstrapper = Instantiate(_videoBootstrapper);
        bootstrapper.gameObject.name += "_" + videoId;
        bootstrapper.Initialize(value, result[0].Url, result[1].Url, videoId);

        // Определяем siblingIndex для нового элемента
        int siblingIndex = insertAtStart ? 0 : _videoBootstrappers.Count;

        // Добавляем элемент в _pageSlider с указанием siblingIndex
        _pageSlider.AddPage(bootstrapper.RectTransform, siblingIndex);

        // Вставляем элемент в правильную позицию в коллекции
        if (insertAtStart)
        {
            _videoBootstrappers.Insert(0, bootstrapper); // Вставка в начало
        }
        else
        {
            _videoBootstrappers.Add(bootstrapper); // Добавление в конец
        }
    }
    
    /*private async Task CreateVideoContainer(VideoJsonData value, int number)
    {
        List<Task<DownloadedVideo>> tasks = new List<Task<DownloadedVideo>>
        {
            _videoDownloader.DownloadVideoAsync(new DownloadedVideo(value.Id, value.LoopedVideoType, value.LoopedVideoId)),
            _videoDownloader.DownloadVideoAsync(new DownloadedVideo(value.Id, value.FinalVideoType, value.FinalVideoId))
        };
            
        DownloadedVideo[] result = await Task.WhenAll(tasks);
            
        VideoBootstrapper bootstrapper = Instantiate(_videoBootstrapper);
        bootstrapper.gameObject.name += number.ToString();
        bootstrapper.Initialize(value, result[0].Url, result[1].Url,number);
        _pageSlider.AddPage(bootstrapper.RectTransform);
        _videoBootstrappers.Add(bootstrapper);
    }*/
}
