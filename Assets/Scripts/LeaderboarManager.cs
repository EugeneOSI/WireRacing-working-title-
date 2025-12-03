using UnityEngine;
using Dan.Main;
using UnityEngine.UI;
using TMPro;
public class LeaderboarManager : MonoBehaviour
{
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Transform entryParent;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private ScoreManager scoreManager;
    public bool EntriesLoading {get; set;}
    bool playerEnteryUploaded {get; set;} = true;
    string playerName {get; set;}
    [SerializeField] private TMP_InputField playerNameInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void LoadEntries()
{
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
    }
    Canvas.ForceUpdateCanvases();
    scrollRect.normalizedPosition = new Vector2(0, 1);
}

public void UploadPlayerEntry(){
    if (playerNameInput.text != null){
    string name = playerNameInput.text;
    Leaderboards.WireRacer.UploadNewEntry(name, (int)scoreManager.mainScore, (success) => {
        if (success){
            playerEnteryUploaded = true;
            playerName = name;
            LoadEntries();
        }
    }, (error) => {
        Debug.LogError(error);
    });}
    else{
        Debug.LogError("Player name is null");
    }

}
}
