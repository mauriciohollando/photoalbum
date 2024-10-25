using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class ThumbnailView3D : MonoBehaviour
{
    public MeshRenderer thumbnailRenderer;
    public TextMeshPro titleText;

    private IAlbumEntry albumEntry;

    public event System.Action<IAlbumEntry> OnThumbnailClicked;

    public void Initialize(IAlbumEntry entry)
    {
        albumEntry = entry;
        titleText.text = entry.Title;
        LoadThumbnail(entry.ThumbnailUrl);
    }

    private async void LoadThumbnail(string url)
    {
        var imageLoadingService = AppInterface.Instance.ServiceManager.GetService<IImageLoadingService>();
        try
        {
            Texture2D texture = await imageLoadingService.LoadImageAsync(url);
            if (texture != null)
            {
                thumbnailRenderer.material.mainTexture = texture;
            }
        }
        catch
        {
            Debug.LogError("Error loading thumbnail image");
        }
    }

    private void OnMouseDown()
    {
        OnThumbnailClicked?.Invoke(albumEntry);
    }
}
