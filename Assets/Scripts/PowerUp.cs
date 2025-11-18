using UnityEngine;

public class PowerUp : MonoBehaviour
{
    Player player;
    Rigidbody2D powerUpRb;

    public float attractionForce;

    bool isAttracting = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        powerUpRb = transform.parent.GetComponent<Rigidbody2D>();
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
        Vector2 powerUpPos = transform.position;
        Vector2 direction = (playerPos - powerUpPos).normalized;
        powerUpRb.AddForce(direction * attractionForce);
    }
}
