using UnityEngine;
using TMPro;
using System;
using System.Collections;

public enum BonusType {nearSand, nearObstacle, highSpeed};
public class BonusScoreUI : MonoBehaviour
{
    public BonusType bonusType;
    private bool _isClosing;

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
                    score += Mathf.MoveTowards(score, 30, Time.deltaTime * 0.5f);
                }
                break;
            case BonusType.highSpeed:
                description.text = "HIGH SPEED!";
                score += player.Velocity * Time.deltaTime;
                break;
        }
        amount.text = "" + (int)score;
    }

    public int TakeScoreAndReset()
{
    int s = (int)score;
    score = 0f;
    return s;
}

// мягкое закрытие: проиграть анимацию скрытия, а затем уничтожить
    public IEnumerator CloseAndDestroy(bool credited, float delay = 0.3f)
    {
        if (_isClosing || this == null) yield break;   // уже закрываемся или объект уничтожен
        _isClosing = true;

        // animator может быть уничтожён раньше
        if (animator != null)
            animator.SetBool("event", false);

        // подстрахуемся, если delay == 0
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        // после ожидания объект мог уже быть уничтожен где-то ещё
        if (this == null) yield break;

        var go = gameObject;       // кешируем, чтобы Unity не дергал MarshalledUnityObject лишний раз
        if (go != null)
            Destroy(go);
    }

}
