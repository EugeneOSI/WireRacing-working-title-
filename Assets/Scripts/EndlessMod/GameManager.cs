using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PursuingEnemy pursuingEnemy;
    [SerializeField] private TextMeshProUGUI healthAlert;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    public bool IsPaused {get; set;}
    public bool GameOver {get; private set;}
    float time;
    const float difficultyK = 0.003f;

    bool gameStarted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameOver = false;
        Time.timeScale = 1;
        healthAlert.gameObject.SetActive(false);
        gameOverMenu.SetActive(false);
        IsPaused = false;
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
                gameOverMenu.SetActive(true);
                GameOver = true;
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape)){
            IsPaused = !IsPaused;
            }
        if (IsPaused){
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else{
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
            
    }
    float GetDifficulty(float t, float start, float max, float k)
{
    return start + (max - start) * (1f - Mathf.Exp(-k * t));
}

void SetDifficulty(){
    spawnManager.obstacleCount = (int)GetDifficulty(time, 8, 12, difficultyK);
    pursuingEnemy.defaultSpeed = GetDifficulty(time, 15, 18, difficultyK);
    player.attractionForce = GetDifficulty(time, 25, 30, difficultyK);
}


public bool GameStarted{
    get {return gameStarted;}
    set {gameStarted = value;}}

public void RestartGame(){
SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
}


