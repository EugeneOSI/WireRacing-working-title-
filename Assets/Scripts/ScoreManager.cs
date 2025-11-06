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
    float bonusScore = 0;
    float timerIn = 0.2f;
    float timerOut = 0.5f;

    private bool bonusScoreIsActive;

    [Header("Позиции линий")]
    Vector2 defaultLinePos;
    Vector2 secondLinePos;
    Vector2 thirdLinePos;
    Vector2 forthLinePos;
    Vector2 fivthLonePos;

    [Header("События")]
    bool nearSand;
    bool nearObstacle;
    bool highSpeed;
    GameObject nearSandInstance;
    GameObject nearObstacleInstance;
    GameObject highSpeedInstance;
    float eventsAmount;

    [SerializeField] GameObject bonusScoreprefab;
    private Player player;
    private ScoreHander scoreHander;
    public TextMeshProUGUI scoreText;

    List<GameObject> lines;

    void Start()
    { 
        nearSand = false;
        nearObstacle = false;
        lines = new List<GameObject>();
        mainScore = 0;
        player = GameObject.Find("Player").GetComponent<Player>();
        scoreHander = GameObject.Find("Player").GetComponent<ScoreHander>();
  

    }

    void Update()
    {
        //eventsAmount = 0;
        //UpdateLinesPositions();
        UpdateMainScore();
        CheckEvents();
        
        /*bool[] events = {
            nearSand = scoreHander.nearSand,
            nearObstacle = scoreHander.nearObstacle,
            highSpeed = scoreHander.HighSpeed };

        foreach (var ev in events)
        {
            if (ev == true) eventsAmount++;
        }
        Debug.Log(eventsAmount);*/
    }

    void CheckEvents()
    {

        if (scoreHander.NearSand&&!nearSand)
        {
            StartCoroutine(TimerAndCreateLine("sand", 0.2f));
            nearSand = true;

        }
        if (!scoreHander.NearSand && nearSand)
        {
            nearSandInstance.GetComponent<BonusScoreUI>().animator.SetBool("event", false);
            StartCoroutine(TimerAndRemoveLine("sand", 0.8f));
            nearSand = false;
        }
        if (scoreHander.NearObstacle && !nearObstacle)
        {
            StartCoroutine(TimerAndCreateLine("obstacle", 0.1f));
            nearObstacle = true;
        }
        if (!scoreHander.NearObstacle && nearObstacle)
        {
            nearObstacleInstance.GetComponent<BonusScoreUI>().animator.SetBool("event", false);
            StartCoroutine(TimerAndRemoveLine("obstacle", 0.8f));
            nearObstacle = false;
        }
        if (scoreHander.HighSpeed && !highSpeed)
        {
            CreateLine("speed");
            highSpeed = true;
        }
        if (!scoreHander.HighSpeed && highSpeed)
        {
            highSpeedInstance.GetComponent<BonusScoreUI>().animator.SetBool("event", false);
            StartCoroutine(TimerAndRemoveLine("speed", 0.8f));
            highSpeed = false;
        }


    }

    void CreateLine(string name)
    {
        GameObject bonusScoreInstance = Instantiate(bonusScoreprefab);

        if (lines.Count > 0)
        {
            foreach (var line in lines)
            {
                line.transform.position = line.transform.position + Vector3.up;
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
                line.transform.position = line.transform.position + Vector3.down;
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
