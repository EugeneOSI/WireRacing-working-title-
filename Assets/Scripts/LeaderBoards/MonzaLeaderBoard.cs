using UnityEngine;
using UnityEngine.UI;
using Dan.Main;
using Dan.Enums;
using System.Collections.Generic;
using TMPro;
using System;
public class MonzaLeaderBoard : MonoBehaviour
{
    [SerializeField] private MainMenuUIController mainMenuUIController;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform entryParent;
    public int playerPosition {get; private set;}
    [SerializeField] private TMP_InputField playerNameInput;

    public static event Action MonzaEntriesLoaded;
    public static event Action MonzaUploadingEntry;

    public StatusCode statusCode;

    private List<GameObject> entryObjects = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statusCode = StatusCode.NotFound;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


public void OnEntriesLoaded(Dan.Models.Entry[] entries)
{

    ClearLeaderBoard();
    FillLeaderBoard(entries);
    MonzaEntriesLoaded?.Invoke();
    Canvas.ForceUpdateCanvases();
    scrollRect.normalizedPosition = new Vector2(0, 1);

}



private void ClearLeaderBoard(){
    if (entryObjects != null && entryObjects.Count > 0){
    foreach (GameObject entryObject in entryObjects){
            Destroy(entryObject);
            }
                entryObjects.Clear();}
}
private void FillLeaderBoard(Dan.Models.Entry[] entries){
        foreach (Dan.Models.Entry entry in entries)
    {
        GameObject entryObject = Instantiate(entryPrefab, entryParent);
        TextMeshProUGUI textObject = entryObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textObject.text = $"{entry.Rank}. {entry.Username} - {UIManager.Instance.FormatTime(entry.Score, "lap")}";
        entryObjects.Add(entryObject);
        bool isMine = entry.IsMine();
        
        if (isMine)
        {
            playerPosition = entry.Rank;
            entryObject.GetComponent<Image>().color = Color.green;
        }
    }
}

public void UploadPlayerEntry(){
    
    string name;
    if (!PrefsManager.Instance.IsPrefsSetted("PlayerName")){
        name = playerNameInput.text;
        PrefsManager.Instance.SetPlayerName(name);
    }
    else{
        name = PrefsManager.Instance.GetPlayerName();
    }
    Leaderboards.WireRacer_TimeTrial_Monza.UploadNewEntry(name, (int)PrefsManager.Instance.GetBestTime("Monza"), (success) => {
        if (success){
            PrefsManager.Instance.SetCircuitUploadStatus("Monza", 1);
            LeaderBoardsManager.Instance.LoadEntries("Monza");
        }}, HandleLeaderboardError);

    if (playerNameInput.text == ""){
        if (!mainMenuUIController.fieldAlert.gameObject.activeSelf){
            //mainMenuUIController.ActivateUI(mainMenuUIController.fieldAlert, true);
            //mainMenuUIController.StartCoroutine(mainMenuUIController.SwitchButtonInteractableForSeconds(mainMenuUIController.submitScoreButton, 2));
        }
    }
}
public void UpdatePlayerEntry(){
    Leaderboards.WireRacer_TimeTrial_Monza.UploadNewEntry(PrefsManager.Instance.playerName, (int)PrefsManager.Instance.bestMonzaTime, (success) => {
        if (success){
            Debug.Log("Entry updated");
            //LoadEntries();
        }
    }, HandleLeaderboardError);
}

public void DeletePlayerEntry(){
    Leaderboards.WireRacer_TimeTrial_Monza.DeleteEntry((success) => {
        if (success){
            Debug.Log("Entry deleted");
            //LoadEntries();
            //mainMenuUIController.StartCoroutine(mainMenuUIController.SwitchUIForSeconds(mainMenuUIController.deleteEntryText, 2));
            //mainMenuUIController.ActivateUI(mainMenuUIController.inputField, false);
            //mainMenuUIController.ActivateUI(mainMenuUIController.underBoardInformation, false);
        }
    }, HandleLeaderboardError);
}
private void HandleLeaderboardError(string error)
{
    Debug.LogError(error);
    //mainMenuUIController.SwitchButtonInteractable(mainMenuUIController.submitScoreButton);
    if (string.IsNullOrEmpty(error))
        return;

    var parts = error.Split(':');
    if (parts.Length == 0)
        return;

    if (int.TryParse(parts[0], out int codeInt))
    {
        statusCode = (StatusCode)codeInt;

        switch (statusCode)
        {
            case StatusCode.Conflict:       // 409
                Debug.LogError(409);
                //mainMenuUIController.SetText(mainMenuUIController.fieldAlert, "Username already exists");
                //mainMenuUIController.StartCoroutine(mainMenuUIController.SwitchUIForSeconds(mainMenuUIController.fieldAlert, 2)); 
                break;

            case StatusCode.Forbidden:      // 403
                Debug.LogError(403);
                //mainMenuUIController.SetText(mainMenuUIController.fieldAlert, "Forbidden name");
                //mainMenuUIController.StartCoroutine(mainMenuUIController.SwitchUIForSeconds(mainMenuUIController.fieldAlert, 2));
                break;

            case StatusCode.FailedToConnect: // 0
                Debug.LogError(0);
                ///mainMenuUIController.SetText(mainMenuUIController.fieldAlert, "Failed to connect");
                //mainMenuUIController.StartCoroutine(mainMenuUIController.SwitchUIForSeconds(mainMenuUIController.fieldAlert, 2));
                break;

            case StatusCode.ServiceUnavailable: // 503
                Debug.LogError(503);
                //mainMenuUIController.SetText(mainMenuUIController.fieldAlert, "Service unavailable");
                //mainMenuUIController.StartCoroutine(mainMenuUIController.SwitchUIForSeconds(mainMenuUIController.fieldAlert, 2));
                break;

            case StatusCode.InternalServerError: // 500
                Debug.LogError(500);
                //mainMenuUIController.SetText(mainMenuUIController.fieldAlert, "Internal server error");
                //mainMenuUIController.StartCoroutine(mainMenuUIController.SwitchUIForSeconds(mainMenuUIController.fieldAlert, 2));
                break;

            default:
                break;
        }
    }
}


}


