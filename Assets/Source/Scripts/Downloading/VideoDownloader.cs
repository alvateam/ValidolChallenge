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
    
    public async Task<DownloadedVideo> DownloadVideoAsync(DownloadedVideo value)
    {
        if(string.IsNullOrEmpty(value.Url))
            throw new NullReferenceException();
        
        string downloadLink = GoogleDriveLinkConverter.GetGoogleDriveDownloadUrl(value.Url);
        string downloadLocalPath = GetDownloadLocalPath(value.VideoType, value.Id);
        
        if (File.Exists(downloadLocalPath))
            return new DownloadedVideo(value.Id, value.VideoType, downloadLocalPath);
        
        string downloadFile = await _fileDownloader.DownloadFileAsync(downloadLink, downloadLocalPath, OnSuccess, OnError);
        return new DownloadedVideo(value.Id, value.VideoType, downloadFile);
    }

    private string GetDownloadLocalPath(VideoType videoType, int id)
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
