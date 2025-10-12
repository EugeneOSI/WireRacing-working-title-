using UnityEngine;
using System.Linq;
public class Player : MonoBehaviour
{
    public bool isAlive;
    public bool onTrack;
    public float health;
    public LayerMask surfaceLayer;

    void Start()
    {
        isAlive = true;
    }

    void Update()
    {
        
        GetSurfaceBehavior();
    }

    /*void OnTriggerExit2D(Collider2D collision)
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
        if (collision.CompareTag("sand"))
        {
            onTrack = false;
            Debug.Log("On Sand");
        }
    }*/

    void GetSurfaceBehavior()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, surfaceLayer);
        if (hit.collider != null)
        {
            switch (hit.collider.tag)
            {
                case "track":
                    onTrack = true;
                    Debug.Log("On Track");
                    break;
                case "sand":
                    onTrack = false;
                    Debug.Log("On Sand");
                    break;
            }
        }
    }


}
