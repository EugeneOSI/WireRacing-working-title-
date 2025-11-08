using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;

public class ScoreManager : MonoBehaviour
{
    float mainScore;

    public float lineSpeed;

    [Header("События")]
    bool nearSand;
    bool nearObstacle;
    bool highSpeed;
    GameObject nearSandInstance;
    GameObject nearObstacleInstance;
    GameObject highSpeedInstance;
    public float nearSandTimerInDefault = 0.2f;
    public float nearSandTimerOutDefault = 0.3f;
    float nearSandTimerIn;
    float nearSandTimerOut;

    public float hightSpeedTimerInDefault = 0.1f;
    public float hightSpeedTimerOutDefault = 0.5f;
    float hightSpeedTimerIn;
    float hightSpeedTimerOut;

    public float nearObstacleTimerInDefault = 0.05f;
    public float nearObstacleTimerOutDefault = 1f;
    float nearObstacleTimerIn;
    float nearObstacleTimerOut;
   

    [SerializeField] GameObject bonusScoreprefab;
    private Player player;
    private ScoreHander scoreHander;
    public TextMeshProUGUI scoreText;

    List<GameObject> lines;

    void Start()
    {

        nearSandTimerIn = nearSandTimerInDefault;
        nearSandTimerOut = nearSandTimerOutDefault;

        nearObstacleTimerIn = nearObstacleTimerInDefault;
        nearObstacleTimerOut = nearObstacleTimerOutDefault;

        hightSpeedTimerIn = hightSpeedTimerInDefault;
        hightSpeedTimerOut = hightSpeedTimerOutDefault;

        nearSand = false;
        nearObstacle = false;
        lines = new List<GameObject>();
        mainScore = 0;
        player = GameObject.Find("Player").GetComponent<Player>();
        scoreHander = GameObject.Find("Player").GetComponent<ScoreHander>();
  

    }

    void Update()
    {
        UpdateMainScore();
        CheckEvents();     
    }

    void CheckEvents()
    {

        if (scoreHander.NearSand && !nearSand)
        {
            nearSandTimerIn -= Time.deltaTime;
            if (nearSandTimerIn <= 0)
            {
                CreateLine("sand");
                nearSand = true;
                nearSandTimerOut = nearSandTimerOutDefault;
            }

        }
        if (!scoreHander.NearSand && !nearSand)
        {
            nearSandTimerIn = nearSandTimerInDefault;
        }
        if (!scoreHander.NearSand && nearSand)
        {
            nearSandTimerOut -= Time.deltaTime;
            if (nearSandTimerOut <= 0)
            {
                nearSandInstance.GetComponent<BonusScoreUI>().animator.SetBool("event", false);
                StartCoroutine(TimerAndRemoveLine("sand", 0.3f));
                nearSandTimerIn = nearSandTimerInDefault;
                nearSand = false;
            }
        }
        if (scoreHander.NearObstacle && !nearObstacle)
        {
            nearObstacleTimerIn -= Time.deltaTime;
            if (nearObstacleTimerIn <= 0)
            {
                CreateLine("obstacle");
                nearObstacle = true;
                nearObstacleTimerOut = nearObstacleTimerOutDefault;
            }
        }
        if (!scoreHander.NearObstacle && !nearObstacle)
        {
            nearObstacleTimerIn = nearObstacleTimerInDefault;
        }
        if (!scoreHander.NearObstacle && nearObstacle)
        {
            nearObstacleTimerOut -= Time.deltaTime;
            if (nearObstacleTimerOut <= 0)
            {
                nearObstacle = false;
                nearObstacleInstance.GetComponent<BonusScoreUI>().animator.SetBool("event", false);
                StartCoroutine(TimerAndRemoveLine("obstacle", 0.3f));
                nearObstacleTimerIn = nearObstacleTimerInDefault;
            }
        }
        if (scoreHander.HighSpeed && !highSpeed)
        {
            hightSpeedTimerIn -= Time.deltaTime;
            if (hightSpeedTimerIn <= 0)
            {
                CreateLine("speed");
                highSpeed = true;
                hightSpeedTimerOut = hightSpeedTimerOutDefault;
            }
        }
        if (!scoreHander.HighSpeed && !highSpeed)
        {
            hightSpeedTimerIn = hightSpeedTimerInDefault;
        }
        if (!scoreHander.HighSpeed && highSpeed)
        {
            hightSpeedTimerOut -= Time.deltaTime;
            if (hightSpeedTimerOut <= 0)
            {
                highSpeedInstance.GetComponent<BonusScoreUI>().animator.SetBool("event", false);
                StartCoroutine(TimerAndRemoveLine("speed", 0.3f));
                highSpeed = false;
                hightSpeedTimerIn = hightSpeedTimerInDefault;
            }
        }


    }

    void CreateLine(string name)
    {
        GameObject bonusScoreInstance = Instantiate(bonusScoreprefab);

        if (lines.Count > 0)
        {
            foreach (var line in lines)
            {
                line.GetComponent<LineMover>().StartMove("up");
            }
        }
        bonusScoreInstance.transform.position = player.transform.position;
        bonusScoreInstance.transform.SetParent(player.transform);
        bonusScoreInstance.GetComponent<BonusScoreUI>().animator.SetBool("event", true);
        switch (name)
        {
            case "sand":
                bonusScoreInstance.GetComponent<BonusScoreUI>().bonusType = BonusType.nearSand;
                nearSandInstance = bonusScoreInstance;
                lines.Add(nearSandInstance);
                break;
            case "obstacle":
                bonusScoreInstance.GetComponent<BonusScoreUI>().bonusType = BonusType.nearObstacle;
                nearObstacleInstance = bonusScoreInstance;
                lines.Add(nearObstacleInstance);
                break;
            case "speed":
            bonusScoreInstance.GetComponent<BonusScoreUI>().bonusType = BonusType.highSpeed;
                highSpeedInstance = bonusScoreInstance;
                lines.Add(highSpeedInstance);
                break;

        }

    }
    
    void RemoveLine(string name)
    {
        switch (name)
        {
            case "sand":
                lines.Remove(nearSandInstance);
                Destroy(nearSandInstance.gameObject);
                break;
            case "obstacle":
                lines.Remove(nearObstacleInstance);
                Destroy(nearObstacleInstance);
                break;
            case "speed":
                lines.Remove(highSpeedInstance);
                Destroy(highSpeedInstance);
                break;
        }

        if (lines.Count > 0)
        {
            foreach (var line in lines)
            {
                line.GetComponent<LineMover>().StartMove("down");
            }
            
        }
    }
    void UpdateMainScore()
    {
        mainScore += player.Velocity * Time.deltaTime;
        scoreText.text = "Score: " + (int)mainScore;
    }

    IEnumerator TimerAndRemoveLine(string lineName, float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveLine(lineName);
    }
    IEnumerator TimerAndCreateLine(string lineName, float delay)
    {
        yield return new WaitForSeconds(delay);
        CreateLine(lineName);
    }
}

