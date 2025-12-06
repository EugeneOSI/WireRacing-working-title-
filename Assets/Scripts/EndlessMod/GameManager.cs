using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using Dan.Main;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PursuingEnemy pursuingEnemy;
    [SerializeField] private UICOntroller uiController;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private PrefsManager prefsManager;
    
    public bool IsPaused {get; set;}
    public bool GameOver {get; private set;}
    bool gameOverSequenceStarted = false;
    float time;
    const float difficultyK = 0.003f;

    bool gameStarted;

    [SerializeField] private LeaderboarManager leaderboarManager;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        leaderboarManager.EntriesLoading = false;
        GameOver = false;
        IsPaused = false;
        gameOverSequenceStarted = false;
        Time.timeScale = 1;
        uiController.ActivateUI("ScoreUI", true);
        uiController.ActivateUI("healthAlert", false);
        uiController.ActivateUI("gameOverMenu", false); 
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.timeSinceLevelLoad;
        SetDifficulty();
        if (player.Health < 2){
            uiController.ActivateUI("healthAlert", true);
        }
        else{uiController.ActivateUI("healthAlert", false);}
        
        if (!player.isAlive && gameOverSequenceStarted == false)
        {
            GameOver = true;
            gameOverSequenceStarted = true;
            GameOverSequence();
        }

        if (Input.GetKeyDown(KeyCode.Escape)){
            IsPaused = !IsPaused;
            }
        if (IsPaused){
            Time.timeScale = 0;
            uiController.ActivateUI("pauseMenu", true);
        }
        else{
            Time.timeScale = 1;
            uiController.ActivateUI("pauseMenu", false);
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

private void GameOverSequence(){

            uiController.ActivateUI("ScoreUI", false);
            uiController.ActivateUI("gameOverMenu", true);
            float currentScore = scoreManager.mainScore;
            uiController.currentScore.text = currentScore.ToString();
            if (currentScore > prefsManager.bestScore){
                prefsManager.SaveBestScore(currentScore);
                uiController.bestScore.text = prefsManager.bestScore.ToString();
            }
            else{
                uiController.bestScore.text = prefsManager.bestScore.ToString();
            }
            if (prefsManager.playerEntryUploaded == 0){
                uiController.ActivateUI("UnderBoardInformation", false);
                leaderboarManager.LoadEntries();
            }
            else{
                if (currentScore > prefsManager.bestScore){
                leaderboarManager.UpdatePlayerEntry();}
                leaderboarManager.LoadEntries();
                uiController.ActivateUI("InputField", false);
                uiController.playerName.text = prefsManager.playerName;
                uiController.playerPosition.text = "#" + leaderboarManager.playerPosition;
            }
}

}


    


