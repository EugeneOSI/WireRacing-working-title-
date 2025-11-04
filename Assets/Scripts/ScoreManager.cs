using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    float mainScore;
    float bonusScore = 0;
    float timerIn = 0.2f;
    float timerOut = 0.5f;

    private bool bonusScoreIsActive;

    public GameObject bonusScoreOffset;

    private Player player;
    private Rigidbody2D playerRb;
    private ScoreHander scoreHander;
    public TextMeshProUGUI scoreText;
    public GameObject bonusScoreObj;
    public Animator animator;

    void Start()
    {
        mainScore = 0;
        player = GameObject.Find("Player").GetComponent<Player>();
        scoreHander = GameObject.Find("Player").GetComponent<ScoreHander>();
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody2D>();
  

    }

    void Update()
    {
        mainScore += playerRb.linearVelocity.magnitude * Time.deltaTime;
        scoreText.text = "Score: " + (int)mainScore;
        CheckEvents();
    }

    void CheckEvents()
    {
        if (scoreHander.NearSand && player.OnTrack)
        {
            timerIn -= Time.deltaTime;
            if (timerIn <= 0)
            {
                bonusScoreIsActive = true;
                animator.SetBool("nearSand", true);
                bonusScore += playerRb.linearVelocity.magnitude * Time.deltaTime * 2;
                bonusScoreObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + (int)bonusScore;
                bonusScoreObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "ON THE EDGE!";
            }
            timerOut = 0.5f;
        }
        if (!scoreHander.NearSand && player.OnTrack)
        {
            timerOut -= Time.deltaTime;
            if (timerOut <= 0)
            {
                animator.SetBool("nearSand", false);
                animator.SetBool("mistake", false);
                bonusScoreIsActive = false;
                mainScore += bonusScore;
                bonusScore = 0;
            }
            timerIn = 0.2f;
        }
        if (scoreHander.NearSand && bonusScoreIsActive && !player.OnTrack)
        {
            animator.SetBool("mistake", true);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            bonusScoreOffset.transform.position = new Vector2 (bonusScoreObj.transform.position.x, bonusScoreObj.transform.position.y + 10);
        }
    }
}
