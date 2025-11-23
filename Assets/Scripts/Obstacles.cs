using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public ParticleSystem particles;

    void Start()
    {
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Instantiate(particles,transform.position,Quaternion.identity);
            Destroy(gameObject);
        }
    }

}
