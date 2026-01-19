using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
public class EM_UIController : MonoBehaviour
{
    public List<GameObject> activeScreens = new List<GameObject>();

    [SerializeField] private EM_GameManager gameManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private EMLeaderBoard emLeaderBoard;
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public GameObject optionsMenu;
    [SerializeField] public GameObject loadingPanel;
    [SerializeField] public GameObject submitScoreButton;
    [SerializeField] public GameObject inputField;
    [SerializeField] public GameObject playerName;
    [SerializeField] public GameObject emLeaderBoardPlace;
    [SerializeField] public GameObject fieldAlert;
    [SerializeField] public GameObject gameOverMenu;
    [SerializeField] public GameObject ScoreUI;
    [SerializeField] public GameObject scoreText;
    [SerializeField] public GameObject healthAlert;
    [SerializeField] public GameObject currentScore;
    [SerializeField] public GameObject bestScore;
    [SerializeField] public GameObject deleteEntryText;
    [SerializeField] public GameObject deleteEntryButton;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EM_GameManager.OnGameStart += OnGameStart;
        EM_GameManager.OnLowHealth += OnLowHealth;
        EM_GameManager.OnNormalHealth += OnNormalHealth;
        GameManager.OnPauseEvent += OnPauseEvent;
    LeaderBoardsManager.EntriesLoading += OnEntriesLoading;
    LeaderBoardsManager.EntriesLoading += CancelUpdateName;
    LeaderBoardsManager.EnteryUploading += OnEntryUploading;
    EMLeaderBoard.EndlessModEntriesLoaded += OnEndlessModEntriesLoaded;
    EMLeaderBoard.EmptyFieldAlert += EmptyFieldAlert;
    LeaderBoardsManager.OnLeaderboardError += OnLeaderboardError;
    LeaderBoardsManager.EndlessModEntryDeleted += OnEndlessModEntryDeleted;
    EM_GameManager.OnGameOver += OnGameOver;
    LeaderBoardsManager.EntriesLoading += OnEntriesLoading;
    }
    void OnDestroy(){
        EM_GameManager.OnGameStart -= OnGameStart;
        EM_GameManager.OnLowHealth -= OnLowHealth;
        EM_GameManager.OnNormalHealth -= OnNormalHealth;
        GameManager.OnPauseEvent -= OnPauseEvent;
        LeaderBoardsManager.EntriesLoading -= OnEntriesLoading;
        LeaderBoardsManager.EntriesLoading -= CancelUpdateName;
        LeaderBoardsManager.EnteryUploading -= OnEntryUploading;
        EMLeaderBoard.EndlessModEntriesLoaded -= OnEndlessModEntriesLoaded;
        EMLeaderBoard.EmptyFieldAlert -= EmptyFieldAlert;
        LeaderBoardsManager.OnLeaderboardError -= OnLeaderboardError;
        LeaderBoardsManager.EndlessModEntryDeleted -= OnEndlessModEntryDeleted;
        EM_GameManager.OnGameOver -= OnGameOver;
        LeaderBoardsManager.EntriesLoading -= OnEntriesLoading;

    }
    // Update is called once per frame
    void Update()
    {

    }

    public void OnGameStart(){
        UIManager.Instance.SetVisibilty(ScoreUI, true);
        UIManager.Instance.SetVisibilty(healthAlert, false);
        UIManager.Instance.SetVisibilty(gameOverMenu, false);
        }
    
    public void OnLowHealth(){
        UIManager.Instance.SetVisibilty(healthAlert, true);
    }
    public void OnNormalHealth(){
        UIManager.Instance.SetVisibilty(healthAlert, false);
    }
    public void OnGameOver(){
        UIManager.Instance.SetVisibilty(ScoreUI, false);
        UIManager.Instance.SetVisibilty(healthAlert, false);
        UIManager.Instance.SetVisibilty(gameOverMenu, true);
        UIManager.Instance.SetText(currentScore, scoreManager.mainScore.ToString());
    }

    private void OnEntriesLoading(){
    UIManager.Instance.SetVisibilty(loadingPanel, true);
    UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
}

    private void OnEntryUploading(){
    UIManager.Instance.SetVisibilty(loadingPanel, true);
    UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
}

