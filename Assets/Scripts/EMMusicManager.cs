using UnityEngine;

public class EMMusicManager : MonoBehaviour
{
    
    [SerializeField] private AudioSource menuMusic;
    float musicVolume;
    bool isSceneLoading;
    float trackLength;
    public AudioClip[] musicTracks;

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
}
