using Source.Scripts.Downloading;
using Source.Scripts.Save;
using UnityEngine;

namespace Source.Scripts
{
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
}
