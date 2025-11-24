using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TimeTrialManager : MonoBehaviour
{
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
    public Image[] sectorImages;

    [Header("Game Objects")]
    [SerializeField] private PlayerTimeTrial player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentLapSectors  = new float[sectorCount];
        previousLapSectors = new float[sectorCount];
        ResetArray(previousLapSectors, -1f);
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
        lapStartTime = Time.time;
        lastSectorTimeStamp = lapStartTime;

        currentLapSectors = new float[sectorCount];
        currentSectorIndex = -1;
        StartCoroutine(ResetSectorsUI());
    }

void FinishLap()
{
    float now = Time.time;

    // --- ДОПИСЫВАЕМ ПОСЛЕДНИЙ СЕКТОР, ЕСЛИ ЕГО ЕЩЕ НЕТ ---
    int lastIndex = sectorCount - 1;

    if (currentLapSectors != null &&
        lastIndex >= 0 && lastIndex < sectorCount &&
        currentLapSectors[lastIndex] <= 0f &&     // не записан
        lastSectorTimeStamp > 0f)
    {
        float lastSectorTime = now - lastSectorTimeStamp;
        RegisterSector(lastIndex, lastSectorTime);
    }
    // ---------------------------------------------------

    float lapTime = now - lapStartTime;

    if (!lapValid)
    {
        Debug.Log($"Lap {currentLapIndex} INVALID, time = {FormatTime(lapTime,"lap")}");
        return;
    }

    previousLapTime = lapTime;
    currentLapSectors.CopyTo(previousLapSectors, 0);

    if (bestLapTime < 0f || lapTime < bestLapTime)
        bestLapTime = lapTime;

    Debug.Log($"Lap {currentLapIndex} FINISHED: {FormatTime(lapTime,"lap")}   Best: {FormatTime(bestLapTime,"lap")}");

    lapTimeText.text = FormatTime(lapTime,"lap");
    bestLapTimeText.text = FormatTime(bestLapTime,"lap");
    previousLapTimeText.text = FormatTime(previousLapTime,"lap");
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
        // UI: показать сообщение, покрасить таймер в красный и т.п.
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

    // тут обновляешь UI сектора
    // TimeTrialUI.Instance.UpdateSector(sectorIndex, sectorTime, sectorDelta, totalDelta);

    Debug.Log($"Sector {sectorIndex}: {sectorTime:F3}  ΔS={sectorDelta:F3}  ΔT={totalDelta:F3}");

    sectorImages[sectorIndex].color = sectorDelta < 0f ? fasterColor : (sectorDelta > 0f ? slowerColor : neutralColor);
    sectorDeltaText.text = FormatTime(sectorDelta,"delta");
    totalDeltaText.text = FormatTime(totalDelta,"delta");
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

    IEnumerator ResetSectorsUI()
    {
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < sectorCount; i++)
        {
            sectorImages[i].color = neutralColor;

        }
        sectorDeltaText.text = "--.---";
        totalDeltaText.text = "--.---";
    }
}