private void OnEndlessModEntriesLoaded(){
    UIManager.Instance.SetVisibilty(loadingPanel, false);
    UIManager.Instance.SetText(bestScore, PrefsManager.Instance.GetBestScore().ToString());

    if (!PrefsManager.Instance.IsPrefsSetted("BestScoreUploaded") && !PrefsManager.Instance.IsPrefsSetted("PlayerName")){
        UIManager.Instance.SetVisibilty(inputField, true);
        UIManager.Instance.SetVisibilty(submitScoreButton, true);
        if (!PrefsManager.Instance.IsPrefsSetted("BestScore")||PrefsManager.Instance.GetBestScore() <= 0){
        UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
        }
       else{
        UIManager.Instance.SetButtonInteractable(submitScoreButton, true);
       }

        UIManager.Instance.SetVisibilty(playerName, false);
        UIManager.Instance.SetVisibilty(emLeaderBoardPlace, false);

        Debug.Log("No player name found and best score not uploaded");
    }
    if (!PrefsManager.Instance.IsPrefsSetted("BestScoreUploaded") && PrefsManager.Instance.IsPrefsSetted("PlayerName")){
        UIManager.Instance.SetVisibilty(submitScoreButton, true);
        UIManager.Instance.SetVisibilty(inputField, false);
        UIManager.Instance.SetButtonInteractable(submitScoreButton, true);
        if (!PrefsManager.Instance.IsPrefsSetted("BestScore")){
        UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
        }
        else{
        UIManager.Instance.SetButtonInteractable(submitScoreButton, true);
        }
        UIManager.Instance.SetVisibilty(playerName, true);
        UIManager.Instance.SetText(playerName, "Your name: "+PrefsManager.Instance.GetPlayerName());

        UIManager.Instance.SetVisibilty(emLeaderBoardPlace, false);
        Debug.Log("Player name found and best score not uploaded");
    }
    if (PrefsManager.Instance.IsPrefsSetted("BestScoreUploaded") && PrefsManager.Instance.IsPrefsSetted("PlayerName")){
        UIManager.Instance.SetVisibilty(playerName, true);
        UIManager.Instance.SetText(playerName, "Your name: " + PrefsManager.Instance.GetPlayerName());

        UIManager.Instance.SetVisibilty(emLeaderBoardPlace, true);
        UIManager.Instance.SetText(emLeaderBoardPlace, "Your position: "+emLeaderBoard.playerPosition.ToString());

        UIManager.Instance.SetVisibilty(inputField, false);
        UIManager.Instance.SetVisibilty(submitScoreButton, false);
        Debug.Log("Player name found and best monza time uploaded");
    }
    }

    public void OnEndlessModEntryDeleted(){
    UIManager.Instance.StartCoroutine(UIManager.Instance.SwitchVisibiltyForSeconds(deleteEntryText, 2));
    UIManager.Instance.SwitchButtonInteractable(deleteEntryButton);
    UIManager.Instance.SetVisibilty(playerName, false);
    UIManager.Instance.SetVisibilty(emLeaderBoardPlace, false);
    UIManager.Instance.SetVisibilty(inputField, false);
    UIManager.Instance.SetVisibilty(submitScoreButton, false);
}

    public void EmptyFieldAlert(){
    UIManager.Instance.StartCoroutine(UIManager.Instance.SwitchVisibiltyForSeconds(fieldAlert, 2));
    UIManager.Instance.SetText(fieldAlert, "Field is empty");
}


    public void OnPauseEvent(){
        UIManager.Instance.SwitchVisibilty(pauseMenu);
    }
    public void OpenOptionsMenu(){
        UIManager.Instance.SwitchVisibilty(optionsMenu);
    }

    public void OnLeaderboardError(string error){
    UIManager.Instance.StartCoroutine(UIManager.Instance.SwitchVisibiltyForSeconds(fieldAlert, 2));
    switch(error){
        case "409":
            UIManager.Instance.SetText(fieldAlert, "Username already exists");
            break;
        case "403":
            UIManager.Instance.SetText(fieldAlert, "Forbidden name");
            break;
        case "0":
            UIManager.Instance.SetText(fieldAlert, "Failed to connect");
            break;
        case "503":
            UIManager.Instance.SetText(fieldAlert, "Service unavailable");
            break;
        case "500":
            UIManager.Instance.SetText(fieldAlert, "Internal server error");
            break;
        default:
            break;
    }
    UIManager.Instance.SetButtonInteractable(submitScoreButton, true);}

public void ShowEditNamePanel(){
    UIManager.Instance.SetVisibilty(playerName, false);
    UIManager.Instance.SetVisibilty(emLeaderBoardPlace, false);
    UIManager.Instance.SetVisibilty(submitScoreButton, false);
    
    UIManager.Instance.SetVisibilty(inputField, true);


}
public void CancelUpdateName(){
    if (PrefsManager.Instance.IsPrefsSetted("PlayerName")){
    UIManager.Instance.SetVisibilty(playerName, true);
    
    UIManager.Instance.SetVisibilty(inputField, false);

    if (PrefsManager.Instance.IsPrefsSetted("BestScoreUploaded")){
        UIManager.Instance.SetVisibilty(emLeaderBoardPlace, true);
        UIManager.Instance.SetVisibilty(submitScoreButton, false);
    }
    if (!PrefsManager.Instance.IsPrefsSetted("BestScoreUploaded")){
        UIManager.Instance.SetVisibilty(emLeaderBoardPlace, false);
        UIManager.Instance.SetVisibilty(submitScoreButton, true);
    }}
}
}

