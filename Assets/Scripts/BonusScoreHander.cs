using UnityEngine;
using TMPro;
using System;
using System.Collections;

public enum BonusType {nearSand, nearObstacle, highSpeed};
public class BonusScoreUI : MonoBehaviour
{
    public BonusType bonusType;

    public Animator animator;
    [SerializeField] TextMeshProUGUI amount;
    [SerializeField] TextMeshProUGUI description;
    Player player;
    ScoreEventsHander scoreEventsHander;
    ScoreManager scoreManager;
    Coroutine outTimerCoroutine;
    float score = 0;
    float outTimer = 2f;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        scoreEventsHander = GameObject.Find("Player").GetComponent<ScoreEventsHander>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        scoreManager.lines.Add(gameObject);
        for (int i =0; i<scoreManager.lines.Count; i++)
        {
                scoreManager.lines[i].GetComponent<LineMover>().StartMove(i);
        }
        
        
    }
    void Update()
    {
        switch (bonusType)
        {
            case BonusType.nearSand:
                description.text = "ON THE EDGE!";
                if (scoreEventsHander.nearSand)
                {
                    score += player.Velocity * Time.deltaTime * 2;
                }
                if (!scoreManager.nearSand)
                {
                    animator.SetBool("event", false);
                    outTimerCoroutine =StartCoroutine(TimerAndRemoveLine(0.3f, "collect"));
                }
                if (!player.OnTrack||player.hitObstacle)
                {
                    if (outTimerCoroutine != null)
                    {
                        StopCoroutine(outTimerCoroutine);
                        outTimerCoroutine = null;
                    }
                    
                    animator.SetBool("mistake", true);
                    outTimerCoroutine = StartCoroutine(TimerAndRemoveLine(0.3f, ""));
                }
                break;


            case BonusType.nearObstacle:
                description.text = "CLOSE!";
                if (score <= 30)
                {
                    score += Time.deltaTime * 40f;
                }
                if (!scoreManager.nearObstacle)
                {
                    outTimer -= Time.deltaTime;
                    if (outTimer <= 0)
                    {
                        animator.SetBool("event", false);
                        StartCoroutine(TimerAndRemoveLine(0.3f, "collect"));
                    }
                }
                break;

            case BonusType.highSpeed:
                description.text = "HIGH SPEED!";
                if (scoreEventsHander.highSpeed)
                {
                    score += player.Velocity * Time.deltaTime * 2;
                }

                if (!scoreManager.highSpeed)
                {
                    animator.SetBool("event", false);
                    outTimerCoroutine = StartCoroutine(TimerAndRemoveLine(0.3f, "collect"));
                }
                if (!player.OnTrack||player.hitObstacle)
                {
                    if (outTimerCoroutine != null)
                    {
                        StopCoroutine(outTimerCoroutine);
                        outTimerCoroutine = null;
                    }
                    animator.SetBool("mistake", true);
                    outTimerCoroutine = StartCoroutine(TimerAndRemoveLine(0.3f, ""));
                }
                break;
        }
        amount.text = "" + (int)score;
    }
    
    IEnumerator TimerAndRemoveLine(float delay, string mode)
    {

        yield return new WaitForSeconds(delay);

        switch (mode)
        {
            case "collect":
                scoreManager.AddBonusToMainScore(score);
                break;
        }
        
        scoreManager.lines.Remove(gameObject);
        for (int i =0; i<scoreManager.lines.Count; i++)
        {
            scoreManager.lines[i].GetComponent<LineMover>().StartMove(i);
        }
        Destroy(gameObject);
    }


}
