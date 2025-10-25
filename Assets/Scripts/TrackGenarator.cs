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
    private GameObject nextSegment;

    private List<GameObject> segments = new List<GameObject>();

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
            Debug.Log($"PrevSegment: {prevSegment.name}, currentSegment: {currentSegment.name}");
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
        Quaternion rotDelta = Quaternion.FromToRotation(currentSegment.transform.up, prevSegment.transform.GetChild(0).up);
        currentSegment.transform.rotation = rotDelta * currentSegment.transform.rotation;
    }

    public void RemovePrevSegment()
    {
        Destroy(prevSegment);
        prevSegment = currentSegment;
    }

}

