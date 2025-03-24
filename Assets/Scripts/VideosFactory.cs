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

    private void Awake()
    {
        for (var index = 0; index < _videoDatas.Count; index++)
        {
            var data = _videoDatas[index];
            var bootstrapper = Instantiate(_videoBootstrapper);
            bootstrapper.Initialize(data, index + 1);
            _pageSlider.AddPage(bootstrapper.RectTransform);
        }
    }
}
