using System.Collections.Generic;
using TS.PageSlider;
using UnityEngine;

namespace Source.Scripts
{
    public class VideosSliderHandler : MonoBehaviour
    {
        [SerializeField] private VideosFactory _videosFactory;
        [SerializeField] private PageSlider _pageSlider;
        [SerializeField] private VideosLifeCycleController _videoLifeCycleController;

        private void Awake()
        {
            _videosFactory.VideoContainersCreated += OnVideoContainersCreated;
            _videoLifeCycleController.CurrentVideoIndexChanged += OnCurrentVideoIndexChanged;
        }

        private void OnCurrentVideoIndexChanged(int value)
        {
            _pageSlider.SetPage(value);
        }

        private void OnVideoContainersCreated(IReadOnlyList<VideoBootstrapper> value)
        {
            foreach (VideoBootstrapper videoBootstrapper in value)
            {
                videoBootstrapper.TapsCounter.TappingStated += OnTappingStarted;
                videoBootstrapper.VideoPlayerWrapper.FinalVideoFinished += OnFinalVideoFinished;
            }
        }

        private void OnFinalVideoFinished() => 
            _pageSlider.SetScrollingAllow(true);

        private void OnTappingStarted() => 
            _pageSlider.SetScrollingAllow(false);
    }
}
