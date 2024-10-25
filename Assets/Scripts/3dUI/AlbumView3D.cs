using System.Collections.Generic;
using UnityEngine;

public class AlbumView3D : MonoBehaviour
{
    public Transform contentContainer;
    public ThumbnailView3D thumbnailPrefab;
    public EntryView3D entryView;

    private List<IAlbumEntry> albumEntries = new List<IAlbumEntry>();
    private IAlbumEntry selectedEntry;
    private AlbumEntryManager albumEntryManager;

    private void Awake()
    {
        albumEntryManager = AppInterface.Instance.AlbumEntryManager;
        albumEntryManager.OnEntryCreated += AddThumbnail;
        ThumbnailEventManager.OnThumbnailClicked += HandleThumbnailClicked;
    }

    public void AddNewEntry()
    {
        albumEntryManager.LoadNextAlbumEntry();
    }

    private void AddThumbnail(IAlbumEntry entry)
    {
        albumEntries.Add(entry);
        var thumbnail = Instantiate(thumbnailPrefab, contentContainer);
        thumbnail.transform.position += Vector3.up * albumEntries.Count * 3 - Vector3.forward * albumEntries.Count * 8f;
        thumbnail.Initialize(entry);
        thumbnail.OnThumbnailClicked += HandleThumbnailClicked;
    }

    public void HandleThumbnailClicked(IAlbumEntry clickedEntry)
    {
        if (selectedEntry == clickedEntry) return;

        selectedEntry = clickedEntry;
        entryView.UpdateView(selectedEntry);
        ThumbnailEventManager.RaiseThumbnailClicked(clickedEntry);
    }

    public void DeleteSelectedEntry()
    {
        if (selectedEntry != null)
        {
            albumEntries.Remove(selectedEntry);
            entryView.UpdateView(null);
            albumEntryManager.DeleteSelectedEntry();
        }
    }
}
