using System;
using System.Collections.Generic;
using System.Linq;
using Source.Scripts.Downloading;
using UnityEngine;

namespace Source.Scripts.VideosLifeCycle
{
    public class PreloadVideosHandler
    {
        private const int DownloadedVideosCount = 2;
    
        private IReadOnlyList<VideoJsonData> _videosJsonData;

        public PreloadVideosHandler(IReadOnlyList<VideoJsonData> videosJsonData)
        {
            _videosJsonData = videosJsonData;
        }

        public List<VideoJsonData> GetStartLoadingVideos(int currentVideoId)
        {
            // Получаем начальный и конечный ID для загрузки
            int startLoadingVideoId = GetStartLoadingVideoId(currentVideoId);
            int endLoadingVideoId = GetEndLoadingVideoId(currentVideoId);

            // Фильтруем видео в заданном диапазоне
            return _videosJsonData
                .Where(x => x.Id >= startLoadingVideoId && x.Id <= endLoadingVideoId)
                .ToList();
        }
    
        public List<VideoJsonData> GetNextVideos(int currentVideoId, List<VideoJsonData> downloadedVideos)
        {
            List<VideoJsonData> videos = GetNextVideos(currentVideoId);
            return GetVideos(downloadedVideos, videos);
        }
    
        public List<VideoJsonData> GetPreviousVideos(int currentVideoId, List<VideoJsonData> downloadedVideos)
        {
            List<VideoJsonData> videos = GetPreviousVideoIds(currentVideoId);
            return GetVideos(downloadedVideos, videos);
        }

        private List<VideoJsonData> GetVideos(List<VideoJsonData> downloadedVideos, List<VideoJsonData> previousVideos)
        {
            // Фильтруем id, которые ещё не скачаны
            List<VideoJsonData> videosToDownload = previousVideos
                .Where(id => !downloadedVideos.Contains(id))
                .ToList();

            return GetDownloadVideos(videosToDownload);
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
            return Mathf.Min(currentId + DownloadedVideosCount, _videosJsonData.Count);
        }
    
        private List<VideoJsonData> GetDownloadVideos(List<VideoJsonData> videoIdsToDownload)
        {
            if (!videoIdsToDownload.Any())
                return new List<VideoJsonData>();

            List<int> videoIds = videoIdsToDownload.Select(video => video.Id).ToList();
        
            // Находим соответствующие данные видео
            List<VideoJsonData> videosToDownload = _videosJsonData
                .Where(video => videoIds.Contains(video.Id))
                .ToList();
        
            return videosToDownload.Any() ? videosToDownload : new List<VideoJsonData>();
        }
    
        private List<VideoJsonData> GetNextVideos(int currentVideoId)
        {
            var nextIds = new List<VideoJsonData>();
            int startIndex = currentVideoId; // Индекс следующего видео
            int endIndex = Math.Min(startIndex + DownloadedVideosCount, _videosJsonData.Count);

            // Добавляем элементы в список
            for (int i = startIndex; i < endIndex; i++)
            {
                nextIds.Add(_videosJsonData[i]);
            }

            return nextIds;
        }
    
        private List<VideoJsonData> GetPreviousVideoIds(int currentVideoId)
        {
            var previousIds = new List<VideoJsonData>();
       
            int startIndex = Math.Max(currentVideoId - DownloadedVideosCount, 1); // Начинаем с предыдущего видео
            int endIndex = currentVideoId; // Конечный индекс (не включая текущее видео)

            // Добавляем элементы в список
            for (int i = startIndex; i < endIndex; i++)
            {
                previousIds.Add(_videosJsonData[i - 1]); // videoId начинается с 1, а индексация с 0
            }

            return previousIds;
        }
    }
}