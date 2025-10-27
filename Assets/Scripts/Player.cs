using UnityEngine;
using System.Linq;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public float minSpeed;
    public bool isAlive;
    public bool onTrack;
    public float health;
    public Slider timerSlider;
    public LayerMask surfaceLayer;
    Rigidbody2D rb;
    TrackGenarator trackGenarator;

    private bool startRace;

    void Start()
    {
        startRace = false;
        isAlive = true;
        rb = GetComponent<Rigidbody2D>();
        timerSlider.value = health;
        trackGenarator = GameObject.Find("TrackGenerator").GetComponent<TrackGenarator>();

    }

    void Update()
    {

        CheckCurrentSpeed();

        if (health <= 0)
        {
            isAlive = false;
            Debug.Log("You Died");
        }

    }   
    void FixedUpdate()
    {
        GetSurfaceBehavior();
    }
    void GetSurfaceBehavior()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, surfaceLayer);
        if (hit.collider != null)
        {
            switch (hit.collider.tag)
            {
                case "track":
                    onTrack = true;
                    break;
                case "sand":
                    onTrack = false;
                    health = 0;
                    break;
            }
        }
    }

    void CheckCurrentSpeed()
    {
        if (rb.linearVelocity.magnitude < minSpeed && startRace)
        {
            health = Mathf.MoveTowards(health, 0, Time.deltaTime * 2);
            timerSlider.value = health;
        }

        if (rb.linearVelocity.magnitude >= minSpeed && startRace)
        {
            health = Mathf.MoveTowards(health, 5, Time.deltaTime * 1);
            timerSlider.value = health;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SegmentCheckPoint"))
        {
            trackGenarator.RemovePrevSegment();
            trackGenarator.GenerateSegment();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("StartRaceZone"))
        {
            startRace = true;
        }
    }




}
