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
    [SerializeField] private EM_UIController uiController;

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
    uiController.ActivateUI(uiController.loadingPanel, true);
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
    uiController.SetText(uiController.playerPosition, "#" + playerPosition);
    uiController.SetText(uiController.playerName, PrefsManager.Instance.playerName);
    Canvas.ForceUpdateCanvases();
    scrollRect.normalizedPosition = new Vector2(0, 1);
    uiController.ActivateUI(uiController.loadingPanel, false);

    if (!PrefsManager.Instance.IsPrefsSetted("BestScore")){
        uiController.ActivateUI(uiController.inputField, true);
        uiController.SwitchButtonInteractable(uiController.submitScoreButton);
    }
    else{
        uiController.ActivateUI(uiController.underBoardInformation, true);
    }
}

public void UploadPlayerEntry(){
    uiController.SwitchButtonInteractable(uiController.submitScoreButton);
    string name = playerNameInput.text;
    Leaderboards.WireRacer.UploadNewEntry(name, (int)PrefsManager.Instance.bestScore, (success) => {
        if (success){
            PrefsManager.Instance.SetPlayerName(name);
            uiController.ActivateUI(uiController.inputField, false);
            uiController.ActivateUI(uiController.underBoardInformation, true);
            uiController.SetText(uiController.playerName, PrefsManager.Instance.playerName);
            LoadEntries();
        }}, HandleLeaderboardError);

    if (playerNameInput.text == ""){
        if (!uiController.fieldAlert.gameObject.activeSelf){
            uiController.ActivateUI(uiController.fieldAlert, true);
            uiController.StartCoroutine(uiController.SwitchButtonInteractableForSeconds(uiController.submitScoreButton, 2));
        }
    }
}
public void UpdatePlayerEntry(){
    Leaderboards.WireRacer.UploadNewEntry(PrefsManager.Instance.playerName, (int)scoreManager.mainScore, (success) => {
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
            uiController.StartCoroutine(uiController.SwitchUIForSeconds(uiController.deleteEntryText, 2));
            uiController.ActivateUI(uiController.inputField, false);
            uiController.ActivateUI(uiController.underBoardInformation, false);
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
                uiController.SetText(uiController.fieldAlert, "Username already exists");
                uiController.StartCoroutine(uiController.SwitchUIForSeconds(uiController.fieldAlert, 2)); 
                break;

            case StatusCode.Forbidden:      // 403
                Debug.LogError(403);
                uiController.SetText(uiController.fieldAlert, "Forbidden name");
                uiController.StartCoroutine(uiController.SwitchUIForSeconds(uiController.fieldAlert, 2));
                break;

            case StatusCode.FailedToConnect: // 0
                Debug.LogError(0);
                uiController.SetText(uiController.fieldAlert, "Failed to connect");
                uiController.StartCoroutine(uiController.SwitchUIForSeconds(uiController.fieldAlert, 2));
                break;

            case StatusCode.ServiceUnavailable: // 503
                Debug.LogError(503);
                uiController.SetText(uiController.fieldAlert, "Service unavailable");
                uiController.StartCoroutine(uiController.SwitchUIForSeconds(uiController.fieldAlert, 2));
                break;

            case StatusCode.InternalServerError: // 500
                Debug.LogError(500);
                uiController.SetText(uiController.fieldAlert, "Internal server error");
                uiController.StartCoroutine(uiController.SwitchUIForSeconds(uiController.fieldAlert, 2));
                break;

            default:
                break;
        }
    }
}
}
