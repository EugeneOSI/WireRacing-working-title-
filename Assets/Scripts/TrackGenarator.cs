using UnityEngine;
using System.Collections;
public class TrackGenarator : MonoBehaviour
{
    public GameObject[] trackSegments;
     [SerializeField] private GameObject firstSegmentInstance;

    private GameObject prevSegment; 

    void Start()
    {
        prevSegment = firstSegmentInstance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateSegment();
        }

    }
    
    public void GenerateSegment()
    {
        GameObject prefabSegment = trackSegments[Random.Range(0, trackSegments.Length)];
        GameObject instSegment = Instantiate(prefabSegment);
        instSegment.transform.position = prevSegment.transform.GetChild(0).position;
        Quaternion rotDelta = Quaternion.FromToRotation(instSegment.transform.up, prevSegment.transform.GetChild(0).up);
        instSegment.transform.rotation = rotDelta * instSegment.transform.rotation;
        prevSegment = instSegment;
    }
}
