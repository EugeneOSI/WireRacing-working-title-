using UnityEngine;
using System.Collections;

public class Obstacles : MonoBehaviour
{
    IEnumerator timerAndDestroyCoroutine;
    Animator animator;
    bool exploded = false;

    [Header("Particles")]
    [SerializeField] private ParticleSystem circleParticles;

    void Start()
    {
        animator = GetComponent<Animator>();
        circleParticles.Stop();
        Player.PowerUpActive += OnPowerUpActive;
        Player.PowerUpEnd += OnPowerUpEnd;
        Player.ObstacleHit += OnObstacleHit;
        Player.ObstacleSmash += OnObstacleSmash;
    }
    void OnDestroy(){
        Player.PowerUpActive -= OnPowerUpActive;
        Player.PowerUpEnd -= OnPowerUpEnd;
        Player.ObstacleHit -= OnObstacleHit;
        Player.ObstacleSmash -= OnObstacleSmash;
    }
    void OnPowerUpActive()
    {
        if (!exploded){
        animator.SetTrigger("PowerUp Active");
        circleParticles.Play();
        }
    }
    void OnPowerUpEnd()
    {
        if (!exploded){
        animator.SetTrigger("PowerUp Unactive");
        circleParticles.Stop();}
    }
    void OnObstacleHit(Transform obstacleTransform)
    {
        if (obstacleTransform == transform)
        {
            animator.SetTrigger("Explosed");
            exploded = true;
            GetComponent<Collider2D>().enabled = false;
            if (timerAndDestroyCoroutine != null)
            {
                StopCoroutine(timerAndDestroyCoroutine);
            }
            timerAndDestroyCoroutine = TimerAndDestroy(5f);
            StartCoroutine(timerAndDestroyCoroutine);
        }
    }
    void OnObstacleSmash(Transform obstacleTransform)
    {
        if (obstacleTransform == transform)
        {
            Destroy(gameObject);
        }
    }
    IEnumerator TimerAndDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}
