using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FileDownloader : MonoBehaviour
{
    public IEnumerator DownloadFile(string fileURL, string downloadLocalPath, Action<string> onSuccess = null, Action<string> onError = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(fileURL))
        {
            // Отправляем запрос
            yield return webRequest.SendWebRequest();

            // Проверяем ошибки
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(webRequest.error);
            }
            else
            {
                onSuccess?.Invoke(webRequest.downloadHandler.text);
                // Сохраняем файл локально
                File.WriteAllBytes(downloadLocalPath, webRequest.downloadHandler.data);
            }
        }
    }
    
    public IEnumerator DownloadFile(string fileURL, Action<string> onSuccess = null, Action<string> onError = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(fileURL))
        {
            // Отправляем запрос
            yield return webRequest.SendWebRequest();

            // Проверяем ошибки
            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(webRequest.error);
            }
            else
            {
                onSuccess?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }
    
    public async Task<string> DownloadFileAsync(string fileURL, string downloadLocalPath, Action<string> onSuccess = null, Action<string> onError = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(fileURL))
        {
            try
            {
                // Отправляем запрос и ждём завершения асинхронно
                var operation = webRequest.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield(); // Освобождаем управление, чтобы не блокировать основной поток
                }

                // Проверяем ошибки
                if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    onError?.Invoke(webRequest.error);
                    return string.Empty;
                }
                else
                {
                    await File.WriteAllBytesAsync(downloadLocalPath, webRequest.downloadHandler.data);
                    onSuccess?.Invoke(downloadLocalPath);
                    return downloadLocalPath;
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений
                onError?.Invoke($"Произошла ошибка: {ex.Message}");
                return string.Empty;
            }
        }
    }
}