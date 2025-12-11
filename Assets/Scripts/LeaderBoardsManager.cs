using UnityEngine;
using Dan.Main;

public class LeaderBoardsManager : MonoBehaviour
{
    public static LeaderBoardsManager Instance {get; private set;}
    [SerializeField] MonzaLeaderBoard monzaLeaderBoard;
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


    void LoadEntries(string leaderboardName){
        switch(leaderboardName){
            case "Monza":
                Leaderboards.WireRacer_TimeTrial_Monza.GetEntries(monzaLeaderBoard.OnEntriesLoaded, (error) => {
                    Debug.LogError(error);
                });
                break;
            //case "EndlessMode":
        }
    }

    
}
