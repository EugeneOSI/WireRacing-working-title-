using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class TrackGenarator : MonoBehaviour
{

    int maxAttempts = 10;
    public GameObject[] trackSegments;
    GameObject currentSegment;
    [SerializeField] private GameObject firstSegmentInstance;

    private GameObject prevSegment;

    private List<GameObject> segments = new List<GameObject>();

    void Start()
    {
        prevSegment = firstSegmentInstance;
        //segments.Add(firstSegmentInstance);
        //prevSegment = segments.Last();
        GenerateSegment();
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateSegment(1);
        }*/

    }
    /*public void GenerateSegment(int amount)
    {
        // Память о перебранных вариантах из КАЖДОЙ точки (узла).
        // Индекс в списке соответствует индексу в segments.
        List<HashSet<int>> triedPerNode = new List<HashSet<int>>();
        for (int i = 0; i < segments.Count; i++)
            triedPerNode.Add(new HashSet<int>());

        int placed = 0;
        int safety = 0;
        int safetyCap = Mathf.Max(2000, amount * 200);

        // Гарантируем, что есть стартовый узел
        if (segments.Count == 0)
        {
            segments.Add(firstSegmentInstance);
            prevSegment = segments.Last();
            triedPerNode.Add(new HashSet<int>());
        }

        while (placed < amount && safety++ < safetyCap)
        {
            // Узел, из которого сейчас строим продолжение
            int nodeIndex = segments.Count - 1;
            var tried = triedPerNode[nodeIndex];

            // Если из этого узла уже перепробовали всё — ОТКАТ
            if (tried.Count >= trackSegments.Length || (maxAttempts > 0 && tried.Count >= maxAttempts))
            {
                // нельзя удалять самый первый базовый сегмент
                if (segments.Count <= 1)
                {
                    Debug.LogWarning("Track generation stuck: no valid continuation from start.");
                    break;
                }

                // удаляем последний установленный сегмент и узел его попыток
                Destroy(segments.Last());
                segments.RemoveAt(segments.Count - 1);
                triedPerNode.RemoveAt(triedPerNode.Count - 1);
                prevSegment = segments.Last();
                continue; // продолжаем откатываться, пока не найдём узел с оставшимися вариантами
            }

            // Выберем следующий НЕиспользованный префаб
            int prefabIndex;
            // простая попытка найти неиспользованный индекс
            int guard = 0;
            do
            {
                prefabIndex = Random.Range(0, trackSegments.Length);
                guard++;
                if (guard > trackSegments.Length * 3) break; // страховка от редких бесконечностей
            } while (tried.Contains(prefabIndex));

            // помечаем, что этот префаб уже пробуем в данном узле
            tried.Add(prefabIndex);

            // Пробуем поставить
            GameObject prefab = trackSegments[prefabIndex];
            currentSegment = Instantiate(prefab);

            // ТВОЙ способ стыковки/поворота — не трогаем
            AlignPositionAndRotation(currentSegment, prevSegment);

            if (!CheckSegmentsOverlap(currentSegment))
            {
                // успех — фиксируем сегмент, создаём новый узел с пустым набором попыток
                segments.Add(currentSegment);
                triedPerNode.Add(new HashSet<int>()); // новый узел начинается "чистым"
                prevSegment = currentSegment;
                placed++;
            }
            else
            {
                // не подошло — удаляем временный инстанс и цикл продолжит подбирать другие варианты
                Destroy(currentSegment);
                // ВАЖНО: в tried уже записано, повторно этот же префаб из этой точки не будет выбран
            }
        }

        if (safety >= safetyCap)
            Debug.LogWarning("Safety cap reached during generation.");
    }*/

    /*public void GenerateSegment(int amount)
{
    int placed = 0;                 
    int safety = 0;                   

    if (segments.Count == 0)
    {
        segments.Add(firstSegmentInstance);
        prevSegment = segments.Last();
    }

    while (placed < amount)
    {
        bool placedThisStep = false;

        int attempt = 0;
        safety++;
        
        while (attempt < maxAttempts)
        {
            GameObject segmentsPrefab = trackSegments[Random.Range(0, trackSegments.Length)];
            currentSegment = Instantiate(segmentsPrefab);

     
            AlignPositionAndRotation(currentSegment, prevSegment);

            if (!CheckSegmentsOverlap(currentSegment))
            {
                
                segments.Add(currentSegment);
                prevSegment = segments.Last();
                placed++;
                placedThisStep = true;
                break;
            }

            Destroy(currentSegment);
            attempt++;
        }

        if (placedThisStep)
            continue;

        bool backtracked = false;
        while (segments.Count > 1) 
        {
            var last = segments.Last();
            Destroy(last);
            segments.RemoveAt(segments.Count - 1);
            prevSegment = segments.Last();  
            backtracked = true;

            break;
        }

        if (!backtracked)
        {

            Debug.LogWarning("Track generation stuck: no valid continuation found.");
            break;
        }
    }
}*/


    public void GenerateSegment()
    {
        GameObject segmentPrefab = trackSegments[Random.Range(0, trackSegments.Length)];
        currentSegment = Instantiate(segmentPrefab);
        AlignPositionAndRotation(currentSegment, prevSegment);
        
    }
    void AlignPositionAndRotation(GameObject currentSegment, GameObject prevSegment)
    {
        currentSegment.transform.position = prevSegment.transform.GetChild(0).position;
        Quaternion rotDelta = Quaternion.FromToRotation(currentSegment.transform.up, prevSegment.transform.GetChild(0).up);
        currentSegment.transform.rotation = rotDelta * currentSegment.transform.rotation;
    }

    /*bool CheckSegmentsOverlap(GameObject instSegment)
    {
        var col = instSegment.transform.GetChild(1).GetComponent<Collider2D>();

        Physics2D.SyncTransforms();

        var filter = new ContactFilter2D
        {
            useTriggers = true,
            layerMask = LayerMask.GetMask("TrackSegment"),
            useLayerMask = true
        };
        var results = new List<Collider2D>(8);
        int hits = col.Overlap(filter, results);

        if (hits > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }*/
    public void RemovePrevSegment()
    {
        Destroy(prevSegment);
        prevSegment = currentSegment;
    }
    void RemoveSegments(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Destroy(segments[i]);
        }
        segments.RemoveRange(0, amount);
    }
    
    IEnumerator SpawnTrack()
    {
        while (true)
        {
            GenerateSegment();
            yield return new WaitForSeconds(1);
            RemoveSegments(3);

        }
    }
}

