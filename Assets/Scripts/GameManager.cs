using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public enum GameState {MainMenu, EndlessMode, TimeTrial}
public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance {get; private set;}
    public static event Action LoadSceneEvent;
    public GameState currentGameState {get; private set;}
    public bool isGamePaused {get; private set;}
    public static event Action OnPauseEvent;
    public static event Action OnUnpauseEvent;
    public static event Action whilePausedEvent;
    void Awake()
    {
        currentGameState = GameState.MainMenu;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)){
            Debug.Log(currentGameState);

        }
        if (isGamePaused){
            Time.timeScale = 0;
        }
        else{
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.Escape)){
            if (currentGameState != GameState.MainMenu){
            PauseGame();}
            else {
                
                whilePausedEvent?.Invoke();
            }
        }
    }
public void PauseGame(){

        if (!isGamePaused){
            OnPauseEvent?.Invoke();
            isGamePaused = true;
            }
            else if(isGamePaused&&UIManager.Instance.ActiveScreens.Count < 2){
                OnUnpauseEvent?.Invoke();
                isGamePaused = false;
            }
            else{
                whilePausedEvent?.Invoke();
            }
            
}
    public void LoadScene(string sceneName){
        isGamePaused = false;
        switch(sceneName){
            case "MainMenu":
            currentGameState = GameState.MainMenu;
            break;
            case "EndlessMode":
            currentGameState = GameState.EndlessMode;
            break;
            default:
            currentGameState = GameState.TimeTrial;
            break;
        }
        SceneManager.LoadScene(sceneName);
        LoadSceneEvent?.Invoke();
    }

}
