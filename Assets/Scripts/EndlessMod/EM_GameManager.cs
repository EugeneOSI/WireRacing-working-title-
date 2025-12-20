using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using Dan.Main;
using UnityEngine.UI;
using System;
public class EM_GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PursuingEnemy pursuingEnemy;
    [SerializeField] private EM_UIController uiController;
    [SerializeField] private ScoreManager scoreManager;
    
    public bool IsPaused {get; set;}
    public bool GameOver {get; private set;}
    bool gameOverSequenceStarted = false;
    float time;
    const float difficultyK = 0.003f;

    bool gameStarted;

    [SerializeField] private EMLeaderBoard emLeaderBoard;

    public static event Action OnGameStart;
    public static event Action OnLowHealth;
    public static event Action OnNormalHealth;
    public static event Action OnGameOver;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnGameOver += GameOverSequence;
        GameOver = false;
        IsPaused = false;
        gameOverSequenceStarted = false;
        Time.timeScale = 1;
        OnGameStart?.Invoke();
    }

    void OnDestroy(){
        OnGameOver -= GameOverSequence;
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.timeSinceLevelLoad;
        SetDifficulty();
        if (player.Health < 2){
            OnLowHealth?.Invoke();
        }
        else{
            OnNormalHealth?.Invoke();
        }
        
        if (!player.isAlive && gameOverSequenceStarted == false)
        {
            GameOver = true;
            gameOverSequenceStarted = true;
            OnGameOver?.Invoke();
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


private void GameOverSequence(){

            float currentScore = scoreManager.mainScore;
            if (currentScore > PrefsManager.Instance.GetBestScore()&&!PrefsManager.Instance.IsPrefsSetted("BestScoreUploaded")){
                PrefsManager.Instance.SaveBestScore(currentScore); 
                emLeaderBoard.LoadLeaderboard();
            }
            else if (currentScore > PrefsManager.Instance.bestScore&&PrefsManager.Instance.IsPrefsSetted("BestScoreUploaded")){
                PrefsManager.Instance.SaveBestScore(currentScore);
                emLeaderBoard.UpdateLeaderboard();
            }
            else{
                emLeaderBoard.LoadLeaderboard();
            }
}

    public void ExitToMainMenu(){
        GameManager.Instance.LoadScene("MainMenu");
    }
    public void ResumeGame(){
        GameManager.Instance.PauseGame();
    }
    public void RestartGame(){
        GameManager.Instance.LoadScene("EndlessMode");
    }

    public void DeletePlayerEntry(){
        PrefsManager.Instance.ResetEndlessModPrefs();
    }

}


    


