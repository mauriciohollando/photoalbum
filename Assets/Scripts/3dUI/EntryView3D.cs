using System.Threading.Tasks;
using UnityEngine;
using TMPro;

public class EntryView3D : MonoBehaviour
{
    public MeshRenderer largeImageRenderer;
    public TextMeshPro titleText;

    public void UpdateView(IAlbumEntry entry)
    {
        if (entry != null)
        {
            titleText.text = entry.Title;
            LoadLargeImage(entry.Url);
        }
    }

    private async void LoadLargeImage(string url)
    {
        var imageLoadingService = AppInterface.Instance.ServiceManager.GetService<IImageLoadingService>();
        try
        {
            Texture2D texture = await imageLoadingService.LoadImageAsync(url);
            if (texture != null)
            {
                largeImageRenderer.material.mainTexture = texture;
            }
        }
        catch
        {
            Debug.LogError("Error loading large image");
        }
    }
}
