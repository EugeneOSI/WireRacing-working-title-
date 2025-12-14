using UnityEngine;
using UnityEngine.UI;
using Dan.Main;
using Dan.Enums;
using System.Collections.Generic;
using TMPro;
using System;
public class MonzaLeaderBoard : MonoBehaviour
{
    public bool isUploading {get; set;}
    [SerializeField] private MainMenuUIController mainMenuUIController;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform entryParent;
    public int playerPosition {get; private set;}
    [SerializeField] private TMP_InputField playerNameInput;

    public static event Action MonzaEntriesLoaded;
    public static event Action EmptyFieldAlert;
    public static event Action MonzaEntryDeleted;

    public StatusCode statusCode;

    private List<GameObject> entryObjects = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statusCode = StatusCode.NotFound;
        isUploading = false;
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
    isUploading = true;

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
    
    
    if (playerNameInput.text == ""){
        EmptyFieldAlert?.Invoke();
        return;
    }
    else{
    string name;
    name = playerNameInput.text;
    PrefsManager.Instance.SetPlayerName(name);
    }
    LeaderBoardsManager.Instance.UploadPlayerEntry("Monza", PrefsManager.Instance.GetPlayerName());

}
public void UpdatePlayerEntry(){
    Leaderboards.WireRacer_TimeTrial_Monza.UploadNewEntry(PrefsManager.Instance.GetPlayerName(), (int)PrefsManager.Instance.GetBestTime("Monza"), (success) => {
        if (success){
            Debug.Log("Entry updated");
            LeaderBoardsManager.Instance.LoadEntries("Monza");
        }
    }, LeaderBoardsManager.Instance.HandleLeaderboardError);
}

public void DeletePlayerEntry(){
    Leaderboards.WireRacer_TimeTrial_Monza.DeleteEntry((success) => {
        if (success){
            Debug.Log("Entry deleted");
            MonzaEntryDeleted?.Invoke();
            LeaderBoardsManager.Instance.LoadEntries("Monza");
            
        }
    }, LeaderBoardsManager.Instance.HandleLeaderboardError);
}


}


