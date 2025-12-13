using UnityEngine;
using Dan.Main;
using System;
using Dan.Enums;

public class LeaderBoardsManager : MonoBehaviour
{
    public static LeaderBoardsManager Instance {get; private set;}
    [SerializeField] MonzaLeaderBoard monzaLeaderBoard;
    public StatusCode statusCode;

    public static event Action EntriesLoading;
    public static event Action<string> OnLeaderboardError;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void LoadEntries(string leaderboardName){        
        switch(leaderboardName){
            case "Monza":
                if (!monzaLeaderBoard.isUploading){
                EntriesLoading?.Invoke();
                Leaderboards.WireRacer_TimeTrial_Monza.GetEntries(monzaLeaderBoard.OnEntriesLoaded, (error) => {
                        Debug.LogError(error);
                    });
                }
                break;
        }
    }

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
