using UnityEngine;
using System.Collections;

public class VFXManager : MonoBehaviour
{

    GameObject tmpVfx;
    [Header("References")] 
    [SerializeField] private Player player;

    [Header("VFX")]
    [SerializeField] private GameObject barierHitVFX;
    [SerializeField] private GameObject powerUpHitVFX;
    [SerializeField] private GameObject obstacleDefaultHitVFX;
    [SerializeField] private GameObject obstacleHit2VFX;
    [SerializeField] private GameObject obstacleSmashVFX;
    [SerializeField] private GameObject outOnTrackVFX;
    [SerializeField] private GameObject enemyFreezedVFX;
    [SerializeField] private GameObject freezedText;
    [SerializeField] private GameObject deadExplosion1;
    [SerializeField] private GameObject deadExplosion2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player.BarierHit += OnBarierHit;
        Player.PowerUpHit += OnPowerUpHit;
        Player.ObstacleHit += OnObstacleHit;
        Player.ObstacleSmash += OnObstacleSmash;
        Player.OutOnTrack += OnOutOnTrack;
        PursuingEnemy.EnemyFreezed += OnEnemyFreezed;
        EM_GameManager.OnGameOver += OnDead;
    }
    void OnDestroy()
    {
        Player.BarierHit -= OnBarierHit;
        Player.PowerUpHit -= OnPowerUpHit;
        Player.ObstacleHit -= OnObstacleHit;
        Player.ObstacleSmash -= OnObstacleSmash;
        Player.OutOnTrack -= OnOutOnTrack;
        PursuingEnemy.EnemyFreezed -= OnEnemyFreezed;
        EM_GameManager.OnGameOver -= OnDead;
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
        var vfx2 = Instantiate(obstacleHit2VFX);
        vfx2.transform.position = obstacleTransform.position;
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
    void OnEnemyFreezed(Transform enemyTransform){
        var vfx = Instantiate(enemyFreezedVFX);
        GameObject text = Instantiate(freezedText);
        text.transform.position = enemyTransform.position + new Vector3(0, 4, 0);
        text.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Active");
        vfx.transform.position = enemyTransform.position;
        StartCoroutine(TimerAndDestroy(2f, vfx.gameObject));
        StartCoroutine(TimerAndDestroyText(3f, text, "Unactive"));
    }
    void OnDead()
    {
        var vfx2 = Instantiate(deadExplosion2);
        vfx2.transform.position = player.transform.position;
        StartCoroutine(TimerAndInstantiate(1f, deadExplosion1));
    }
    IEnumerator TimerAndDestroy(float time, GameObject vfx)
    {
        yield return new WaitForSeconds(time);
        Destroy(vfx);
    }

    IEnumerator TimerAndDestroyText(float time, GameObject text, string animTrigger){
        yield return new WaitForSeconds(time);
        text.transform.GetChild(0).GetComponent<Animator>().SetTrigger(animTrigger);
        yield return new WaitForSeconds(1f);
        Destroy(text);

    }
    IEnumerator TimerAndInstantiate(float time, GameObject vfx){
        yield return new WaitForSeconds(time);
        Instantiate(vfx, player.transform.position, Quaternion.identity);
    }
}
