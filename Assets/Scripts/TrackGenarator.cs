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
        segments.Add(firstSegmentInstance);
        prevSegment = segments.Last();
        StartCoroutine(SpawnTrack());
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateSegment(1);
        }*/

    }

    public void GenerateSegment(int amount)
{
    int placed = 0;                 // сколько сегментов уже поставили в этом вызове
    int safety = 0;                 // на всякий случай, чтобы не зациклиться бесконечно
    int safetyCap = amount * 500;   // можно подправить под себя

    // гарантируем, что у нас есть стартовая точка
    if (segments.Count == 0)
    {
        segments.Add(firstSegmentInstance);
        prevSegment = segments.Last();
    }

    while (placed < amount && safety++ < safetyCap)
    {
        bool placedThisStep = false;

        // пробуем поставить следующий сегмент maxAttempts раз
        int attempt = 0;
        while (attempt++ < maxAttempts)
        {
            GameObject segmentsPrefab = trackSegments[Random.Range(0, trackSegments.Length)];
            currentSegment = Instantiate(segmentsPrefab);

            // ВАЖНО: не трогаем твою логику выравнивания
            AlignPositionAndRotation(currentSegment, prevSegment);

            if (!CheckSegmentsOverlap(currentSegment))
            {
                // успех — фиксируем сегмент и двигаем "хвост"
                segments.Add(currentSegment);
                prevSegment = segments.Last();
                placed++;
                placedThisStep = true;
                break;
            }

            // не подошёл — удаляем временный инстанс и пробуем другой
            Destroy(currentSegment);
        }

        if (placedThisStep)
            continue;

        // Если сюда дошли — на текущем узле варианты закончились.
        // Делаем БЭКТРЕКИНГ: удаляем последний установленный сегмент
        // и пробуем строить с предыдущего. Можно откатываться на любое число шагов.
        bool backtracked = false;
        while (segments.Count > 1) // не трогаем самый первый сегмент-опору
        {
            var last = segments.Last();
            Destroy(last);
            segments.RemoveAt(segments.Count - 1);
            prevSegment = segments.Last();  // «сдвигаем хвост» назад на один узел
            backtracked = true;

            // Выходим из внутреннего while — основной цикл снова попробует
            // подобрать сегмент из новой точки. Если опять упрёмся, сюда вернёмся
            // и откатимся ещё глубже.
            break;
        }

        if (!backtracked)
        {
            // Остался только первый сегмент — продолжать уже некуда.
            Debug.LogWarning("Track generation stuck: no valid continuation found.");
            break;
        }
    }
}

    /*public void GenerateSegment(int amount)
    {

        int currentAmount = 0;
        int attempt = 0;
        while (currentAmount < amount)
        {
            attempt++;
            GameObject segmentsPrefab = trackSegments[Random.Range(0, trackSegments.Length)];
            currentSegment = Instantiate(segmentsPrefab);
            AlignPositionAndRotation(currentSegment, prevSegment);
            if (!CheckSegmentsOverlap(currentSegment))
            {
                segments.Add(currentSegment);
                prevSegment = segments.Last();
            }
            if (CheckSegmentsOverlap(currentSegment) && attempt < maxAttempts)
            {
                Destroy(currentSegment);
                continue;
            }
            if (CheckSegmentsOverlap(currentSegment) && attempt >= maxAttempts)
            {
                Destroy(currentSegment);
                if (segments.Count > 0)
                {
                    Destroy(segments.Last());
                    segments.RemoveAt(segments.Count - 1);
                    prevSegment = segments.Last();
                }
            }
            currentAmount++;
            attempt = 0;
            continue;
        }
        


    }*/

    void AlignPositionAndRotation(GameObject currentSegment, GameObject prevSegment)
    {
        currentSegment.transform.position = prevSegment.transform.GetChild(0).position;
        Quaternion rotDelta = Quaternion.FromToRotation(currentSegment.transform.up, prevSegment.transform.GetChild(0).up);
        currentSegment.transform.rotation = rotDelta * currentSegment.transform.rotation;
    }

    bool CheckSegmentsOverlap(GameObject instSegment)
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
            GenerateSegment(5);
            yield return new WaitForSeconds(15);
            RemoveSegments(3);

        }
    }
}

