using System;
using System.Collections.Generic;
using Data;
using TS.PageSlider;
using UnityEngine;

public class VideosFactory : MonoBehaviour
{
    [SerializeField] private VideoBootstrapper _videoBootstrapper;
    [SerializeField] private Transform _container;
    [SerializeField] private List<VideoData> _videoDatas;
    [SerializeField] private PageSlider _pageSlider;

    private List<VideoBootstrapper> _videoBootstrappers;

    public event Action<IReadOnlyList<VideoBootstrapper>> VideoContainersCreated;

    private void Awake()
    {
        _videoBootstrappers = new List<VideoBootstrapper>(_videoDatas.Count);
        
        for (var index = 0; index < _videoDatas.Count; index++)
        {
            var data = _videoDatas[index];
            VideoBootstrapper bootstrapper = Instantiate(_videoBootstrapper);
            bootstrapper.Initialize(data, index + 1);
            _pageSlider.AddPage(bootstrapper.RectTransform);
            _videoBootstrappers.Add(bootstrapper);
        }
        
        VideoContainersCreated?.Invoke(_videoBootstrappers);
    }
}
