using UnityEngine;

public class TTSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;   
    [SerializeField] private AudioClip barierHitSound;
    [SerializeField] private AudioClip outOnTrackSound;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake(){
        PlayerTimeTrial.BarierHitTT += OnBarierHit;
        PlayerTimeTrial.OutOnTrackTT += OnOutOnTrack;
        GameManager.OnPauseEvent += OnPause;
        GameManager.OnUnpauseEvent += OnUnpause;
    }
    void OnDestroy(){
        PlayerTimeTrial.BarierHitTT -= OnBarierHit;
        PlayerTimeTrial.OutOnTrackTT -= OnOutOnTrack;
        GameManager.OnPauseEvent -= OnPause;
        GameManager.OnUnpauseEvent -= OnUnpause;
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = PrefsManager.Instance.soundsVolume;
    }
    void OnBarierHit(Vector2 collisionPoint)
    {
        audioSource.PlayOneShot(barierHitSound);
    }
    void OnOutOnTrack()
    {
        audioSource.PlayOneShot(outOnTrackSound);
    }
    void OnPause()
    {
        audioSource.volume = 0;
    }
    void OnUnpause()
    {
        audioSource.volume = PrefsManager.Instance.soundsVolume;
    }
}
