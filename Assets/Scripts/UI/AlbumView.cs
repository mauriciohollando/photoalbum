using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumView : MonoBehaviour
{
    public Transform contentContainer;
    public ThumbnailView thumbnailPrefab;
    public AlbumEntryView entryView;
    public Button addButton;
    public Button deleteButton;
    public Button logButton;
    
    private List<IAlbumEntry> albumEntries = new List<IAlbumEntry>();
    private Dictionary<int, ThumbnailView> idsToThumbnailViews = new Dictionary<int, ThumbnailView>();
    
    private IAlbumEntry selectedEntry;
    private AlbumEntryManager albumEntryManager;

    private void Start()
    {
        albumEntryManager = AppInterface.Instance.AlbumEntryManager;
        albumEntryManager.OnEntryCreated += AddThumbnail;
        
        addButton.onClick.AddListener(AddNewEntry);
        deleteButton.onClick.AddListener(DeleteSelectedEntry);
        logButton.onClick.AddListener(PrintLogs);
        
        ThumbnailEventManager.OnThumbnailClicked += HandleThumbnailClicked;
        ThumbnailEventManager.OnThumbnailDeleted += HandleThumbnailDeleted;
    }

    public void PrintLogs()
    {
        foreach (var entry in albumEntries)
        {
            Debug.Log($"Id:{entry.Id}\nTitle:{entry.Title}\nUrl:{entry.Url}\nThumbnailUrl:{entry.ThumbnailUrl}");
        }
    }
    
    public void AddNewEntry()
    {
        albumEntryManager.LoadNextAlbumEntry();
    }

    private void AddThumbnail(IAlbumEntry entry)
    {
        albumEntries.Add(entry);
        var thumbnail = Instantiate(thumbnailPrefab, contentContainer);
        thumbnail.Initialize(entry);
        thumbnail.OnThumbnailClicked += HandleThumbnailClicked;
        idsToThumbnailViews.Add(entry.Id, thumbnail);
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
            if (idsToThumbnailViews.TryGetValue(selectedEntry.Id, out ThumbnailView view))
            {
                Destroy(view.gameObject);
                idsToThumbnailViews.Remove(selectedEntry.Id);
            }

            albumEntries.Remove(selectedEntry);
            entryView.UpdateView(null);

            // Notify the 3D view of the deletion, but don't call the manager delete again
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

        if (idsToThumbnailViews.TryGetValue(deletedEntry.Id, out ThumbnailView view))
        {
            Destroy(view.gameObject);
            idsToThumbnailViews.Remove(deletedEntry.Id);
        }
        
        albumEntries.Remove(deletedEntry);
    }
}
