using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

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
    public static event Action OnSceneLoading;

    [SerializeField] private Animator loadingScreen;
    [SerializeField] private GameObject loadingScreenObject;
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
        loadingScreen.SetTrigger("Off");
        loadingScreenObject.SetActive(true);
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
            Debug.Log("Игра поставлена на паузу");
            }
            else if(isGamePaused&&UIManager.Instance.ActiveScreens.Count < 2){
                OnUnpauseEvent?.Invoke();
                isGamePaused = false;Debug.Log("Игра поставлена на паузу");
            }
            else{
                whilePausedEvent?.Invoke();
                Debug.Log("Игра остается на паузе");
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
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    IEnumerator LoadSceneCoroutine(string sceneName){
        OnSceneLoading?.Invoke();
        loadingScreen.SetTrigger("In");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
        LoadSceneEvent?.Invoke();
        loadingScreen.SetTrigger("Out");
    }

}
