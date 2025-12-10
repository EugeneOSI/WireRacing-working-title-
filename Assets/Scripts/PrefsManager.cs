using UnityEngine;

public class PrefsManager : MonoBehaviour
{
    public string playerName {get; private set;}
    public float bestScore {get; private set;}
    public float bestMonzaTime {get; private set;}
    public int playerEntryUploaded {get; private set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName")){
        playerName = PlayerPrefs.GetString("PlayerName");
        }
        else{
            playerName = "Player";
        }

        if (PlayerPrefs.HasKey("BestScore")){
            bestScore = PlayerPrefs.GetFloat("BestScore");
        }
        else{
            bestScore = 0;
        }

        if (PlayerPrefs.HasKey("BestMonzaTime")){
            bestMonzaTime = PlayerPrefs.GetFloat("BestMonzaTime");
        }
        else{
            bestMonzaTime = 0;
        }

        if (PlayerPrefs.HasKey("PlayerEntryUploaded")){
            playerEntryUploaded = PlayerPrefs.GetInt("PlayerEntryUploaded");
        }
        else{
            playerEntryUploaded = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
            if (Input.GetKeyDown(KeyCode.I)){
            Debug.Log("Player Name: " + playerName);
            Debug.Log("Best Score: " + bestScore);
            Debug.Log("Best Monza Time: " + bestMonzaTime);
            Debug.Log("Player Entry Uploaded: " + playerEntryUploaded);
        }
        /*if (Input.GetKeyDown(KeyCode.R)){
            ResetPrefs();
            Debug.Log("Prefs reset");
        }*/
    }

    public void SaveBestScore(float score){
        bestScore = score;
        PlayerPrefs.SetFloat("BestScore", bestScore);
    }

    public void SaveBestTime(float time, string circuitName){
        bestMonzaTime = time;
        switch(circuitName){
            case "Monza":
                PlayerPrefs.SetFloat("BestMonzaTime", bestMonzaTime);
                break;
        }
    }
    public void SetPlayerEntryUploaded(int uploaded){
        playerEntryUploaded = uploaded;
        PlayerPrefs.SetInt("PlayerEntryUploaded", playerEntryUploaded);
    }
    public void SetPlayerName(string name){
        playerName = name;
        PlayerPrefs.SetString("PlayerName", playerName);
    }

    //debuging methods
    public void ResetPrefs(){
        playerName = "Player";
        PlayerPrefs.SetString("PlayerName", playerName);
        bestScore = 0;
        PlayerPrefs.SetFloat("BestScore", bestScore);
        bestMonzaTime = 0;
        PlayerPrefs.SetFloat("BestMonzaTime", bestMonzaTime);
        playerEntryUploaded = 0;
        PlayerPrefs.SetInt("PlayerEntryUploaded", playerEntryUploaded);
    }
}
