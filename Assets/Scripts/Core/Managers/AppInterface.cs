using UnityEngine;

public class AppInterface : MonoBehaviour
{
    private static AppInterface _instance;
    public static AppInterface Instance => _instance;

    public ServiceManager ServiceManager;
    public AlbumEntryManager AlbumEntryManager;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeManagers();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeManagers()
    {
        ServiceManager = new ServiceManager();

        var albumService = new AlbumAPIService();
        ServiceManager.RegisterService<IAlbumService>(albumService);

        var imageLoadingService = new ImageLoadingService();
        ServiceManager.RegisterService<IImageLoadingService>(imageLoadingService);

        AlbumEntryManager.Initialize();

        Debug.Log("AppInterface and all managers initialized.");
    }
}
