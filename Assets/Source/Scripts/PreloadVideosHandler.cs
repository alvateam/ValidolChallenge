using System.Collections.Generic;
using System.Linq;
using Source.Scripts.Downloading;
using UnityEngine;

public class PreloadVideosHandler
{
    private const int DownloadedVideosCount = 2;
    
    private List<VideoJsonData> _videoJsonDatas;
    private HashSet<int> _downloadedVideosIds;
    
    public PreloadVideosHandler()
    {
        
    }

    public List<VideoJsonData> GetStartLoadingVideos(int currentVideoId)
    {
        // Получаем начальный и конечный ID для загрузки
        int startLoadingVideoId = GetStartLoadingVideoId(currentVideoId);
        int endLoadingVideoId = GetEndLoadingVideoId(currentVideoId);

        // Фильтруем видео в заданном диапазоне
        return _videoJsonDatas
            .Where(x => x.Id >= startLoadingVideoId && x.Id <= endLoadingVideoId)
            .ToList();
    }
    
    public List<VideoJsonData> GetNextVideos(int currentVideoId)
    {
        // Получаем диапазон id для проверки (текущий + следующие два)
        var videoIdsToCheck = GetNextVideoIds(currentVideoId);

        // Фильтруем id, которые ещё не скачаны
        var videoIdsToDownload = videoIdsToCheck
            .Where(id => !_downloadedVideosIds.Contains(id))
            .ToList();

        return GetDownloadVideos(videoIdsToDownload, false);
    }
    
    public List<VideoJsonData> GetPreviousVideos(int currentVideoId)
    {
        // Получаем диапазон id для проверки (текущий - предыдущие два)
        var videoIdsToCheck = GetPreviousVideoIds(currentVideoId);

        // Фильтруем id, которые ещё не скачаны
        var videoIdsToDownload = videoIdsToCheck
            .Where(id => !_downloadedVideosIds.Contains(id))
            .ToList();

        return GetDownloadVideos(videoIdsToDownload, true);
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
    
    private List<VideoJsonData> GetDownloadVideos(List<int> videoIdsToDownload, bool insertAtStart)
    {
        // Если нет видео для скачивания, выходим
        if (!videoIdsToDownload.Any())
            return new List<VideoJsonData>();

        // Находим соответствующие данные видео
        var videosToDownload = _videoJsonDatas
            .Where(video => videoIdsToDownload.Contains(video.Id))
            .ToList();

        // Если есть видео для создания контейнеров, создаём их
        if (videosToDownload.Any())
        {
            /*_videosFactory.CreateVideoContainers(videosToDownload, insertAtStart);
            AddDownloadedVideoIds(videoIdsToDownload);*/
            return videosToDownload;
        }
        
        return new List<VideoJsonData>();
    }
    
    private List<int> GetNextVideoIds(int currentVideoId)
    {
        var nextIds = new List<int>();
        for (int i = currentVideoId + 1; i <= currentVideoId + DownloadedVideosCount && i <= _videoJsonDatas.Count; i++)
        {
            nextIds.Add(i);
        }
        
        return nextIds;
    }
    
    private List<int> GetPreviousVideoIds(int currentVideoId)
    {
        var previousIds = new List<int>();
        for (int i = currentVideoId - 1; i >= currentVideoId - DownloadedVideosCount && i >= 0; i--)
        {
            previousIds.Add(i);
        }
        
        return previousIds;
    }
}