using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class TrackGenarator : MonoBehaviour
{
    [SerializeField] private GameObject[] trackSegments;
    [SerializeField] private GameObject firstSegmentInstance;
    
    private GameObject currentSegment;
    private GameObject prevSegment;

    private SpawnManager spawnManager;
    
     void Start()
    {
        prevSegment = firstSegmentInstance;
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        GenerateSegment();
    }

    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            RemovePrevSegment();
            GenerateSegment();
        }*/

    }
    public void GenerateSegment()
    {
            GameObject segmentPrefab = trackSegments[Random.Range(0, trackSegments.Length)];
            currentSegment = Instantiate(segmentPrefab);
            AlignPositionAndRotation(currentSegment, prevSegment);
            spawnManager.SpawnObstacles(currentSegment.transform.GetChild(1).GetComponent<SplineContainer>());
    }
    void AlignPositionAndRotation(GameObject currentSegment, GameObject prevSegment)
    {
        currentSegment.transform.position = prevSegment.transform.GetChild(0).position;
        currentSegment.transform.rotation = prevSegment.transform.GetChild(0).rotation;
    }
    public void RemovePrevSegment()
    {
        spawnManager.ClearLastObstacles();
        Destroy(prevSegment);
        prevSegment = currentSegment;
    }

    public GameObject GetCurrentSegment
    {
        get
        {
            return currentSegment;
        }
    }
        public GameObject GetPrevSegment
    {
        get
        {
            return prevSegment;
        }
    }

}

