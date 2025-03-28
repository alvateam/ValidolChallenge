using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FileDownloader : MonoBehaviour
{
    public IEnumerator DownloadFile(string fileURL, string downloadLocalPath = "", Action<string> onSuccess = null,
        Action<string> onError = null)
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
                // Сохраняем файл локально
                if (!string.IsNullOrEmpty(downloadLocalPath))
                    File.WriteAllBytes(downloadLocalPath, webRequest.downloadHandler.data);
                
                onSuccess?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }

    public async Task<string> DownloadFileAsync(string fileURL, string downloadLocalPath = "",
        Action<string> onSuccess = null, Action<string> onError = null)
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

                if (!string.IsNullOrEmpty(downloadLocalPath))
                    await File.WriteAllBytesAsync(downloadLocalPath, webRequest.downloadHandler.data);
                
                onSuccess?.Invoke(downloadLocalPath);
                return downloadLocalPath;
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
                return string.Empty;
            }
        }
    }
}