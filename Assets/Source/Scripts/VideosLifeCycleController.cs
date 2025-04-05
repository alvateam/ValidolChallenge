using System;
using System.Collections.Generic;
using System.Linq;
using TS.PageSlider;
using UnityEngine;

public class VideosLifeCycleController : MonoBehaviour
{
    private const int DownloadedVideosCount = 3;

    [SerializeField] private JsonDownloader _jsonDownloader;
    [SerializeField] private VideosFactory _videosFactory;
    [SerializeField] private PageScroller _pageScroller;

    private int _currentVideoId;
    private int _currentVideoIndex;

    private List<VideoJsonData> _videoJsonDatas;
    private IReadOnlyList<VideoBootstrapper> _videoBootstrappers;
    
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
        int index = _videoBootstrappers
            .Select((x, idx) => new { x.Data.Id, Index = idx }) // Создаем проекцию с индексами
            .FirstOrDefault(x => x.Id == _currentVideoId)?.Index ?? -1;
        
        SetCurrentVideoIndex(index);
    }

    private void OnJsonDownloaded(VideoJsonWrapper value)
    {
        _videoJsonDatas = value.videos;
        _currentVideoId = 4;

        var startLoadingVideoId = 0;
        var endLoadingVideoId = _currentVideoId;

        if (_currentVideoId > 1)
        {
            startLoadingVideoId = _currentVideoId - DownloadedVideosCount;
            startLoadingVideoId = Mathf.Clamp(startLoadingVideoId, 1, _videoJsonDatas.Count);
        }

        if (endLoadingVideoId < _videoJsonDatas.Count)
        {
            endLoadingVideoId += DownloadedVideosCount;
            endLoadingVideoId = Mathf.Clamp(endLoadingVideoId, endLoadingVideoId, _videoJsonDatas.Count);
        }
        
        var downloadedVideos = _videoJsonDatas
            .SkipWhile(x => x.Id != startLoadingVideoId)
            .TakeWhile(x => x.Id <= endLoadingVideoId)
            .ToList();

        _videosFactory.CreateVideoContainers(downloadedVideos);
    }

    private void OnPageChangeEnded(int previous, int current)
    {
        if (_currentVideoIndex == current)
            return;

        SetCurrentVideoIndex(current);
        
        if (current > previous)
        {
            _currentVideoId++;

            var checkableIndex = _currentVideoIndex + DownloadedVideosCount - 1;
            var lastDownloadedIndex = _videoBootstrappers.Count - 1;


            // if(_currentVideoIndex == )

            if (lastDownloadedIndex < _videoJsonDatas.Count - 1)
            {
                var downloadVideosCount = checkableIndex - lastDownloadedIndex;
                var downloadedVideos = _videoJsonDatas
                    .Skip(_videoBootstrappers.Count)
                    .Take(downloadVideosCount)
                    .ToList();

                _videosFactory.CreateVideoContainers(downloadedVideos);
            }
        }
        else if (current < previous)
        {
            _currentVideoId--;
            if (current > DownloadedVideosCount - 1)
            {
                var checkableIndex = current - DownloadedVideosCount;
                /*var lastDownloadedIndex = _videoBootstrappers.Count - 1;
                if (lastDownloadedIndex < _videoJsonDatas.Count - 1)
                {
                    var downloadVideosCount = checkableIndex - lastDownloadedIndex;
                    var downloadedVideos = _videoJsonDatas
                        .Skip(_videoBootstrappers.Count)
                        .Take(downloadVideosCount)
                        .ToList();

                    _videosFactory.CreateVideoContainers(downloadedVideos);
                }*/
            }
        }
    }

    private void SetCurrentVideoIndex(int current)
    {
        _currentVideoIndex = current;
        CurrentVideoIndexChanged?.Invoke(_currentVideoIndex);
    }
}