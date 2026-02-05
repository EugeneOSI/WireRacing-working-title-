using UnityEngine;

public class EMSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private AudioClip barierHitSound;
    [SerializeField] private AudioClip powerUpHitSound;
    [SerializeField] private AudioClip obstacleHitSound;


    void Start()
    {
        Player.BarierHit += OnBarierHit;
        Player.PowerUpHit += OnPowerUpHit;
        Player.ObstacleHit += OnObstacleHit;
    }
    void OnDestroy()
    {
        Player.BarierHit -= OnBarierHit;
        Player.PowerUpHit -= OnPowerUpHit;
        Player.ObstacleHit -= OnObstacleHit;
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
    void OnPowerUpHit(Transform powerUpTransform)
    {
        audioSource.PlayOneShot(powerUpHitSound);
    }
    void OnObstacleHit(Transform obstacleTransform)
    {
        audioSource.PlayOneShot(obstacleHitSound);
    }
}
