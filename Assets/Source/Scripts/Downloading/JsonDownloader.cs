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
        string downloadUrl = GoogleDriveLinkConverter.GetGoogleDriveDownloadUrl(_googleDriveFileId);
        yield return StartCoroutine(_fileDownloader.DownloadFile(downloadUrl, onSuccess: OnSuccess, onError: OnError));
    }

    private void OnError(string value) => Debug.LogError(value);

    private void OnSuccess(string value)
    {
        VideoJsonWrapper videoDataWrapper = JsonUtility.FromJson<VideoJsonWrapper>(value);
        Debug.Log("Json download finished");
        Downloaded?.Invoke(videoDataWrapper);
    }
}