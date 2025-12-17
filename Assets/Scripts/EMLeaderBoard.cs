using UnityEngine;
using Dan.Main;
using Dan.Enums;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;
public class EMLeaderBoard : MonoBehaviour
{
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform entryParent;
    public int playerPosition {get; private set;}
    [SerializeField] private TMP_InputField playerNameInput;

    public static event Action EndlessModEntriesLoaded;
    public static event Action EmptyFieldAlert;

    public StatusCode statusCode;

    private List<GameObject> entryObjects = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statusCode = StatusCode.NotFound;
        LeaderBoardsManager.EntriesLoaded += OnEntriesLoaded;
    }
    void OnDestroy()
    {
        LeaderBoardsManager.EntriesLoaded -= OnEntriesLoaded;
    }

    public void OnEntriesLoaded(Dan.Models.Entry[] entries)
{

    ClearLeaderBoard();
    FillLeaderBoard(entries);
    EndlessModEntriesLoaded?.Invoke();
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
        //scrollRect.transform.SetParent(entryParent);
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
}

    
public void UploadPlayerEntry(){
    
    
    string name;
    if (PrefsManager.Instance.IsPrefsSetted("PlayerName")){
        name = PrefsManager.Instance.GetPlayerName();
    }
    else if(playerNameInput.text == ""){
        EmptyFieldAlert?.Invoke();
        return;
    }
    else{
        name = playerNameInput.text;
        PrefsManager.Instance.SetPlayerName(name);
    }
    LeaderBoardsManager.Instance.UploadPlayerEntry("EndlessMod", PrefsManager.Instance.GetPlayerName());

}

public void UpdatePlayerName(){
    PrefsManager.Instance.SetPlayerName(playerNameInput.text);
}

public void LoadLeaderboard(){
        LeaderBoardsManager.Instance.LoadEntries("EndlessMod");
    
}
public void UpdateLeaderboard(){
    LeaderBoardsManager.Instance.UpdatePlayerEntry("EndlessMod");
}  
public void DeletePlayerEntry(){
    LeaderBoardsManager.Instance.DeletePlayerEntry("EndlessMod");
}
}

