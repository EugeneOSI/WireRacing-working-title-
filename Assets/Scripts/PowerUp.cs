using UnityEngine;

public class PowerUp : MonoBehaviour
{
    Player player;
    [SerializeField] Transform powerUpTransform;

    public float attractionForce;

    bool isAttracting = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttracting)
        {
            AttractToPlayer();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAttracting = true;
        }
    }

    void AttractToPlayer(){
        Vector2 playerPos = player.transform.position;
        powerUpTransform.position = Vector2.MoveTowards(powerUpTransform.position, playerPos, attractionForce * Time.deltaTime);
    }
}
