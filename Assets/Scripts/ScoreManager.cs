using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

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

    [SerializeField] GameObject bonusScoreprefab;
    private Player player;
    private ScoreHander scoreHander;
    public TextMeshProUGUI scoreText;

    List<GameObject> lines;

    void Start()
    {
        mainScore = 0;
        player = GameObject.Find("Player").GetComponent<Player>();
        scoreHander = GameObject.Find("Player").GetComponent<ScoreHander>();
  

    }

    void Update()
    {
        UpdateLinesPositions();
        UpdateMainScore();
        CheckEvents();
    }

    void CheckEvents()
    {
        
        if (scoreHander.NearSand && player.OnTrack)
        {
            if (!bonusScoreIsActive)
            {
                bonusScoreIsActive = true;
                GameObject bonusScoreInstance = Instantiate(bonusScoreprefab, defaultLinePos, Quaternion.identity);
                bonusScoreInstance.transform.SetParent(player.transform);

            }
            if (bonusScoreIsActive)
            {

            }

        }
        if ((!scoreHander.NearSand && player.OnTrack) || !player.OnTrack)
        {
            bonusScoreIsActive = false;
        }


    }
    
    void UpdateMainScore()
    {
        mainScore += player.Velocity * Time.deltaTime;
        scoreText.text = "Score: " + (int)mainScore;
    }
    void UpdateLinesPositions()
    {
        defaultLinePos = player.transform.position;
        secondLinePos = player.transform.position + new Vector3 (0f, 1.38f,0f);
        thirdLinePos = player.transform.position + new Vector3 (0f, 2.76f,0f);;
        forthLinePos = player.transform.position + new Vector3 (0f, 4.14f,0f);;
        fivthLonePos = player.transform.position + new Vector3 (0f, 5.52f,0f);;
        
    }

}
