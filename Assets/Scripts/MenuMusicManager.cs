using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private AudioSource menuMusic;
    float musicVolume;
    bool isSceneLoading;
    
    void Awake()
    {
        GameManager.OnSceneLoading += OnSceneLoading;
    }
    void OnDestroy()
    {
        GameManager.OnSceneLoading -= OnSceneLoading;
    }
    void Start()
    {
        isSceneLoading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSceneLoading){
        menuMusic.volume = PrefsManager.Instance.musicVolume;
        }
        else{
            menuMusic.volume = Mathf.Lerp(menuMusic.volume, 0, Time.deltaTime * 3f);
        }
    }
    void OnSceneLoading()
    {
        isSceneLoading = true;
    }
}
