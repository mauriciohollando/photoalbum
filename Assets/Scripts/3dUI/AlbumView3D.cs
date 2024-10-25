using System.Collections.Generic;
using UnityEngine;

public class AlbumView3D : MonoBehaviour
{
    public Transform contentContainer;
    public ThumbnailView3D thumbnailPrefab;
    public EntryView3D entryView;

    private List<IAlbumEntry> albumEntries = new List<IAlbumEntry>();
    private Dictionary<int, ThumbnailView3D> idsToThumbnailViews3D = new Dictionary<int, ThumbnailView3D>();
    private IAlbumEntry selectedEntry;
    private AlbumEntryManager albumEntryManager;

    private void Awake()
    {
        albumEntryManager = AppInterface.Instance.AlbumEntryManager;
        albumEntryManager.OnEntryCreated += AddThumbnail;
        
        ThumbnailEventManager.OnThumbnailClicked += HandleThumbnailClicked;
        ThumbnailEventManager.OnThumbnailDeleted += HandleThumbnailDeleted;
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
        idsToThumbnailViews3D.Add(entry.Id, thumbnail);
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
            if (idsToThumbnailViews3D.TryGetValue(selectedEntry.Id, out ThumbnailView3D view))
            {
                Destroy(view.gameObject);
                idsToThumbnailViews3D.Remove(selectedEntry.Id);
            }

            albumEntries.Remove(selectedEntry);
            entryView.UpdateView(null);
            ThumbnailEventManager.RaiseThumbnailDeleted(selectedEntry);
            selectedEntry = null;
        }
    }

    private void HandleThumbnailDeleted(IAlbumEntry deletedEntry)
    {
        if (selectedEntry == deletedEntry)
        {
            selectedEntry = null;
            entryView.UpdateView(null);
        }

        if (idsToThumbnailViews3D.TryGetValue(deletedEntry.Id, out ThumbnailView3D view))
        {
            Destroy(view.gameObject);
            idsToThumbnailViews3D.Remove(deletedEntry.Id);
        }

        albumEntries.Remove(deletedEntry);
    }
}
