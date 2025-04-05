using System;

public class GoogleDriveLinkConverter
{
    /// <summary>
    /// Преобразует ссылку Google Диска в прямую ссылку для скачивания.
    /// </summary>
    /// <param name="googleDriveLink">Исходная ссылка на файл Google Диска.</param>
    /// <returns>Прямая ссылка для скачивания файла.</returns>
    public static string ConvertToDirectDownloadLink(string googleDriveLink)
    {
        // Проверяем, что ссылка не пустая
        if (string.IsNullOrEmpty(googleDriveLink))
        {
            throw new ArgumentException("Ссылка не может быть пустой.");
        }

        // Извлекаем FILE_ID из ссылки
        string fileId = ExtractFileId(googleDriveLink);

        // Формируем прямую ссылку для скачивания
        string directDownloadLink = $"https://drive.google.com/uc?export=download&id={fileId}";

        return directDownloadLink;
    }
    
    public static string GetGoogleDriveDownloadUrl(string fileId) => 
        $"https://drive.google.com/uc?export=download&id={fileId}";

    /// <summary>
    /// Извлекает FILE_ID из ссылки Google Диска.
    /// </summary>
    /// <param name="googleDriveLink">Исходная ссылка на файл Google Диска.</param>
    /// <returns>FILE_ID файла.</returns>
    private static string ExtractFileId(string googleDriveLink)
    {
        // Определяем ключевые части ссылки
        const string fileIdMarker = "/file/d/";
        const string viewMarker = "/view";

        // Находим позицию начала и конца FILE_ID
        int startIndex = googleDriveLink.IndexOf(fileIdMarker, StringComparison.Ordinal);
        if (startIndex == -1)
        {
            throw new ArgumentException("Неверный формат ссылки Google Диска.");
        }

        startIndex += fileIdMarker.Length; // Сдвигаем индекс до начала FILE_ID

        int endIndex = googleDriveLink.IndexOf(viewMarker, startIndex, StringComparison.Ordinal);
        if (endIndex == -1)
        {
            throw new ArgumentException("Неверный формат ссылки Google Диска.");
        }

        // Извлекаем FILE_ID
        string fileId = googleDriveLink.Substring(startIndex, endIndex - startIndex);

        return fileId;
    }
}