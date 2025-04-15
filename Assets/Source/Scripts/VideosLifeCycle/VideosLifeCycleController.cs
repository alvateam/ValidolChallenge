using System;
using System.Collections.Generic;
using System.Linq;
using Source.Scripts.Downloading;
using Source.Scripts.Save;
using TS.PageSlider;
using UnityEngine;

namespace Source.Scripts.VideosLifeCycle
{
    public class VideosLifeCycleController : MonoBehaviour
    {
        [SerializeField] private JsonDownloader _jsonDownloader;
        [SerializeField] private VideosFactory _videosFactory;
        [SerializeField] private PageScroller _pageScroller;
        [SerializeField] private SaveService _saveService;

        private int _currentVideoId;
        private int _currentVideoIndex = -1;

        private List<VideoJsonData> _videoJsonDatas;
        private IReadOnlyList<VideoBootstrapper> _videoBootstrappers;
        private List<VideoJsonData> _downloadedVideos;
        private PreloadVideosHandler _preloadVideosHandler;

        private SaveData SaveData => _saveService.SaveData;

        public event Action<int> CurrentVideoIndexChanged;

        private void OnEnable()
        {
            _jsonDownloader.Downloaded += OnJsonDownloaded;
            _pageScroller.OnPageChangeEnded.AddListener(OnPageChangeEnded);
            _videosFactory.VideoContainersCreated += OnVideosContainersCreated;
        }

        private void OnVideosContainersCreated(IReadOnlyList<VideoBootstrapper> videoBootstrappers)
        {
            _videoBootstrappers = videoBootstrappers;

            if (_currentVideoIndex > 0)
            {
                if (_videoBootstrappers[_currentVideoIndex].Data.Id == _currentVideoId)
                {
                    _videoBootstrappers[_currentVideoIndex].VideoPlayerWrapper.RestartPlayer();
                    return;
                }
            }

            int index = _videoBootstrappers
                .Select((x, idx) => new { x.Data.Id, Index = idx }) // Создаем проекцию с индексами
                .FirstOrDefault(x => x.Id == _currentVideoId)?.Index ?? -1;

            SetCurrentVideoIndex(index);
            _videoBootstrappers[_currentVideoIndex].VideoPlayerWrapper.RestartPlayer();
            CurrentVideoIndexChanged?.Invoke(_currentVideoIndex);
        }

        private void OnJsonDownloaded(VideoJsonWrapper value)
        {
            _videoJsonDatas = value.videos;
            SetCurrentVideoId(SaveData.VideoId);

            _preloadVideosHandler = new PreloadVideosHandler(_videoJsonDatas);
            List<VideoJsonData> downloadedVideos = _preloadVideosHandler.GetStartLoadingVideos(_currentVideoId);
            if (!downloadedVideos.Any())
                throw new ArgumentException("Started videos does not exist");

            _downloadedVideos = downloadedVideos;
            _videosFactory.CreateVideoContainers(_downloadedVideos);
        }

        private void OnPageChangeEnded(int previous, int current)
        {
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

                List<VideoJsonData> videosToDownload =
                    _preloadVideosHandler.GetNextVideos(_currentVideoId, _downloadedVideos);
                
                UpdateSavedData();
                TryCreateVideoContainters(videosToDownload, false);
            }
            else if (current < previous)
            {
                // Листаем назад: проверяем и скачиваем предыдущие видео
                int currentVideoId = Mathf.Max(_currentVideoId - 1, 1);
                SetCurrentVideoId(currentVideoId);
                
                List<VideoJsonData> videosToDownload =
                    _preloadVideosHandler.GetPreviousVideos(_currentVideoId, _downloadedVideos);
                
                UpdateSavedData();
                TryCreateVideoContainters(videosToDownload, true);
            }
        }

        private void UpdateSavedData()
        {
            SaveData.VideoId = _currentVideoId;
            _saveService.Save();
        }

        private void SetCurrentVideoIndex(int current) => _currentVideoIndex = current;

        private void SetCurrentVideoId(int value) => _currentVideoId = value;

        private void TryCreateVideoContainters(List<VideoJsonData> videosToDownload, bool insertAtStart)
        {
            if (videosToDownload.Any())
            {
                AddDownloadedVideos(videosToDownload);
                _videosFactory.CreateVideoContainers(videosToDownload, insertAtStart);
            }
        }

        private void AddDownloadedVideos(List<VideoJsonData> newVideos)
        {
            foreach (var video in newVideos)
            {
                _downloadedVideos.Add(video);
            }
        }
    }
}