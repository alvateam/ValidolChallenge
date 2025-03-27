using System;
using System.Collections;
using UnityEngine;

public class JsonDownloader : MonoBehaviour
{
    [SerializeField] private string _googleDriveFileId;
    [SerializeField] private FileDownloader _fileDownloader;

    public event Action<VideoJsonWrapper> Downloaded;
    
    private IEnumerator Start()
    {
        string downloadUrl = GetGoogleDriveDownloadUrl(_googleDriveFileId);
        yield return StartCoroutine(_fileDownloader.DownloadFile(downloadUrl, OnSuccess, OnError));
    }

    private string GetGoogleDriveDownloadUrl(string fileId) => 
        $"https://drive.google.com/uc?export=download&id={fileId}";

    private void OnError(string value)
    {
        Debug.LogError(value);
    }

    private void OnSuccess(string value)
    {
        VideoJsonWrapper videoDataWrapper = JsonUtility.FromJson<VideoJsonWrapper>(value);

        if (videoDataWrapper is { videos: not null })
            Downloaded?.Invoke(videoDataWrapper);
        else
            Debug.LogError("Failed to parse JSON or the 'videos' list is null.");
    }
}