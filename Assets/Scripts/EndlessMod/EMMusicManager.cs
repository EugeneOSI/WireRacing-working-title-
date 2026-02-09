using UnityEngine;

public class EMMusicManager : MonoBehaviour
{
    
    [SerializeField] private AudioSource menuMusic;
    bool isSceneLoading;
    float trackLength;
    public AudioClip[] musicTracks;

        void Awake()
    {
        GameManager.OnSceneLoading += OnSceneLoading;
        EM_GameManager.OnGameOver += OnGameOver;
    }
    void OnDestroy()
    {
        GameManager.OnSceneLoading -= OnSceneLoading;
        EM_GameManager.OnGameOver -= OnGameOver;
    }
    void Start()
    {
        isSceneLoading = false;
        SetTrack();
    }
    

    void Update()
    {
        if (!isSceneLoading){
        menuMusic.volume = PrefsManager.Instance.musicVolume;
        trackLength -= Time.deltaTime;
        if (trackLength <= 0){
            SetTrack();
        }
        }
        else{
            menuMusic.volume = Mathf.Lerp(menuMusic.volume, 0, Time.deltaTime * 3f);
        }
    }
    void OnSceneLoading()
    {
        isSceneLoading = true;
    }
    void SetTrack(){
        menuMusic.clip = musicTracks[Random.Range(0, musicTracks.Length)];
        menuMusic.Play();
        trackLength = menuMusic.clip.length;
    }
    void OnGameOver(){
        menuMusic.Stop();
    }
}
