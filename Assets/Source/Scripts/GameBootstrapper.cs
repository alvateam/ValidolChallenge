using System;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField] private JsonDownloader _jsonDownloader;
    [SerializeField] private SaveService _saveService;

    private void Awake()
    {
        _saveService.Load();
        _jsonDownloader.Download();
    }
}
