using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using Dan.Main;
using UnityEngine.UI;
public class GameManagerEM : MonoBehaviour
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

    [SerializeField] private LeaderboarManager leaderboarManager;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameOver = false;
        IsPaused = false;
        gameOverSequenceStarted = false;
        Time.timeScale = 1;
        uiController.ActivateUI(uiController.ScoreUI, true);
        uiController.ActivateUI(uiController.healthAlert, false);
        uiController.ActivateUI(uiController.gameOverMenu, false); 
    }

    // Update is called once per frame
    void Update()
    {
        time = Time.timeSinceLevelLoad;
        SetDifficulty();
        if (player.Health < 2){
            uiController.ActivateUI(uiController.healthAlert, true);
        }
        else{uiController.ActivateUI(uiController.healthAlert, false);}
        
        if (!player.isAlive && gameOverSequenceStarted == false)
        {
            GameOver = true;
            gameOverSequenceStarted = true;
            GameOverSequence();
        }

        if (Input.GetKeyDown(KeyCode.Escape)){
            PauseGame();
        }
    }

public void PauseGame(){
                if(!IsPaused){
                uiController.SwitchScreenActive(uiController.pauseMenu);
                IsPaused = true;
                Time.timeScale = 0;
            }
            else if(IsPaused&&uiController.activeScreens.Count < 2){
                uiController.DeactivateActiveScreen();
                IsPaused = false;
                Time.timeScale = 1;
            
            }
            else if (IsPaused&&uiController.activeScreens.Count >= 2){
                uiController.DeactivateActiveScreen();
            } }
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

            uiController.SwitchScreenActive(uiController.ScoreUI);
            uiController.SwitchScreenActive(uiController.gameOverMenu);
            float currentScore = scoreManager.mainScore;
            uiController.SetText(uiController.currentScore, currentScore.ToString());
            if (currentScore > PrefsManager.Instance.bestScore&&!PrefsManager.Instance.IsPrefsSetted("BestScore")){
                PrefsManager.Instance.SaveBestScore(currentScore); 
                leaderboarManager.LoadEntries();
            }
            else if (currentScore > PrefsManager.Instance.bestScore&&PrefsManager.Instance.IsPrefsSetted("BestScore")){
                PrefsManager.Instance.SaveBestScore(currentScore);
                leaderboarManager.UpdatePlayerEntry();
            }
            else{
                leaderboarManager.LoadEntries();
            }
            uiController.SetText(uiController.bestScore, PrefsManager.Instance.bestScore.ToString());
}

}


    


