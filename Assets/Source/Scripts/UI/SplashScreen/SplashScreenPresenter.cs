using System;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenPresenter : MonoBehaviour
{
    [SerializeField] private SplashScreenView _view;
    [SerializeField] private VideosFactory _videosFactory;

    private void Awake()
    {
        _videosFactory.VideoContainersCreated += OnVideoContainersCreated;
    }

    private void OnVideoContainersCreated(IReadOnlyList<VideoBootstrapper> obj)
    {
        _view.Hide();
    }
}
