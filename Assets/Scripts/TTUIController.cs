using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class TTUIController : MonoBehaviour
{
    [SerializeField] private MonzaLeaderBoard monzaLeaderBoard;
    [SerializeField] private TimeTrialManager timeTrialManager;
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public GameObject optionsMenu;
    [SerializeField] public GameObject loadingPanel;
    [SerializeField] public GameObject submitScoreButton;
    [SerializeField] public GameObject inputField;
    [SerializeField] public GameObject playerName;
    [SerializeField] public GameObject monzaLeaderBoardPlace;
    [SerializeField] public GameObject fieldAlert;
    [SerializeField] public GameObject BestTimeAlert;
    [SerializeField] public GameObject InvalidLapAlert;
    [SerializeField] public GameObject WrongDirectionAlert;

    public List<GameObject> ActiveScreens = new List<GameObject>();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    GameManager.OnPauseEvent += OnPauseEvent;
    LeaderBoardsManager.EntriesLoading += OnEntriesLoading;
    LeaderBoardsManager.EntriesLoading += CancelUpdateName;
    LeaderBoardsManager.EnteryUploading += OnEntryUploading;
    MonzaLeaderBoard.MonzaEntriesLoaded += OnMonzaEntriesLoaded;
    MonzaLeaderBoard.EmptyFieldAlert += EmptyFieldAlert;
    LeaderBoardsManager.OnLeaderboardError += OnLeaderboardError;
    LeaderBoardsManager.EntriesLoading += OnEntriesLoading;
    TimeTrialManager.BestTimeUpdated += OnBestTimeUpdated;
    TimeTrialManager.InvalidLap += OnInvalidLap;
    TimeTrialManager.NewLapStarted += OnNewLapStarted;
    SplineDirectionTracker.OnWrongDirection += OnWrongDirection;
    SplineDirectionTracker.OnCorrectDirection += OnCorrectDirection;
    }
    void OnDestroy()
    {
    GameManager.OnPauseEvent -= OnPauseEvent;
    LeaderBoardsManager.EntriesLoading -= OnEntriesLoading;
    LeaderBoardsManager.EntriesLoading -= CancelUpdateName;
    LeaderBoardsManager.EnteryUploading -= OnEntryUploading;
    MonzaLeaderBoard.MonzaEntriesLoaded -= OnMonzaEntriesLoaded;
    MonzaLeaderBoard.EmptyFieldAlert -= EmptyFieldAlert;
    LeaderBoardsManager.OnLeaderboardError -= OnLeaderboardError;
    LeaderBoardsManager.EntriesLoading -= OnEntriesLoading;
    TimeTrialManager.BestTimeUpdated -= OnBestTimeUpdated;
    TimeTrialManager.InvalidLap -= OnInvalidLap;
    TimeTrialManager.NewLapStarted -= OnNewLapStarted;
    SplineDirectionTracker.OnWrongDirection -= OnWrongDirection;
    SplineDirectionTracker.OnCorrectDirection -= OnCorrectDirection;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEntriesLoading(){
    UIManager.Instance.SetVisibilty(loadingPanel, true);
    UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
}
    private void OnEntryUploading(){
    UIManager.Instance.SetVisibilty(loadingPanel, true);
    UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
}
private void OnMonzaEntriesLoaded(){
    Debug.Log("Monza entries loaded");
    //UIManager.Instance.SetText(bestMonzaTime, UIManager.Instance.FormatTime(PrefsManager.Instance.GetBestTime("Monza"), "lap"));
    //UIManager.Instance.SetText(monzalaps, PrefsManager.Instance.GetLapsAmount("Monza").ToString());
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
    UIManager.Instance.SetVisibilty(monzaLeaderBoardPlace, false);
    UIManager.Instance.SetVisibilty(submitScoreButton, false);
    
    UIManager.Instance.SetVisibilty(inputField, true);
    if (!PrefsManager.Instance.IsPrefsSetted("BestMonzaTime")){
    }
    else{
    }

}
public void CancelUpdateName(){
    if (PrefsManager.Instance.IsPrefsSetted("PlayerName")){
    UIManager.Instance.SetVisibilty(playerName, true);
    
    UIManager.Instance.SetVisibilty(inputField, false);
    

    if (PrefsManager.Instance.IsPrefsSetted("MonzaTimeUploaded")){
        UIManager.Instance.SetVisibilty(monzaLeaderBoardPlace, true);
        UIManager.Instance.SetVisibilty(submitScoreButton, false);
    }
    if (!PrefsManager.Instance.IsPrefsSetted("MonzaTimeUploaded")){
        UIManager.Instance.SetVisibilty(monzaLeaderBoardPlace, false);
        UIManager.Instance.SetVisibilty(submitScoreButton, true);
    }}
}

public void OnBestTimeUpdated(){
    UIManager.Instance.StartCoroutine(UIManager.Instance.SwitchVisibiltyForSeconds(BestTimeAlert, 2.6f));}
public void OnInvalidLap(){
    UIManager.Instance.SetVisibilty(InvalidLapAlert, true);
    InvalidLapAlert.GetComponent<Animator>().SetBool("lapIsValid", false);
}
public void OnNewLapStarted(){
    InvalidLapAlert.GetComponent<Animator>().SetBool("lapIsValid", true);
    if (InvalidLapAlert.activeSelf){
        UIManager.Instance.SwitchVisibilty(InvalidLapAlert);
    }
}
public void OnWrongDirection(){
    UIManager.Instance.SetVisibilty(WrongDirectionAlert, true);
}
public void OnCorrectDirection(){
    UIManager.Instance.SetVisibilty(WrongDirectionAlert, false);
}
}
