using System;
using System.IO;
using System.Threading.Tasks;
using Data;
using UnityEngine;

public class VideoDownloader : MonoBehaviour
{
    [SerializeField] private FileDownloader _fileDownloader;
    
    public event Action<string> Success;
    public event Action<string> Error;
    
    public async Task<string> DownloadVideoAsync(string url, VideoType videoType, string id)
    {
        string downloadLink = GoogleDriveLinkConverter.ConvertToDirectDownloadLink(url);
        string downloadLocalPath = GetDownloadLocalPath(videoType, id);
        
        if (File.Exists(downloadLocalPath))
            return downloadLocalPath;
        
        return await _fileDownloader.DownloadFileAsync(downloadLink, downloadLocalPath, OnSuccess, OnError);
    }

    private string GetDownloadLocalPath(VideoType videoType, string id)
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

    private void OnError(string value) => Error?.Invoke(value);
    private void OnSuccess(string value) => Success?.Invoke(value);
}
