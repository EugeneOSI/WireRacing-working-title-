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
    [SerializeField] private DirectionTracker directionTracker;
    public bool IsPaused {get; set;}

    private Action onWrongDirectionHandler;

    public static event Action BestTimeUpdated;
    public static event Action InvalidLap;
    public static event Action NewLapStarted;
    public static event Action CrossedFinishLineWrongSide;



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

        onWrongDirectionHandler = () => InvalidateCurrentLap("Wrong Direction");
        DirectionTracker.onWrongDirection += onWrongDirectionHandler;
    }

    private void OnDestroy()
    {
        if (onWrongDirectionHandler != null)
        {
            DirectionTracker.onWrongDirection -= onWrongDirectionHandler;
        }
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
        if (directionTracker.correctDirection){
        if (!lapRunning)
        {
            Debug.Log("Запуск нового круга");
            StartNewLap();
        }
        else
        {
            Debug.Log("Финиш круга");
            FinishLap();
            StartNewLap();
        }}
    else{
        CrossedFinishLineWrongSide?.Invoke();
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
    Debug.Log("Начало метода финиша");
    float now = Time.time;
    PrefsManager.Instance.SaveLapsAmount("Monza", PrefsManager.Instance.GetLapsAmount("Monza") + 1);


    int lastIndex = sectorCount - 1;

    Debug.Log("Проверка секторов");
    if (currentLapSectors != null &&
        lastIndex >= 0 && lastIndex < sectorCount &&
        currentLapSectors[lastIndex] <= 0f &&  
        lastSectorTimeStamp > 0f)
    {
        float lastSectorTime = now - lastSectorTimeStamp;
        RegisterSector(lastIndex, lastSectorTime);
    }

    float lapTime = now - lapStartTime;

    Debug.Log("Проверка валидности круга");
    if (!lapValid)
    {
        previousLapTime = lapTime;
        Debug.Log($"Lap {currentLapIndex} INVALID, time = {FormatTime(lapTime,"lap")}");
        return;
    }
    Debug.Log("Круг валиден");
    previousLapTime = lapTime;
    currentLapSectors.CopyTo(previousLapSectors, 0);
    Debug.Log("Проверка лучшего времени");
    if (lapValid&& (bestLapTime < 0f || lapTime < PrefsManager.Instance.GetBestTime("Monza"))){
    Debug.Log("Лучшее время найдено");
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
}
    Debug.Log("Обновление UI");
    Debug.Log($"Lap {currentLapIndex} FINISHED: {FormatTime(lapTime,"lap")}   Best: {FormatTime(bestLapTime,"lap")}");

    lapTimeText.text = FormatTime(lapTime,"lap");
    previousLapTimeText.text = FormatTime(previousLapTime,"lap");
    Debug.Log("Круг завершен: " + FormatTime(previousLapTime,"lap"));
}


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
    Debug.Log("Запуск загрузки лидеров");    
    yield return new WaitForSeconds(2f);
    monzaLeaderBoard.isLoaded = false;
    Debug.Log("Начало загрузки лидеров");
    monzaLeaderBoard.LoadLeaderboard();
}
}



