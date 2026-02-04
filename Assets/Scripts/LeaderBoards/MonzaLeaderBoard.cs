using UnityEngine;
using UnityEngine.UI;
using Dan.Main;
using Dan.Enums;
using System.Collections.Generic;
using TMPro;
using System;
public class MonzaLeaderBoard : MonoBehaviour
{
    public bool isLoaded {get; set;}
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Transform entryParent;
    public int playerPosition {get; private set;}
    [SerializeField] private TMP_InputField playerNameInput;

    public static event Action MonzaEntriesLoaded;
    public static event Action<String> InpudFieldAlert;

    public StatusCode statusCode;

    private List<GameObject> entryObjects = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statusCode = StatusCode.NotFound;
        isLoaded = false;
        LeaderBoardsManager.EntriesLoaded += OnEntriesLoaded;
    }
    void OnDestroy()
    {
        LeaderBoardsManager.EntriesLoaded -= OnEntriesLoaded;
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
    isLoaded = true;

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
        float time = (float)entry.Score / 1000;
        textObject.text = $"{entry.Rank}. {entry.Username} - {UIManager.Instance.FormatTime(time, "lap")}";
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
        InpudFieldAlert?.Invoke("Field is empty");
        return;
    }
    else if (playerNameInput.text.Length > 6){
        InpudFieldAlert?.Invoke("Name is too long");
        return;
    }
    else{
        name = playerNameInput.text;
        PrefsManager.Instance.SetPlayerName(name);
    }
    LeaderBoardsManager.Instance.UploadPlayerEntry("Monza", PrefsManager.Instance.GetPlayerName());

}

public void UpdatePlayerName(){
    PrefsManager.Instance.SetPlayerName(playerNameInput.text);
}


public void LoadLeaderboard(){
    if (!isLoaded){
        LeaderBoardsManager.Instance.LoadEntries("Monza");
    }
}
public void UpdateLeaderboard(){
    isLoaded = false;
    LeaderBoardsManager.Instance.UpdatePlayerEntry("Monza");
}

public void DeletePlayerEntry(){
    LeaderBoardsManager.Instance.DeletePlayerEntry("Monza");
}


}


