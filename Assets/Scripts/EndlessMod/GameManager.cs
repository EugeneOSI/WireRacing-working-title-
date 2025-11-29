using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PursuingEnemy pursuingEnemy;
    [SerializeField] private float difficultyIncreaseInterval = 10f;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private TextMeshProUGUI healthAlert;
    float time;
    const float difficultyK = 0.003f;

    bool gameStarted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthAlert.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.timeSinceLevelLoad;
        SetDifficulty();
        if (player.Health < 2){
            healthAlert.gameObject.SetActive(true);
        }
        else{healthAlert.gameObject.SetActive(false);}
        
        switch (player.isAlive)
        {
            case false:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }
    float GetDifficulty(float t, float start, float max, float k)
{
    return start + (max - start) * (1f - Mathf.Exp(-k * t));
}

void SetDifficulty(){
    spawnManager.obstacleCount = (int)GetDifficulty(time, 8, 12, difficultyK);
    pursuingEnemy.defaultSpeed = GetDifficulty(time, 13, 15, difficultyK);
    player.attractionForce = GetDifficulty(time, 25, 30, difficultyK);
}


public bool GameStarted{
    get {return gameStarted;}
    set {gameStarted = value;}}
}
