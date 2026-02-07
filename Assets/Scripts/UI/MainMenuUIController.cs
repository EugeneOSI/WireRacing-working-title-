using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
public class MainMenuUIController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private MonzaLeaderBoard monzaLeaderBoard;
    
    
    public List<GameObject> activeScreens = new List<GameObject>();
    
    [SerializeField] public GameObject monzaLeaderBoardPlace;
    
    
    [SerializeField] public GameObject optionsMenu;
    [SerializeField] public GameObject creditsScreen;
    [SerializeField] private GameObject gameModeMenu;
    [SerializeField] private GameObject timeTrialMenu;

    

    [SerializeField] public GameObject loadingPanel;
    [SerializeField] public GameObject submitScoreButton;
    [SerializeField] public GameObject inputField;
    [SerializeField] public GameObject fieldAlert;
    [SerializeField] public GameObject playerName;
    [SerializeField] public GameObject bestMonzaTime;
    [SerializeField] public GameObject monzalaps;
    [SerializeField] public GameObject deleteEntryButton;
    [SerializeField] public GameObject monzaStartButton;
    [SerializeField] public GameObject monzaInformationPanel;
    [SerializeField] public GameObject serviceUnavailablePanel;



void Awake(){
    
    LeaderBoardsManager.EntriesLoading += OnEntriesLoading;
    LeaderBoardsManager.EnteryUploading += OnEntryUploading;
    MonzaLeaderBoard.MonzaEntriesLoaded += OnMonzaEntriesLoaded;
    MonzaLeaderBoard.InpudFieldAlert += InpudFieldAlert;
    LeaderBoardsManager.MonzaEntryDeleted += OnMonzaEntryDeleted;
    LeaderBoardsManager.OnLeaderboardError += OnLeaderboardError;
    LeaderBoardsManager.EntriesLoading += OnEntriesLoading;
}

void Start(){
    UIManager.Instance.SetVisibilty(serviceUnavailablePanel, false);
}


