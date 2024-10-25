using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlbumEntryView : MonoBehaviour
{
    public Image largeImage;
    public TextMeshProUGUI titleText;

    private Task currentLoadTask;

    public void UpdateView(IAlbumEntry entry)
    {
        if (entry != null)
        {
            titleText.text = entry.Title;
            if (currentLoadTask != null && !currentLoadTask.IsCompleted)
            {
                currentLoadTask = null;
            }
            currentLoadTask = LoadLargeImageAsync(entry.Url);
        }
        else
        {
            _ = AnimateImageAlpha(0f);
            titleText.text = "";
        }
    }

    private async Task LoadLargeImageAsync(string url)
    {
        if (string.IsNullOrEmpty(url)) return;

        await AnimateImageAlpha(0f);
        var imageLoadingService = AppInterface.Instance.ServiceManager.GetService<IImageLoadingService>();

        try
        {
            Texture2D texture = await imageLoadingService.LoadImageAsync(url);
            if (texture != null)
            {
                largeImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                await AnimateImageAlpha(1f);
            }
        }
        catch
        {
            Debug.LogError($"Error loading image from {url}");
        }

        currentLoadTask = null;
    }

    private async Task AnimateImageAlpha(float targetAlpha)
    {
        float duration = 0.25f;
        float startAlpha = largeImage.color.a;
        float time = 0;
        Color originalColor = largeImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            largeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            await Task.Yield();
        }

        largeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, targetAlpha);
    }
}
