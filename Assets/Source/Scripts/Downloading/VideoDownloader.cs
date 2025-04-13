using System;
using System.IO;
using System.Threading.Tasks;
using Source.Scripts.Data;
using UnityEngine;

namespace Source.Scripts.Downloading
{
    public class VideoDownloader : MonoBehaviour
    {
        [SerializeField] private FileDownloader _fileDownloader;
    
        public async Task<DownloadedVideo> DownloadVideoAsync(DownloadedVideo value)
        {
            if(string.IsNullOrEmpty(value.Url))
                throw new NullReferenceException();
        
            string path = GetResourcesPath(value.VideoType, value.Id);
        
            if (File.Exists(path))
                return new DownloadedVideo(value.Id, value.VideoType, "file://" + path);
        
            path = GetPreloadedPath(value.VideoType, value.Id);
        
            if (File.Exists(path))
                return new DownloadedVideo(value.Id, value.VideoType, path);
        
            string downloadLink = GoogleDriveLinkConverter.GetGoogleDriveDownloadUrl(value.Url);
            string downloadFile = await _fileDownloader.DownloadFileAsync(downloadLink, path);
            return new DownloadedVideo(value.Id, value.VideoType, downloadFile);
        }

        private string GetResourcesPath(VideoType videoType, int id)
        {
            string videoFileName = $"{videoType}_{id}.mp4";
            return Path.Combine(Application.dataPath, "Source/Resources/Videos", videoFileName);
        }
    
        private string GetPreloadedPath(VideoType videoType, int id)
        {
            string folderPath = Path.Combine(Application.persistentDataPath, "Videos");
        
            CreateDirectoryIfNotExists(folderPath);
        
            return Path.Combine(folderPath, $"{videoType}_{id}.mp4");
        }

        private void CreateDirectoryIfNotExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log($"Folder created: {folderPath}");
            }
        }
    }
}
