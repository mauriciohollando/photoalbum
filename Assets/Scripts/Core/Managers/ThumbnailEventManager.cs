using System;

public static class ThumbnailEventManager
{
    public static event Action<IAlbumEntry> OnThumbnailClicked;

    public static void RaiseThumbnailClicked(IAlbumEntry entry)
    {
        OnThumbnailClicked?.Invoke(entry);
    }
}
