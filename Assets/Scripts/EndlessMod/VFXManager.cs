using UnityEngine;
using System.Collections;

public class VFXManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Player player;

    [Header("VFX")]
    [SerializeField] private GameObject barierHitVFX;
    [SerializeField] private GameObject powerUpHitVFX;
    [SerializeField] private GameObject obstacleDefaultHitVFX;
    [SerializeField] private GameObject obstacleSmashVFX;
    [SerializeField] private GameObject outOnTrackVFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player.BarierHit += OnBarierHit;
        Player.PowerUpHit += OnPowerUpHit;
        Player.ObstacleHit += OnObstacleHit;
        Player.ObstacleSmash += OnObstacleSmash;
        Player.OutOnTrack += OnOutOnTrack;
    }
    void OnDestroy()
    {
        Player.BarierHit -= OnBarierHit;
        Player.PowerUpHit -= OnPowerUpHit;
        Player.ObstacleHit -= OnObstacleHit;
        Player.ObstacleSmash -= OnObstacleSmash;
        Player.OutOnTrack -= OnOutOnTrack;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnBarierHit(Vector2 collisionPoint)
    {
        var vfx = Instantiate(barierHitVFX);
        vfx.transform.position = collisionPoint;
        
        Vector2 direction =  collisionPoint - (Vector2)player.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        vfx.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        StartCoroutine(TimerAndDestroy(1f, vfx.gameObject));
    }
    void OnPowerUpHit(Transform powerUpTransform)
    {
        var vfx = Instantiate(powerUpHitVFX);
        vfx.transform.position = powerUpTransform.position;
        StartCoroutine(TimerAndDestroy(1f, vfx.gameObject));
    }

    void OnObstacleHit(Transform obstacleTransform)
    {
        var vfx = Instantiate(obstacleDefaultHitVFX);
        vfx.transform.position = obstacleTransform.position;
        StartCoroutine(TimerAndDestroy(1f, vfx.gameObject));
    }
    void OnObstacleSmash(Transform obstacleTransform)
    {
        var vfx = Instantiate(obstacleSmashVFX);
        vfx.transform.position = obstacleTransform.position;
        StartCoroutine(TimerAndDestroy(1f, vfx.gameObject));
    }
    void OnOutOnTrack()
    {
        var vfx = Instantiate(outOnTrackVFX);
        vfx.transform.position = player.transform.position;
        StartCoroutine(TimerAndDestroy(1f, vfx.gameObject));
    }
    IEnumerator TimerAndDestroy(float time, GameObject vfx)
    {
        yield return new WaitForSeconds(time);
        Destroy(vfx);
    }
}
