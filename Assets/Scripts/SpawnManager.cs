using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using Unity.VisualScripting;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] obstacles;
    public LayerMask layerMask;
    public float trackWidth = 2.5f;
    public int obstacleCount = 5;

    List<GameObject> obstaclesInstances = new List<GameObject>();


    public void SpawnObstacles(SplineContainer splineContainer)
    {
        if (splineContainer == null || obstacles.Length == 0)
        {
            Debug.LogWarning("Spline or obstacle prefabs not assigned!");
            return;
        }

        Spline spline = splineContainer.Spline;
        
        for (int i = 0; i < obstacleCount; i++)
        {
            float t = UnityEngine.Random.value;

            // --- LOCAL space ---
            float3 posL = spline.EvaluatePosition(t);
            float3 tanL = math.normalize(spline.EvaluateTangent(t));

            // --- to WORLD space ---
            Vector3 posW = splineContainer.transform.TransformPoint((Vector3)posL);
            Vector3 tanW = splineContainer.transform.TransformDirection((Vector3)tanL).normalized;

            // 2D-нормаль в мире (перпендикуляр к направлению)
            Vector3 normalW = new Vector3(-tanW.y, tanW.x, 0f).normalized;

            // случайное смещение внутрь ширины (чуть меньше половины)
            float offset = UnityEngine.Random.Range(-trackWidth * 0.45f, trackWidth * 0.45f);
            Vector3 spawnPos = posW + normalW * offset;

            //if (!Physics2D.OverlapPoint(spawnPos, layerMask)) { i--; continue; }

            // Ориентация по траектории (для 2D)
            Quaternion rot = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));

            var prefab = obstacles[UnityEngine.Random.Range(0, obstacles.Length)];

            GameObject instance = Instantiate(prefab, spawnPos, rot);
            obstaclesInstances.Add(instance);
            
        }
    }

    public void ClearLastObstacles()
    {
        //Debug.Log(obstaclesInstances.Count);
        if (obstaclesInstances.Count > obstacleCount)
        {
            for (int i = 0; i<obstaclesInstances.Count; i++)
            {
                Destroy(obstaclesInstances[0]);
                obstaclesInstances.RemoveAt(0);
            }
        }

    } 
}
