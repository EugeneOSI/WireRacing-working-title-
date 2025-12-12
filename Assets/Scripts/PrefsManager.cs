using System.Collections.Generic;
using UnityEngine;

public class PrefsManager : MonoBehaviour
{
    public static PrefsManager Instance {get; private set;}
    public string playerName {get; private set;}
    public string monzaPlayerName {get; private set;}
    public float bestScore {get; private set;}
    public float bestMonzaTime {get; private set;}
    public int monzaLaps {get; private set;}
    public bool monzaTimeUploaded {get; private set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

        void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else{
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerName")){
            playerName = PlayerPrefs.GetString("PlayerName");
        }

    }

    // Update is called once per frame
    void Update()
    {
            if (Input.GetKeyDown(KeyCode.M)){
            Debug.Log("Player Name: " + playerName);
            Debug.Log("Best Score: " + bestScore);
            Debug.Log("Best Monza Time: " + bestMonzaTime);
        }
        if (Input.GetKeyDown(KeyCode.I)){
            DeleteNamePrefs();
        }
        if (Input.GetKeyDown(KeyCode.O)){
            DeleteMonzaPrefs();
        }
        if (Input.GetKeyDown(KeyCode.S)){
            Debug.Log("PlayerName: " + GetPlayerName());
        }
        if (Input.GetKeyDown(KeyCode.Space)){
            SaveBestTime(10, "Monza");
        }
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
    public void SaveLapsAmount (string circuitName, int laps){
        switch(circuitName){
            case "Monza":
                PlayerPrefs.SetInt("MonzaLaps", laps);
                break;
        }
    }
    public int GetLapsAmount(string circuitName){
        switch(circuitName){
            case "Monza":
                if (PlayerPrefs.HasKey("MonzaLaps")){
                    return PlayerPrefs.GetInt("MonzaLaps");
                }
                else{
                    Debug.Log("MonzaLaps not found");
                    return 0;
                }
            default:
                Debug.Log("Circuit name not found");
                return 0;
        }
    }

    public float GetBestTime(string circuitName){
        switch(circuitName){
            case "Monza":
                if (PlayerPrefs.HasKey("BestMonzaTime")){
                    return PlayerPrefs.GetFloat("BestMonzaTime");
                }
                else{
                    Debug.Log("BestMonzaTime not found");
                    return 0;
                }
            default:
                Debug.Log("Circuit name not found");
                return 0;
        }

    }
    public void SetPlayerName(string name){
        PlayerPrefs.SetString("PlayerName", name);
    }
    public string GetPlayerName(){
        if (PlayerPrefs.HasKey("PlayerName")){;
            return PlayerPrefs.GetString("PlayerName");
        }
        else{
            Debug.Log("PlayerName not found");
            return "no name";
        }
    }

    public void SetCircuitUploadStatus(string circuitName, int status){
        switch(circuitName){
            case "Monza":
                PlayerPrefs.SetInt("MonzaTimeUploaded", status);
                break;
        }
    }


    public bool IsPrefsSetted(string pref){
        return PlayerPrefs.HasKey(pref);
    }

    //debuging methods

    public void ResetPrefs(){
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.DeleteKey("BestScore");
        PlayerPrefs.DeleteKey("BestMonzaTime");
        PlayerPrefs.DeleteKey("PlayerEntryUploaded");
    }

    public void ResetMonzaPrefs(){
        PlayerPrefs.DeleteKey("BestMonzaTime");
        PlayerPrefs.DeleteKey("MonzaPlayerName");
        PlayerPrefs.DeleteKey("BestMonzaTimeUploaded");
        PlayerPrefs.DeleteKey("MonzaLaps");
    }

    public void DeleteNamePrefs(){
        PlayerPrefs.DeleteKey("PlayerName");
        Debug.Log("Player Name prefs deleted");
    }

    public void DeleteMonzaPrefs(){
        bestMonzaTime = 0;
        PlayerPrefs.DeleteKey("BestMonzaTime");
        monzaPlayerName = "";
        PlayerPrefs.DeleteKey("MonzaPlayerName");
        PlayerPrefs.DeleteKey("BestMonzaTimeUploaded");
        monzaLaps = 0;
        PlayerPrefs.DeleteKey("MonzaLaps");
        Debug.Log("Monza prefs deleted");
    }
}
