using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoadingService : IImageLoadingService
{
    private readonly Dictionary<string, Texture2D> memoryCache = new Dictionary<string, Texture2D>();
    private readonly Dictionary<string, Task<Texture2D>> loadingTasks = new Dictionary<string, Task<Texture2D>>();
    private readonly string diskCachePath;

    private const int MaxRetryCount = 3;
    private const int InitialRetryDelayMs = 500;

    public ImageLoadingService()
    {
        diskCachePath = Path.Combine(Application.persistentDataPath, "ImageCache");
        if (!Directory.Exists(diskCachePath))
        {
            Directory.CreateDirectory(diskCachePath);
        }
    }

    public async Task<Texture2D> LoadImageAsync(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("Image URL is null or empty");
            return null;
        }

        if (memoryCache.TryGetValue(url, out Texture2D cachedTexture))
        {
            return cachedTexture;
        }

        string filePath = GetCacheFilePath(url);
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                memoryCache[url] = texture;
                return texture;
            }
        }

        if (loadingTasks.TryGetValue(url, out Task<Texture2D> existingTask))
        {
            return await existingTask;
        }

        var downloadTask = DownloadAndCacheImageWithRetryAsync(url);
        loadingTasks[url] = downloadTask;

        try
        {
            Texture2D texture = await downloadTask;
            return texture;
        }
        finally
        {
            loadingTasks.Remove(url);
        }
    }

    private async Task<Texture2D> DownloadAndCacheImageWithRetryAsync(string url)
    {
        int attempt = 0;
        int delay = InitialRetryDelayMs;

        while (attempt < MaxRetryCount)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                    if (texture != null)
                    {
                        memoryCache[url] = texture;
                        string filePath = GetCacheFilePath(url);
                        File.WriteAllBytes(filePath, texture.EncodeToPNG());
                        return texture;
                    }
                    else
                    {
                        Debug.LogError("Failed to create texture from downloaded image.");
                    }
                }
                else if (request.responseCode == 504)
                {
                    attempt++;
                    Debug.LogWarning($"504 Gateway Timeout. Retrying {attempt}/{MaxRetryCount} in {delay}ms...");
                    await Task.Delay(delay);
                    delay *= 2; // Exponential backoff
                }
                else
                {
                    Debug.LogError($"Error downloading image from {url}: {request.error}");
                    break;
                }
            }
        }

        Debug.LogError($"Failed to download image after {MaxRetryCount} attempts: {url}");
        return null;
    }

    private string GetCacheFilePath(string url)
    {
        string fileName = GetMD5Hash(url) + ".png";
        return Path.Combine(diskCachePath, fileName);
    }

    private string GetMD5Hash(string input)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
