using System.Collections.Generic;
using UnityEngine;

public class PrefsManager : MonoBehaviour
{
    public static PrefsManager Instance {get; private set;}
    public string playerName {get; private set;}
    public float bestScore {get; private set;}


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
        if (IsPrefsSetted("PlayerName")){
            Debug.Log("PlayerName: " + GetPlayerName());
        }
        if (IsPrefsSetted("BestMonzaTime")){
            Debug.Log("BestMonzaTime: " + GetBestTime("Monza"));
        }
        if (IsPrefsSetted("MonzaLaps")){
            Debug.Log("MonzaLaps: " + GetLapsAmount("Monza"));
        }
        if (IsPrefsSetted("MonzaTimeUploaded")){
            Debug.Log("MonzaTimeUploaded: " + GetCircuitUploadStatus("Monza"));
        }
        if (IsPrefsSetted("BestScore")){
            Debug.Log("BestScore: " + GetBestScore());
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)){
            DeleteNamePrefs();
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
    public float GetBestScore(){
        if (IsPrefsSetted("BestScore")){
            return PlayerPrefs.GetFloat("BestScore");
        }
        else{
            Debug.Log("BestScore not found");
            return 0;
        }
    }

    public void SaveBestTime(float time, string circuitName){
        switch(circuitName){
            case "Monza":
                PlayerPrefs.SetFloat("BestMonzaTime", time);
                break;
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
    public int GetCircuitUploadStatus(string circuitName){
        switch(circuitName){
            case "Monza":
                if (PlayerPrefs.HasKey("MonzaTimeUploaded")){
                    return PlayerPrefs.GetInt("MonzaTimeUploaded");
                }
                else{
                    Debug.Log("MonzaTimeUploaded not found");
                    return 0;
                }
                default:
                    Debug.Log("Circuit name not found");
                    return 0;
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
        PlayerPrefs.DeleteKey("BestMonzaTimeUploaded");
        PlayerPrefs.DeleteKey("MonzaLaps");
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.DeleteKey("MonzaTimeUploaded");
    }

    public void DeleteNamePrefs(){
        PlayerPrefs.DeleteKey("PlayerName");
    }
}
