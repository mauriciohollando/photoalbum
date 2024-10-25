using System.Collections;
using System.Collections.Generic;

public interface IAlbumService : IService
{
    IEnumerator FetchAlbumEntry(System.Action<AlbumEntry> onSuccess, System.Action<string> onError);
}
