using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class TimeTrialManager : MonoBehaviour
{
    [SerializeField] private MonzaLeaderBoard monzaLeaderBoard;
    public static TimeTrialManager Instance { get; private set; }

    [Header("Settings")]
    public int sectorCount = 3;

    [Header("Runtime")]
    public bool lapRunning;
    public bool lapValid;
    public int currentLapIndex = 0;
    int currentSectorIndex = -1;

    float lapStartTime;
    float lastSectorTimeStamp;

    float[] currentLapSectors;
    float[] previousLapSectors;
    public float previousLapTime = -1f;
    public float bestLapTime = -1f;

    [Header("UI")]
    public Color fasterColor = Color.green;
    public Color slowerColor = Color.yellow;
    public Color neutralColor = Color.white;
    public TMP_Text lapTimeText;
    public TMP_Text bestLapTimeText;
    public TMP_Text previousLapTimeText;
    public TMP_Text totalDeltaText;
    public TMP_Text sectorDeltaText;
    public TMP_Text lapNumberText;
    public Image[] sectorImages;

    [Header("Game Objects")]
    private PlayerTimeTrial player;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TTUIController ttUIController;
    public bool IsPaused {get; set;}


    public static event Action BestTimeUpdated;
    public static event Action InvalidLap;
    public static event Action NewLapStarted;




    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        player = GameObject.Find("Player").GetComponent<PlayerTimeTrial>();
        currentLapSectors  = new float[sectorCount];
        previousLapSectors = new float[sectorCount];
        ResetArray(previousLapSectors, -1f);

        SplineDirectionTracker.OnWrongDirection += () => InvalidateCurrentLap("Wrong Direction");
    }
    void OnDestroy()
    {
        SplineDirectionTracker.OnWrongDirection -= () => InvalidateCurrentLap("Wrong Direction");
    }

private void Start()
{
    if (PrefsManager.Instance.IsPrefsSetted("MonzaTimeUploaded")){
        bestLapTime = PrefsManager.Instance.GetBestTime("Monza");
        bestLapTimeText.text = FormatTime(bestLapTime,"lap");
    }
    StartCoroutine(LoadLeaderboard());
    
}
    void Update()
    {
        if (player.mistake)
        {
            InvalidateCurrentLap("Mistake");
        }
        lapTimeText.text = FormatTime(CurrentLapTime,"lap");

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLap();
        }

        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.PauseGame();
            Debug.Log("Pause game");
        }*/

    }


    void ResetArray(float[] arr, float value)
    {
        for (int i = 0; i < arr.Length; i++)
            arr[i] = value;
    }

    public float CurrentLapTime =>
        lapRunning ? Time.time - lapStartTime : 0f;

    // Вызывается из триггера старт/финиш
    public void OnStartFinishCrossed()
    {
        if (!lapRunning)
        {
            // Первый запуск/после остановки
            StartNewLap();
        }
        else
        {
            FinishLap();
            StartNewLap();
        }
    }

    void StartNewLap()
    {
        lapRunning = true;
        lapValid   = true;

        currentLapIndex++;
        lapNumberText.text = "Lap " + currentLapIndex.ToString();
        lapStartTime = Time.time;
        lastSectorTimeStamp = lapStartTime;

        currentLapSectors = new float[sectorCount];
        currentSectorIndex = -1;
        lapTimeText.color = Color.white;
        StartCoroutine(ResetSectorsUI(2f));
        NewLapStarted?.Invoke();
    }

