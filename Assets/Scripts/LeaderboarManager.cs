using UnityEngine;
using Dan.Main;
using Dan.Enums;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class LeaderboarManager : MonoBehaviour
{
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Transform entryParent;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private UICOntroller uiController;
    [SerializeField] private PrefsManager prefsManager;
    public int playerPosition {get; private set;}
    [SerializeField] private TMP_InputField playerNameInput;

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
    
    public void LoadEntries()
{
    uiController.ActivateUI("loadingPanel", true);
    uiController.SwitchButtonInteractable(uiController.submitScoreButton);
    if (entryObjects != null && entryObjects.Count > 0){
        ClearLeaderBoard();
            }
        Leaderboards.WireRacer.GetEntries(OnEntriesLoaded, (error) => {
            Debug.LogError(error);
        });
}

private void OnEntriesLoaded(Dan.Models.Entry[] entries)
{
    foreach (Dan.Models.Entry entry in entries)
    {
        GameObject entryObject = Instantiate(entryPrefab, entryParent);
        TextMeshProUGUI textObject = entryObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textObject.text = $"{entry.Rank}. {entry.Username} - {entry.Score}";
        entryObjects.Add(entryObject);
        bool isMine = entry.IsMine();
        
        if (isMine)
        {
            playerPosition = entry.Rank;
            entryObject.GetComponent<Image>().color = Color.green;
        }
    }
    uiController.playerPosition.text = "#" + playerPosition;
    uiController.playerName.text = prefsManager.playerName;
    Canvas.ForceUpdateCanvases();
    scrollRect.normalizedPosition = new Vector2(0, 1);
    uiController.ActivateUI("loadingPanel", false);

    if (prefsManager.playerEntryUploaded == 0){
        uiController.ActivateUI("InputField", true);
        uiController.SwitchButtonInteractable(uiController.submitScoreButton);
    }
    else{
        uiController.ActivateUI("UnderBoardInformation", true);
    }
}

public void UploadPlayerEntry(){
    uiController.SwitchButtonInteractable(uiController.submitScoreButton);
    string name = playerNameInput.text;
    Leaderboards.WireRacer.UploadNewEntry(name, (int)scoreManager.mainScore, (success) => {
        if (success){
            prefsManager.SetPlayerEntryUploaded(1);
            prefsManager.SetPlayerName(name);
            uiController.ActivateUI("InputField", false);
            uiController.ActivateUI("UnderBoardInformation", true);
            uiController.playerName.text = prefsManager.playerName;
            LoadEntries();
        }}, HandleLeaderboardError);

    if (playerNameInput.text == ""){
        if (!uiController.fieldAlert.gameObject.activeSelf){
            uiController.ActivateEmptyFieldAlert();
            uiController.StartCoroutine(uiController.ActivateButtonForSeconds(uiController.submitScoreButton, 2));
        }
    }
}
public void UpdatePlayerEntry(){
    Leaderboards.WireRacer.UploadNewEntry(prefsManager.playerName, (int)scoreManager.mainScore, (success) => {
        if (success){
            Debug.Log("Entry updated");
            LoadEntries();
        }
    }, HandleLeaderboardError);
}

private void ClearLeaderBoard(){
    foreach (GameObject entryObject in entryObjects){
            Destroy(entryObject);
            }
                entryObjects.Clear();
}

public void DeletePlayerEntry(){
    Leaderboards.WireRacer.DeleteEntry((success) => {
        if (success){
            Debug.Log("Entry deleted");
            LoadEntries();
            uiController.StartCoroutine(uiController.showDeleteEntryText());
            uiController.ActivateUI("InputField", false);
            uiController.ActivateUI("UnderBoardInformation", false);
        }
    }, HandleLeaderboardError);
}
private void HandleLeaderboardError(string error)
{
    Debug.LogError(error);
    uiController.SwitchButtonInteractable(uiController.submitScoreButton);
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
                uiController.fieldAlert.text = "Username already exists";
                uiController.ActivateEmptyFieldAlert(); 
                break;

            case StatusCode.Forbidden:      // 403
                Debug.LogError(403);
                uiController.fieldAlert.text = "Forbidden name";
                uiController.ActivateEmptyFieldAlert();
                break;

            case StatusCode.FailedToConnect: // 0
                Debug.LogError(0);
                uiController.fieldAlert.text = "Failed to connect";
                uiController.ActivateEmptyFieldAlert();
                break;

            case StatusCode.ServiceUnavailable: // 503
                Debug.LogError(503);
                uiController.fieldAlert.text = "Service unavailable";
                uiController.ActivateEmptyFieldAlert();
                break;

            case StatusCode.InternalServerError: // 500
                Debug.LogError(500);
                uiController.fieldAlert.text = "Internal server error";
                uiController.ActivateEmptyFieldAlert();
                uiController.ActivateEmptyFieldAlert();
                break;

            default:
                // На все остальные пока просто лог
                break;
        }
    }
}
}