void OnDestroy(){
    LeaderBoardsManager.EntriesLoading -= OnEntriesLoading;
    LeaderBoardsManager.EntriesLoading -= OnEntriesLoading;
    LeaderBoardsManager.EnteryUploading -= OnEntryUploading;
    MonzaLeaderBoard.MonzaEntriesLoaded -= OnMonzaEntriesLoaded;
    MonzaLeaderBoard.InpudFieldAlert -= InpudFieldAlert;
    LeaderBoardsManager.MonzaEntryDeleted -= OnMonzaEntryDeleted;
    LeaderBoardsManager.OnLeaderboardError -= OnLeaderboardError;
}
private void OnEntriesLoading(){
    UIManager.Instance.SetVisibilty(loadingPanel, true);
    UIManager.Instance.SetVisibilty(serviceUnavailablePanel, false);
    UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
    //UIManager.Instance.SetButtonInteractable(monzaStartButton, false);
}
private void OnEntryUploading(){
    UIManager.Instance.SetVisibilty(loadingPanel, true);
    UIManager.Instance.SetVisibilty(serviceUnavailablePanel, false);
    UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
    UIManager.Instance.SetButtonInteractable(monzaStartButton, false);
}
private void OnMonzaEntriesLoaded(){
    Debug.Log("Monza entries loaded");
    UIManager.Instance.SetButtonInteractable(monzaStartButton, true);
    UIManager.Instance.SetText(bestMonzaTime, UIManager.Instance.FormatTime(PrefsManager.Instance.GetBestTime("Monza"), "lap"));
    UIManager.Instance.SetText(monzalaps, PrefsManager.Instance.GetLapsAmount("Monza").ToString());
    UIManager.Instance.SetVisibilty(loadingPanel, false);

    if (!PrefsManager.Instance.IsPrefsSetted("MonzaTimeUploaded") && !PrefsManager.Instance.IsPrefsSetted("PlayerName")){
        UIManager.Instance.SetVisibilty(inputField, true);
        UIManager.Instance.SetVisibilty(submitScoreButton, true);
        if (!PrefsManager.Instance.IsPrefsSetted("BestMonzaTime")){
        UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
        }
       else{
        UIManager.Instance.SetButtonInteractable(submitScoreButton, true);
       }

        UIManager.Instance.SetVisibilty(playerName, false);
        UIManager.Instance.SetVisibilty(monzaLeaderBoardPlace, false);

        Debug.Log("No player name found and best monza time not uploaded");
    }
    if (!PrefsManager.Instance.IsPrefsSetted("MonzaTimeUploaded") && PrefsManager.Instance.IsPrefsSetted("PlayerName")){
        UIManager.Instance.SetVisibilty(submitScoreButton, true);
        UIManager.Instance.SetButtonInteractable(submitScoreButton, true);
        if (!PrefsManager.Instance.IsPrefsSetted("BestMonzaTime")){
        UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
        }
        else{
        UIManager.Instance.SetButtonInteractable(submitScoreButton, true);
        }
        UIManager.Instance.SetVisibilty(inputField, false);
        UIManager.Instance.SetVisibilty(playerName, true);
        UIManager.Instance.SetText(playerName, "Your name: "+PrefsManager.Instance.GetPlayerName());

        UIManager.Instance.SetVisibilty(monzaLeaderBoardPlace, false);
        Debug.Log("Player name found and best monza time not uploaded");
    }
    if (PrefsManager.Instance.IsPrefsSetted("MonzaTimeUploaded") && PrefsManager.Instance.IsPrefsSetted("PlayerName")){
        UIManager.Instance.SetVisibilty(playerName, true);
        UIManager.Instance.SetText(playerName, "Your name: " + PrefsManager.Instance.GetPlayerName());

        UIManager.Instance.SetVisibilty(monzaLeaderBoardPlace, true);
        UIManager.Instance.SetText(monzaLeaderBoardPlace, "Your position: "+monzaLeaderBoard.playerPosition.ToString());

        UIManager.Instance.SetVisibilty(inputField, false);
        UIManager.Instance.SetVisibilty(submitScoreButton, false);
        Debug.Log("Player name found and best monza time uploaded");
    }
}
public void InpudFieldAlert(string alert){
    UIManager.Instance.StartCoroutine(UIManager.Instance.SwitchVisibiltyForSeconds(fieldAlert, 2));
    UIManager.Instance.SetText(fieldAlert, alert);
}

public void OnMonzaEntryDeleted(){
    UIManager.Instance.SwitchButtonInteractable(deleteEntryButton);
    UIManager.Instance.SetVisibilty(playerName, false);
    UIManager.Instance.SetVisibilty(monzaLeaderBoardPlace, false);
    UIManager.Instance.SetVisibilty(inputField, false);
    UIManager.Instance.SetVisibilty(submitScoreButton, false);
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
            UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
            break;
        case "503":
            UIManager.Instance.SetText(fieldAlert, "Service unavailable");
            UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
            break;
        case "500":
            UIManager.Instance.SetText(fieldAlert, "Internal server error");
            UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
            break;
        default:
            UIManager.Instance.SetVisibilty(serviceUnavailablePanel, true);
            UIManager.Instance.SetText(fieldAlert, "Service unavailable");
            UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
            break;

    }
    UIManager.Instance.SetButtonInteractable(submitScoreButton, true);
    UIManager.Instance.SetVisibilty(loadingPanel, false);
    }


public void OpenGameModeMenu(){
    UIManager.Instance.SwitchVisibilty(gameModeMenu);
}
public void OpenTimeTrialMenu(){
    UIManager.Instance.SwitchVisibilty(timeTrialMenu);
}
public void OpenOptionsMenu(){
    UIManager.Instance.SwitchVisibilty(optionsMenu);
}
public void OpenMonzaInformation(){
    UIManager.Instance.SwitchVisibilty(monzaInformationPanel);
}
public void OpenCreditsScreen(){
    UIManager.Instance.SwitchVisibilty(creditsScreen);
}
public void StartEndlessMode(){
    GameManager.Instance.LoadScene("EndlessMode");
}
public void StartTimeTrial(string trackSceneName){
    GameManager.Instance.LoadScene(trackSceneName);
}
public void ExitGame(){
    Application.Quit();
}
}
