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
    ScoreHander scoreHander;
    float score = 0;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        scoreHander = GameObject.Find("Player").GetComponent<ScoreHander>();
    }
    void Update()
    {
        switch (bonusType)
        {
            case BonusType.nearSand:
                description.text = "ON THE EDGE!";
                if (scoreHander.NearSand)
                {
                    score += player.Velocity * Time.deltaTime * 2;
                }
                break;
            case BonusType.nearObstacle:
                description.text = "CLOSE!";
                if (score <= 30)
                {
                    score += player.Velocity * Time.deltaTime * 2f;
                }
                break;
            case BonusType.highSpeed:
                description.text = "HIGH SPEED!";
                if (scoreHander.HighSpeed)
                {
                    score += player.Velocity * Time.deltaTime * 2;
                }
                break;
        }
        amount.text = "" + (int)score;
    }


}
