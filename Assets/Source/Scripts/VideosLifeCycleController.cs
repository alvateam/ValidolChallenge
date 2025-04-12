using System;
using System.Collections.Generic;
using System.Linq;
using TS.PageSlider;
using UnityEngine;

public class VideosLifeCycleController : MonoBehaviour
{
    private const int DownloadedVideosCount = 2;

    [SerializeField] private JsonDownloader _jsonDownloader;
    [SerializeField] private VideosFactory _videosFactory;
    [SerializeField] private PageScroller _pageScroller;
    [SerializeField] private SaveService _saveService;

    private int _currentVideoId;
    private int _currentVideoIndex = -1;

    private List<VideoJsonData> _videoJsonDatas;
    private IReadOnlyList<VideoBootstrapper> _videoBootstrappers;
    private HashSet<int> _downloadedVideosIds;

    private SaveData SaveData => _saveService.SaveData;

    public event Action<int> CurrentVideoIndexChanged;

    private void OnEnable()
    {
        _jsonDownloader.Downloaded += OnJsonDownloaded;
        _pageScroller.OnPageChangeEnded.AddListener(OnPageChangeEnded);
        _videosFactory.VideoContainersCreated += OnStartVideosContainersCreated;
    }

    private void OnStartVideosContainersCreated(IReadOnlyList<VideoBootstrapper> videoBootstrappers)
    {
        _videoBootstrappers = videoBootstrappers;
        int index = _videoBootstrappers
            .Select((x, idx) => new { x.Data.Id, Index = idx }) // Создаем проекцию с индексами
            .FirstOrDefault(x => x.Id == _currentVideoId)?.Index ?? -1;

        SetCurrentVideoIndex(index);
        _videoBootstrappers[_currentVideoIndex].VideoPlayerWrapper.RestartPlayer();
    }
    
    private void OnJsonDownloaded(VideoJsonWrapper value)
    {
        _videoJsonDatas = value.videos;
        SetCurrentVideoId(SaveData.VideoId);

        // Получаем начальный и конечный ID для загрузки
        int startLoadingVideoId = GetStartLoadingVideoId(_currentVideoId);
        int endLoadingVideoId = GetEndLoadingVideoId(_currentVideoId);

        // Фильтруем видео в заданном диапазоне
        var downloadedVideos = _videoJsonDatas
            .Where(x => x.Id >= startLoadingVideoId && x.Id <= endLoadingVideoId)
            .ToList();

        // Сохраняем ID скачанных видео
        _downloadedVideosIds = new HashSet<int>(downloadedVideos.Select(x => x.Id));

        // Создаем контейнеры для скачанных видео
        _videosFactory.CreateVideoContainers(downloadedVideos);
    }
    
    // Метод для получения начального ID (текущий - DownloadedVideosCount)
    private int GetStartLoadingVideoId(int currentId)
    {
        // Ограничиваем минимальное значение до 1, так как ID начинаются с 1
        return Mathf.Max(1, currentId - DownloadedVideosCount);
    }

// Метод для получения конечного ID (текущий + DownloadedVideosCount)
    private int GetEndLoadingVideoId(int currentId)
    {
        // Ограничиваем максимальное значение количеством видео в коллекции
        return Mathf.Min(currentId + DownloadedVideosCount, _videoJsonDatas.Count);
    }
    
    private void OnPageChangeEnded(int previous, int current)
    {
        // Если текущий индекс совпадает с новым, ничего не делаем
        if (_currentVideoIndex == current)
            return;
        
        _videoBootstrappers[_currentVideoIndex].VideoPlayerWrapper.StopPlayer();
        
        SetCurrentVideoIndex(current);
        
        _videoBootstrappers[_currentVideoIndex].VideoPlayerWrapper.StartPlayer();

        if (current > previous)
        {
            // Листаем вперёд: проверяем и скачиваем следующие видео
            int currentVideoId = Mathf.Min(_currentVideoId + 1, _videoJsonDatas.Count);
            SetCurrentVideoId(currentVideoId);
            UpdateSavedData();
            HandleNextVideos();
        }
        else if (current < previous)
        {
            // Листаем назад: проверяем и скачиваем предыдущие видео
            var currentVideoId = Mathf.Max(_currentVideoId - 1, 1);
            SetCurrentVideoId(currentVideoId);
            UpdateSavedData();
            HandlePreviousVideos();
        }
    }

    private void UpdateSavedData()
    {
        SaveData.VideoId = _currentVideoId;
        _saveService.Save();
    }

    private void HandleNextVideos()
    {
        // Получаем диапазон id для проверки (текущий + следующие два)
        var videoIdsToCheck = GetNextVideoIds();

        // Фильтруем id, которые ещё не скачаны
        var videoIdsToDownload = videoIdsToCheck
            .Where(id => !_downloadedVideosIds.Contains(id))
            .ToList();

        DownloadVideosIfNeeded(videoIdsToDownload, false);
    }
    
    private void HandlePreviousVideos()
    {
        // Получаем диапазон id для проверки (текущий - предыдущие два)
        var videoIdsToCheck = GetPreviousVideoIds();

        // Фильтруем id, которые ещё не скачаны
        var videoIdsToDownload = videoIdsToCheck
            .Where(id => !_downloadedVideosIds.Contains(id))
            .ToList();

        DownloadVideosIfNeeded(videoIdsToDownload, true);
    }
    
    private void DownloadVideosIfNeeded(List<int> videoIdsToDownload, bool insertAtStart)
    {
        // Если нет видео для скачивания, выходим
        if (!videoIdsToDownload.Any())
            return;

        // Находим соответствующие данные видео
        var videosToDownload = _videoJsonDatas
            .Where(video => videoIdsToDownload.Contains(video.Id))
            .ToList();

        // Если есть видео для создания контейнеров, создаём их
        if (videosToDownload.Any())
        {
            _videosFactory.CreateVideoContainers(videosToDownload, insertAtStart);
            AddDownloadedVideoIds(videoIdsToDownload);
        }
    }
    
    private void AddDownloadedVideoIds(List<int> newVideoIds)
    {
        foreach (var id in newVideoIds)
        {
            _downloadedVideosIds.Add(id);
        }
    }
    
    private void SetCurrentVideoIndex(int current)
    {
        Debug.Log($"Current index: {current}");
        _currentVideoIndex = current;
        CurrentVideoIndexChanged?.Invoke(_currentVideoIndex);
    }
    
    private List<int> GetNextVideoIds()
    {
        var nextIds = new List<int>();
        for (int i = _currentVideoId + 1; i <= _currentVideoId + DownloadedVideosCount && i <= _videoJsonDatas.Count; i++)
        {
            nextIds.Add(i);
        }
        
        return nextIds;
    }
    
    private List<int> GetPreviousVideoIds()
    {
        var previousIds = new List<int>();
        for (int i = _currentVideoId - 1; i >= _currentVideoId - DownloadedVideosCount && i >= 0; i--)
        {
            previousIds.Add(i);
        }
        
        return previousIds;
    }

    private void SetCurrentVideoId(int value) => _currentVideoId = value;
}