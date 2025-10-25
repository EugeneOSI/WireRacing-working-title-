using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class TrackGenarator : MonoBehaviour
{
    [SerializeField] private GameObject[] trackSegments;
    [SerializeField] private GameObject firstSegmentInstance;
    private GameObject currentSegment;
    private GameObject prevSegment;
     void Start()
    {
        prevSegment  = firstSegmentInstance;
        GenerateSegment();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RemovePrevSegment();
            GenerateSegment();
        }

    }
    public void GenerateSegment()
    {
            GameObject segmentPrefab = trackSegments[Random.Range(0, trackSegments.Length)];
            currentSegment = Instantiate(segmentPrefab);
            AlignPositionAndRotation(currentSegment, prevSegment);
    }
    void AlignPositionAndRotation(GameObject currentSegment, GameObject prevSegment)
    {
        currentSegment.transform.position = prevSegment.transform.GetChild(0).position;
        currentSegment.transform.rotation = prevSegment.transform.GetChild(0).rotation;
    }
    public void RemovePrevSegment()
    {
        Destroy(prevSegment);
        prevSegment = currentSegment;
    }

}

