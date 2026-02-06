using UnityEngine;
using System.Collections;

public class EMSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;   
    [SerializeField] private AudioClip barierHitSound;
    [SerializeField] private AudioClip powerUpHitSound;
    [SerializeField] private AudioClip obstacleHitSound;
    [SerializeField] private AudioClip obstacleSmashSound;
    [SerializeField] private AudioClip enemyFreezedSound;
    [SerializeField] private AudioClip outOnTrackSound;
    [SerializeField] private AudioClip deadExplosion1;
    [SerializeField] private AudioClip deadExplosion2;


    bool paused = false;


    void Start()
    {
        Player.BarierHit += OnBarierHit;
        Player.PowerUpHit += OnPowerUpHit;
        Player.ObstacleHit += OnObstacleHit;
        Player.ObstacleSmash += OnObstacleSmash;
        PursuingEnemy.EnemyFreezed += OnEnemyFreezed;
        Player.OutOnTrack += OnOutOnTrack;
        EM_GameManager.OnGameOver += OnDead;
        GameManager.OnPauseEvent += OnPause;
        GameManager.OnUnpauseEvent += OnUnpause;
    }
    void OnDestroy()
    {
        Player.BarierHit -= OnBarierHit;
        Player.PowerUpHit -= OnPowerUpHit;
        Player.ObstacleHit -= OnObstacleHit;
        Player.ObstacleSmash -= OnObstacleSmash;
        PursuingEnemy.EnemyFreezed -= OnEnemyFreezed;
        Player.OutOnTrack -= OnOutOnTrack;
        EM_GameManager.OnGameOver -= OnDead;
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
    void OnPowerUpHit(Transform powerUpTransform)
    {
        audioSource.PlayOneShot(powerUpHitSound);
    }
    void OnObstacleHit(Transform obstacleTransform)
    {
        audioSource.PlayOneShot(obstacleHitSound);
    }
    void OnObstacleSmash(Transform obstacleTransform)
    {
        audioSource.PlayOneShot(obstacleSmashSound);
    }
    void OnEnemyFreezed(Transform enemyTransform)
    {
        audioSource.PlayOneShot(enemyFreezedSound);
    }
    void OnOutOnTrack()
    {
        audioSource.PlayOneShot(outOnTrackSound);
    }
    void OnDead()
    {
        audioSource.PlayOneShot(deadExplosion1);
        StartCoroutine(TimerAndPlay(1f, deadExplosion2));
    }

    IEnumerator TimerAndPlay(float time, AudioClip audioClip)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("SoundPlayed");
        audioSource.PlayOneShot(audioClip);
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
