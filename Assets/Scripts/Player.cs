using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isAlive;

    public bool onTrack;

    public float health;
    void Start()
    {
        isAlive = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("track"))
        {
            onTrack = false;
            Debug.Log("Off Track");
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("track"))
        {
            onTrack = true;
            Debug.Log("On Track");
        }
    }

}
