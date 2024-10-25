using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class ThumbnailView : MonoBehaviour
{
    public Image thumbnailImage;
    public TextMeshProUGUI titleText;

    private IAlbumEntry albumEntry;

    public event System.Action<IAlbumEntry> OnThumbnailClicked;

    public void Initialize(IAlbumEntry entry)
    {
        albumEntry = entry;
        titleText.text = entry.Title;

        LoadThumbnailImage(entry.ThumbnailUrl);
    }

    private async void LoadThumbnailImage(string url)
    {
        var imageLoadingService = AppInterface.Instance.ServiceManager.GetService<IImageLoadingService>();

        try
        {
            Texture2D texture = await imageLoadingService.LoadImageAsync(url);
            if (texture != null)
            {
                thumbnailImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
            else
            {
                Debug.LogError("Failed to load thumbnail image.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading thumbnail image: {e.Message}");
        }
    }

    public void OnThumbnailClick()
    {
        OnThumbnailClicked?.Invoke(albumEntry);
    }
}
