using UnityEngine;
using Dan.Main;
using System;
using Dan.Enums;

public class LeaderBoardsManager : MonoBehaviour
{
    public static LeaderBoardsManager Instance {get; private set;}
    public StatusCode statusCode;

    public static event Action EntriesLoading;
    public static event Action EnteryUploading;
    public static event Action<Dan.Models.Entry[]> EntriesLoaded;
    public static event Action<string> OnLeaderboardError;
    public static event Action MonzaEntryDeleted;
    public static event Action EndlessModEntryDeleted;
    public Dan.Models.Entry[] entries {get; private set;}
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log(GetInstanceID());
    }


    public void LoadEntries(string leaderboardName){        
        switch(leaderboardName){
            case "Monza":
                Leaderboards.WireRacer_TimeTrial_Monza.GetEntries((success) => {
                EntriesLoaded?.Invoke(success);
                });
                break;
            case "EndlessMod":
                Leaderboards.WireRacer.GetEntries((success) => {
                EntriesLoaded?.Invoke(success);
                });
                break;
        }
    }


    public void UploadPlayerEntry(string leaderboardName, string name){   
        EnteryUploading?.Invoke();
        switch(leaderboardName){
            case "Monza":
            float uploadTime = PrefsManager.Instance.GetBestTime("Monza") * 1000;
            Leaderboards.WireRacer_TimeTrial_Monza.UploadNewEntry(name, (int)uploadTime, (success) => {
        if (success){
            PrefsManager.Instance.SetCircuitUploadStatus("Monza", 1);
            LoadEntries("Monza");
        }}, HandleLeaderboardError);
        break;
           case "EndlessMod":
        float uploadScore = PrefsManager.Instance.GetBestScore();
        Leaderboards.WireRacer.UploadNewEntry(name, (int)uploadScore, (success) => {
        if (success){
            PrefsManager.Instance.SetBestScoreUploadStatus(1);
            LoadEntries("EndlessMod");
        }}, HandleLeaderboardError);
        break;
        }
    }

    public void UpdatePlayerEntry(string leaderboardName){
        switch(leaderboardName){
            case "Monza":
            float uploadTime = PrefsManager.Instance.GetBestTime("Monza") * 1000;
            Leaderboards.WireRacer_TimeTrial_Monza.UploadNewEntry(PrefsManager.Instance.GetPlayerName(), (int)uploadTime, (success) => {
        if (success){
            LoadEntries("Monza");
        }}, HandleLeaderboardError);
        break;
        case "EndlessMod":
        float uploadScore = PrefsManager.Instance.GetBestScore();
        Leaderboards.WireRacer.UploadNewEntry(PrefsManager.Instance.GetPlayerName(), (int)uploadScore, (success) => {
        if (success){
            LoadEntries("EndlessMod");
        }}, HandleLeaderboardError);
        break;
    }}

    public void DeletePlayerEntry(string leaderboardName){
        switch(leaderboardName){
            case "Monza":
            Leaderboards.WireRacer_TimeTrial_Monza.DeleteEntry((success) => {
                if (success){
                    LoadEntries("Monza");
                    MonzaEntryDeleted?.Invoke();
                }
            }, HandleLeaderboardError);
        break;
        case "EndlessMod":
        Leaderboards.WireRacer.DeleteEntry((success) => {
            if (success){
                LoadEntries("EndlessMod");
                EndlessModEntryDeleted?.Invoke();
            }
        }, HandleLeaderboardError);
        break;
    }}

    public void HandleLeaderboardError(string error)
{
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
                OnLeaderboardError?.Invoke("409");
                //mainMenuUIController.SetText(mainMenuUIController.fieldAlert, "Username already exists");
                //mainMenuUIController.StartCoroutine(mainMenuUIController.SwitchUIForSeconds(mainMenuUIController.fieldAlert, 2)); 
                break;

            case StatusCode.Forbidden:      // 403
                Debug.LogError(403);
                OnLeaderboardError?.Invoke("403");
                //mainMenuUIController.SetText(mainMenuUIController.fieldAlert, "Forbidden name");
                //mainMenuUIController.StartCoroutine(mainMenuUIController.SwitchUIForSeconds(mainMenuUIController.fieldAlert, 2));
                break;

            case StatusCode.FailedToConnect: // 0
                Debug.LogError(0);
                OnLeaderboardError?.Invoke("0");
                ///mainMenuUIController.SetText(mainMenuUIController.fieldAlert, "Failed to connect");
                //mainMenuUIController.StartCoroutine(mainMenuUIController.SwitchUIForSeconds(mainMenuUIController.fieldAlert, 2));
                break;

            case StatusCode.ServiceUnavailable: // 503
                Debug.LogError(503);
                OnLeaderboardError?.Invoke("503");
                //mainMenuUIController.SetText(mainMenuUIController.fieldAlert, "Service unavailable");
                //mainMenuUIController.StartCoroutine(mainMenuUIController.SwitchUIForSeconds(mainMenuUIController.fieldAlert, 2));
                break;

            case StatusCode.InternalServerError: // 500
                Debug.LogError(500);
                OnLeaderboardError?.Invoke("500");
                //mainMenuUIController.SetText(mainMenuUIController.fieldAlert, "Internal server error");
                //mainMenuUIController.StartCoroutine(mainMenuUIController.SwitchUIForSeconds(mainMenuUIController.fieldAlert, 2));
                break;

            default:
                break;
        }
        PrefsManager.Instance.DeleteNamePrefs();
    }
}

    
}
