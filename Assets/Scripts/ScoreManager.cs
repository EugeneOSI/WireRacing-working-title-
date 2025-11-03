using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    float mainScore;
    float bonusScore = 0;

    private Player player;
    private ScoreHander scoreHander;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bonusScoreText;
    public TextMeshProUGUI bonusScoreDescText;

    void Start()
    {
        mainScore = 0;
        scoreHander = GameObject.Find("Player").GetComponent<ScoreHander>();
        player = GameObject.Find("Player").GetComponent<Player>();
  

    }

    void Update()
    {
        mainScore = scoreHander.Distance;
        scoreText.text = "Score: " + (int)mainScore;
    }

    void CheckEvents()
    {
        if (scoreHander.NearSand)
        {
            bonusScore += Time.deltaTime;
            bonusScoreText.gameObject.SetActive(true);
            bonusScoreDescText.gameObject.SetActive(true);

            bonusScoreText.text = "" + (int)bonusScore;
            bonusScoreDescText.text = "NEAR THE EDGE!";
        }
        if (!scoreHander.NearSand)
        {
            bonusScoreText.gameObject.SetActive(false);
            bonusScoreDescText.gameObject.SetActive(false);
        }
    }
}
