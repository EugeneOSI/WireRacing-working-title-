using UnityEngine;
using UnityEngine.Splines;
using System;

public class SplineDirectionTracker : MonoBehaviour
{
    [SerializeField] private SplineContainer spline;
    [SerializeField] private Transform player;

    // Насколько маленькое изменение t мы игнорируем (чтобы не дрожало)
    [SerializeField] private float deadZone = 0.0001f;

    // Событие: игрок едет в обратную сторону
    public static event Action OnWrongDirection;
    public static event Action OnCorrectDirection;

    private float previousT;
    private bool hasPrevious;

    private void Update()
    {
        if (spline == null || player == null)
            return;

        float currentT = GetPlayerT();
        Debug.Log("currentT: " + currentT);

        if (!hasPrevious)
        {
            previousT = currentT;
            hasPrevious = true;
            return;
        }

        float delta = GetDeltaT(previousT, currentT);
        

        if (Mathf.Abs(delta) > deadZone)
        {
            // ❗ считаем, что движение "вперёд" — это увеличение t
            if (delta < 0f)
            {
                //OnWrongDirection?.Invoke();
            }
            //else OnCorrectDirection?.Invoke();
            
        }

        previousT = currentT;
    }

    private float GetPlayerT()
    {
        SplineUtility.GetNearestPoint(
            spline.Spline,
            player.position,
            out _,
            out float t
        );

        return t;
    }

    // Корректная дельта t для ЗАМКНУТОГО сплайна
    private float GetDeltaT(float from, float to)
    {
        float delta = to - from;

        // переход через 0 -> 1
        if (delta > 0.5f)
            delta -= 1f;

        // переход через 1 -> 0
        if (delta < -0.5f)
            delta += 1f;

        return delta;
    }
}
