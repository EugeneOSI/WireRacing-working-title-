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
    [SerializeField] private GameObject gameModeMenu;
    [SerializeField] private GameObject timeTrialMenu;

    

    [SerializeField] public GameObject loadingPanel;
    [SerializeField] public GameObject submitScoreButton;
    [SerializeField] public GameObject inputField;
    [SerializeField] public GameObject fieldAlert;
    [SerializeField] public GameObject playerName;
    [SerializeField] public GameObject bestMonzaTime;
    [SerializeField] public GameObject monzalaps;
    [SerializeField] public GameObject deleteEntryText;


void Awake(){
    MonzaLeaderBoard.MonzaEntriesLoaded += OnMonzaEntriesLoaded;
}
void OnDestroy(){
    MonzaLeaderBoard.MonzaEntriesLoaded -= OnMonzaEntriesLoaded;
}
private void OnMonzaEntriesLoaded(){
    UIManager.Instance.SetText(bestMonzaTime, UIManager.Instance.FormatTime(PrefsManager.Instance.GetBestTime("Monza"), "lap"));
    UIManager.Instance.SetText(monzalaps, PrefsManager.Instance.GetLapsAmount("Monza").ToString());
    UIManager.Instance.SetVisibilty(loadingPanel, false);
    if (!PrefsManager.Instance.IsPrefsSetted("BestMonzaTime")){
        UIManager.Instance.SetButtonInteractable(submitScoreButton, false);
    }

    if (!PrefsManager.Instance.IsPrefsSetted("MonzaTimeUploaded") && !PrefsManager.Instance.IsPrefsSetted("PlayerName")){
        UIManager.Instance.SetVisibilty(inputField, true);
        UIManager.Instance.SetVisibilty(submitScoreButton, true);

        UIManager.Instance.SetVisibilty(playerName, false);
        UIManager.Instance.SetVisibilty(monzaLeaderBoardPlace, false);

        Debug.Log("No player name found and best monza time uploaded is 0");
    }
    if (!PrefsManager.Instance.IsPrefsSetted("MonzaTimeUploaded") && PrefsManager.Instance.IsPrefsSetted("PlayerName")){
        UIManager.Instance.SetVisibilty(submitScoreButton, true);
        UIManager.Instance.SetVisibilty(playerName, true);
        UIManager.Instance.SetText(playerName, "Your name: "+PrefsManager.Instance.GetPlayerName());

        UIManager.Instance.SetVisibilty(monzaLeaderBoardPlace, false);
        Debug.Log("Player name found and best monza time uploaded is 0");
    }
    if (PrefsManager.Instance.IsPrefsSetted("MonzaTimeUploaded") && PrefsManager.Instance.IsPrefsSetted("PlayerName")){
        UIManager.Instance.SetVisibilty(playerName, true);
        UIManager.Instance.SetText(playerName, "Your name: "+PrefsManager.Instance.playerName);

        UIManager.Instance.SetVisibilty(monzaLeaderBoardPlace, true);
        UIManager.Instance.SetText(monzaLeaderBoardPlace, "Your position: "+monzaLeaderBoard.playerPosition.ToString());

        UIManager.Instance.SetVisibilty(inputField, false);
        UIManager.Instance.SetVisibilty(submitScoreButton, false);
        Debug.Log("Player name found and best monza time uploaded is not 0");
    }
}



}
