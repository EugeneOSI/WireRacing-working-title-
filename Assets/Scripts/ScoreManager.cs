using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;

public class ScoreManager : MonoBehaviour
{
    [Header("Счет")]
    float mainScore;

    [Header("События")]
    public bool nearSand;
    public bool nearObstacle;
    public bool highSpeed;

    [Header("Корутины")]
    Coroutine nearSandInCoroutine;
    Coroutine nearSandOutCoroutine;
    Coroutine highSpeedOutCoroutine;
    Coroutine nearObstacleInCoroutine;

    [Header("UI")]
    [SerializeField] GameObject UICanvas;
    [SerializeField] GameObject bonusScoreprefab;
    [SerializeField] GameObject addedScorePrefab;

    [Header("Компоненты")]
    private Player player;
    private ScoreEventsHander scoreEventsHander;
    public TextMeshProUGUI scoreText;

    [Header("Линии")]
    public List<GameObject> lines;

    void Start()
    {

        nearSand = false;
        nearObstacle = false;
        lines = new List<GameObject>();
        mainScore = 0;
        player = GameObject.Find("Player").GetComponent<Player>();
        scoreEventsHander = GameObject.Find("Player").GetComponent<ScoreEventsHander>();
    }

    void Update()
    {
        UpdateMainScore();
        CheckEvents();     
    }

    void CheckEvents()
    {
        // Near Sand
        if (scoreEventsHander.NearSand && !nearSand && nearSandInCoroutine == null)
        {
            nearSandInCoroutine = StartCoroutine(NearSandTimerIn(0.3f));
        }
        if (!scoreEventsHander.NearSand && !nearSand && nearSandInCoroutine != null)
        {
            StopCoroutine(nearSandInCoroutine);
            nearSandInCoroutine = null;
        }
        if (!scoreEventsHander.NearSand && nearSand && nearSandOutCoroutine == null)
        {
            nearSandOutCoroutine = StartCoroutine(NearSandTimerOut(2f));
        }
        if (scoreEventsHander.NearSand && nearSandOutCoroutine != null)
        {
            StopCoroutine(nearSandOutCoroutine);
            nearSandOutCoroutine = null;
        }

        //Near Obstacle
        if (scoreEventsHander.NearObstacle && !nearObstacle && nearObstacleInCoroutine == null)
        {
            nearObstacleInCoroutine = StartCoroutine(ObstacleTimerIn(0.5f));
        }
        if (nearObstacleInCoroutine != null && player.hitObstacle)
        {
            StopCoroutine(nearObstacleInCoroutine);
            nearObstacleInCoroutine = null;
        }

        //High Speed
        if (scoreEventsHander.HighSpeed && !highSpeed)
        {
            highSpeed = true;
            CreateLine("speed");
        }
        if (!scoreEventsHander.HighSpeed && highSpeed && highSpeedOutCoroutine == null)
        {
            highSpeedOutCoroutine = StartCoroutine(HighSpeedTimerOut(2f));
        }
        if (scoreEventsHander.HighSpeed && highSpeedOutCoroutine != null)
        {
            StopCoroutine(highSpeedOutCoroutine);
            highSpeedOutCoroutine = null;
        }
        if (player.hitObstacle||!player.OnTrack){
            if (highSpeedOutCoroutine != null)
            {
                StopCoroutine(highSpeedOutCoroutine);
                highSpeedOutCoroutine = null;
            }
            highSpeed = false;
        }


    }

    void CreateLine(string name)
    {
        GameObject bonusScoreInstance = Instantiate(bonusScoreprefab);
        bonusScoreInstance.transform.position = player.transform.position;
        bonusScoreInstance.transform.SetParent(player.transform);
        bonusScoreInstance.GetComponent<BonusScoreHander>().animator.SetBool("event", true);
        switch (name)
        {
            case "sand":
                bonusScoreInstance.GetComponent<BonusScoreHander>().bonusType = BonusType.nearSand;
                break;
            case "obstacle":
                bonusScoreInstance.GetComponent<BonusScoreHander>().bonusType = BonusType.nearObstacle;
                break;
            case "speed":
                bonusScoreInstance.GetComponent<BonusScoreHander>().bonusType = BonusType.highSpeed;
                break;
        }

    }
    
    public void AddBonusToMainScore(float bonusScore)
    {
        mainScore += bonusScore;
    }
    void UpdateMainScore()
    {
        //mainScore += player.Velocity * Time.deltaTime;
        scoreText.text = "Score: " + (int)mainScore;
    }

    IEnumerator ObstacleTimerIn(float delay)
    {
        yield return new WaitForSeconds(delay);
            nearObstacle = true;
            CreateLine("obstacle");
            StartCoroutine(ObstacleTimerOut(0.8f));
    }
    IEnumerator ObstacleTimerOut(float delay)
    {
        yield return new WaitForSeconds(delay);
        nearObstacle = false;
        nearObstacleInCoroutine = null;
    }
    IEnumerator HighSpeedTimerOut(float delay)
    {
        yield return new WaitForSeconds(delay);
        highSpeed = false;
    }

    IEnumerator NearSandTimerOut(float delay)
    {
        yield return new WaitForSeconds(delay);
        nearSand = false;
    }
    
        IEnumerator NearSandTimerIn (float delay)
    {
        yield return new WaitForSeconds(delay);
        CreateLine("sand");
        nearSand = true;
    }
}

