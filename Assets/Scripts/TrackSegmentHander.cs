using UnityEngine;

public class TrackSegmentHander : MonoBehaviour
{
    TrackGenarator trackGenarator;
    void Awake()
    {
        trackGenarator = GameObject.Find("TrackGenerator").GetComponent<TrackGenarator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            trackGenarator.RemovePrevSegment();
            trackGenarator.GenerateSegment();
        }
    }

}
