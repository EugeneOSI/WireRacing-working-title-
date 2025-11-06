using UnityEngine;
using TMPro;
using System;

public enum BonusType {nearSand, nearObstacle, highSpeed};
public class BonusScoreUI : MonoBehaviour
{
    public BonusType bonusType;

    public Animator animator;
    [SerializeField] TextMeshProUGUI amount;
    [SerializeField] TextMeshProUGUI description;
    Player player;
    float score = 0;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }
    void Update()
    {
        switch (bonusType)
        {
            case BonusType.nearSand:
                description.text = "ON THE EDGE!";
                score += player.Velocity * Time.deltaTime * 2;
                break;
            case BonusType.nearObstacle:
                description.text = "CLOSE!";
                if (score <= 30)
                {
                    score += Mathf.MoveTowards(score, 30, Time.deltaTime * 2);
                }
                break;
            case BonusType.highSpeed:
                description.text = "HIGH SPEED!";
                score += player.Velocity * Time.deltaTime;
                break;
        }
        amount.text = "" + (int)score;
    }

}
