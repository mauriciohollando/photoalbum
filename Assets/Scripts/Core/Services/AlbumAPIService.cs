using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class AlbumAPIService : IAlbumService
{
    private string apiUrl = "https://jsonplaceholder.typicode.com/photos/";
    private int currentIndex = 1;

    public IEnumerator FetchAlbumEntry(System.Action<AlbumEntry> onSuccess, System.Action<string> onError)
    {
        string fullUrl = apiUrl + currentIndex.ToString();
        UnityWebRequest request = UnityWebRequest.Get(fullUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            AlbumEntry albumEntry = JsonConvert.DeserializeObject<AlbumEntry>(request.downloadHandler.text);
            onSuccess?.Invoke(albumEntry);
            currentIndex++;
        }
        else
        {
            onError?.Invoke("Error fetching data: " + request.error);
        }
    }
}
