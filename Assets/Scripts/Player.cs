using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public bool isAlive;
    public bool onTrack;
    public float health;
    public Slider timerSlider;

    float timer;
    public LayerMask surfaceLayer;
    Rigidbody2D rb;

    void Start()
    {
        isAlive = true;
        rb = GetComponent<Rigidbody2D>();
        timerSlider = GameObject.Find("Slider").GetComponent<Slider>();
        timerSlider.value = health;

    }

    void Update()
    {

        if (rb.linearVelocity.magnitude < 5f)
        {
            health = Mathf.MoveTowards(health, 0, Time.deltaTime * 2);
            timerSlider.value = health;
        }

        if (rb.linearVelocity.magnitude >= 5f)
        {
            health = Mathf.MoveTowards(health, 5, Time.deltaTime * 1);
        }
        
        if (health <= 0)
        {
            isAlive = false;
            Debug.Log("You Died");
        }
        GetSurfaceBehavior();
    }


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
                default:
                onTrack = false;
                Debug.Log("No Surface");
                break;
            }

        }
    }


}
