using System;

public static class ThumbnailEventManager
{
    public static event Action<IAlbumEntry> OnThumbnailClicked;
    public static event Action<IAlbumEntry> OnThumbnailDeleted;

    public static void RaiseThumbnailClicked(IAlbumEntry entry)
    {
        OnThumbnailClicked?.Invoke(entry);
    }

    public static void RaiseThumbnailDeleted(IAlbumEntry entry)
    {
        OnThumbnailDeleted?.Invoke(entry);
    }
}
