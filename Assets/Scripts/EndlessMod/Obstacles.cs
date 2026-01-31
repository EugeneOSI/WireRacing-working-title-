using UnityEngine;
using System.Collections;

public class Obstacles : MonoBehaviour
{
    IEnumerator timerAndDestroyCoroutine;
    Animator animator;
    bool exploded = false;

    void Start()
    {
        animator = GetComponent<Animator>();
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
        if (!exploded)
        animator.SetTrigger("PowerUp Active");
    }
    void OnPowerUpEnd()
    {
        if (!exploded)
        animator.SetTrigger("PowerUp Unactive");
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