void FinishLap()
{
    float now = Time.time;
    PrefsManager.Instance.SaveLapsAmount("Monza", PrefsManager.Instance.GetLapsAmount("Monza") + 1);


    int lastIndex = sectorCount - 1;

    if (currentLapSectors != null &&
        lastIndex >= 0 && lastIndex < sectorCount &&
        currentLapSectors[lastIndex] <= 0f &&  
        lastSectorTimeStamp > 0f)
    {
        float lastSectorTime = now - lastSectorTimeStamp;
        RegisterSector(lastIndex, lastSectorTime);
    }

    float lapTime = now - lapStartTime;

    if (!lapValid)
    {
        previousLapTime = lapTime;
        Debug.Log($"Lap {currentLapIndex} INVALID, time = {FormatTime(lapTime,"lap")}");
        return;
    }
    
    previousLapTime = lapTime;
    currentLapSectors.CopyTo(previousLapSectors, 0);

    if (lapValid&& (bestLapTime < 0f || lapTime < PrefsManager.Instance.GetBestTime("Monza"))){
    if (bestLapTime < 0f){
        monzaLeaderBoard.isLoaded = false;
        LeaderBoardsManager.Instance.LoadEntries("Monza");
    }
    BestTimeUpdated?.Invoke();
    bestLapTime = lapTime;
    bestLapTimeText.text = FormatTime(bestLapTime,"lap");
    PrefsManager.Instance.SaveBestTime(bestLapTime, "Monza");
    if (PrefsManager.Instance.IsPrefsSetted("MonzaTimeUploaded")){     
    Debug.Log("Best time updated");
        monzaLeaderBoard.UpdateLeaderboard();}

    Debug.Log($"Lap {currentLapIndex} FINISHED: {FormatTime(lapTime,"lap")}   Best: {FormatTime(bestLapTime,"lap")}");

    lapTimeText.text = FormatTime(lapTime,"lap");
    previousLapTimeText.text = FormatTime(previousLapTime,"lap");
}}


    // Вызывается из триггера сектора
public void OnSectorCrossed(int sectorIndex)
{
    if (!lapRunning || sectorIndex < 0 || sectorIndex >= sectorCount)
        return;

    float now = Time.time;
    float sectorTime = now - lastSectorTimeStamp;
    lastSectorTimeStamp = now;

    RegisterSector(sectorIndex, sectorTime);
}


    // Вызов при выезде за пределы трассы / срезке / обратном направлении
    public void InvalidateCurrentLap(string reason)
    {
        if (!lapRunning) return;

        lapValid = false;
        Debug.Log($"Lap {currentLapIndex} INVALIDATED: {reason}");
        lapTimeText.color = Color.red;
        StartCoroutine(ResetSectorsUI(0.1f));
        // UI: показать сообщение, покрасить таймер в красный и т.п.
        InvalidLap?.Invoke();
    }

void RegisterSector(int sectorIndex, float sectorTime)
{
    if (sectorIndex < 0 || sectorIndex >= sectorCount)
        return;

    currentLapSectors[sectorIndex] = sectorTime;
    currentSectorIndex = sectorIndex;

    bool hasPrevLap = previousLapTime > 0f && previousLapSectors[sectorIndex] > 0f;
    float sectorDelta = 0f;

    if (hasPrevLap)
        sectorDelta = sectorTime - previousLapSectors[sectorIndex];

    float currentTotal = 0f;
    float previousTotal = 0f;

    for (int i = 0; i <= sectorIndex; i++)
    {
        currentTotal += currentLapSectors[i];
        if (previousLapSectors[i] > 0f)
            previousTotal += previousLapSectors[i];
    }

    float totalDelta = hasPrevLap ? currentTotal - previousTotal : 0f;

    Debug.Log($"Sector {sectorIndex}: {sectorTime:F3}  ΔS={sectorDelta:F3}  ΔT={totalDelta:F3}");
// UI
    if(lapValid){
    sectorImages[sectorIndex].color = sectorDelta < 0f ? fasterColor : (sectorDelta > 0f ? slowerColor : neutralColor);
    sectorDeltaText.text = FormatTime(sectorDelta,"delta");
    totalDeltaText.text = FormatTime(totalDelta,"delta");}
}

    public string FormatTime(float time, string type)
    {
        int minutes = (int)(time / 60f);
        float seconds = time - minutes * 60f;
        string result = "";
        switch (type){
            case "lap":
                if (time <= 0f) return "--:--.---";
               result = string.Format("{0:00}:{1:00.000}", minutes, seconds);
               break;
            case "delta":
            result = string.Format("{0:0.000}", time);
             break;
        }
        return result;

    }

    IEnumerator ResetSectorsUI(float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < sectorCount; i++)
        {
            sectorImages[i].color = neutralColor;

        }
        sectorDeltaText.text = "--.---";
        totalDeltaText.text = "--.---";
    }

    public void ExitToMainMenu(){
        GameManager.Instance.LoadScene("MainMenu");
    }
    public void ResumeGame(){
        GameManager.Instance.PauseGame();
    }

    public void RestartLap(){
        GameManager.Instance.LoadScene("TimeTrial_Monza");
    }
    IEnumerator LoadLeaderboard(){
    //monzaLeaderBoard.LoadLeaderboard();
    yield return new WaitForSeconds(2f);
    monzaLeaderBoard.isLoaded = false;
    monzaLeaderBoard.LoadLeaderboard();
}
}



