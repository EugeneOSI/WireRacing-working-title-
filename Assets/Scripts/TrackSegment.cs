using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class TrackSegment : MonoBehaviour
{
    TrackGenarator trackGenarator;
    GameObject parentSegment;
    
    void Start()
    {
        trackGenarator = GameObject.Find("TrackGenerator").GetComponent<TrackGenarator>();
        parentSegment = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            trackGenarator.GenerateSegment(2);
            StartCoroutine(DestroyThisSegment());
        }
    }

    IEnumerator DestroyThisSegment()
    {

        yield return new WaitForSeconds(3f);
        Destroy(parentSegment.gameObject);
    }
}
