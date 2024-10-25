using System;
using System.Collections.Generic;
using UnityEngine;

public class AlbumEntryManager : MonoBehaviour
{
    private IAlbumEntry selectedEntry;

    private List<IAlbumEntry> albumEntries = new List<IAlbumEntry>();


    public event Action<IAlbumEntry> OnEntryCreated;
    public event Action<IAlbumEntry, IAlbumEntry> OnSelectedEntryChanged;
    public event Action<IAlbumEntry> OnEntryDeleted;

    private IAlbumService albumService;

    public void Initialize()
    {
        albumService = AppInterface.Instance.ServiceManager.GetService<IAlbumService>();
    }

    public void LoadNextAlbumEntry()
    {
        StartCoroutine(albumService.FetchAlbumEntry(OnAlbumEntryFetched, OnAlbumEntryFetchFailed));
    }

    private void OnAlbumEntryFetched(IAlbumEntry entry)
    {
        albumEntries.Add(entry);
        OnEntryCreated?.Invoke(entry); 

        Debug.Log($"Album entry created: {entry.Title}");
    }

    private void OnAlbumEntryFetchFailed(string error)
    {
        Debug.LogError("Failed to load album entry: " + error);
    }

    public void SelectEntry(IAlbumEntry entry)
    {
        if (selectedEntry != entry)
        {
            IAlbumEntry previouslySelectedEntry = selectedEntry;

            selectedEntry = entry;

            OnSelectedEntryChanged?.Invoke(previouslySelectedEntry, selectedEntry);
            
            Debug.Log("Album entry selected: " + selectedEntry.Title);
        }
    }

    public void DeselectCurrentEntry()
    {
        if (selectedEntry != null)
        {
            var previouslySelectedEntry = selectedEntry;
            selectedEntry = null;

            OnSelectedEntryChanged?.Invoke(previouslySelectedEntry, null);

            Debug.Log("Deselected entry: " + previouslySelectedEntry.Title);
        }
    }


    public void DeleteSelectedEntry()
    {
        if (selectedEntry != null)
        {
            albumEntries.Remove(selectedEntry);
            OnEntryDeleted?.Invoke(selectedEntry);
            Debug.Log("Deleted selected entry: " + selectedEntry.Title);
            selectedEntry = null;
        }
    }
}
