using UnityEngine;

public class Obstacles : MonoBehaviour
{
    public ParticleSystem particles;
    FollowCamera mainCamera;

    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<FollowCamera>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Instantiate(particles,transform.position,Quaternion.identity);
            mainCamera.Shake(0.5f, 0.3f);
            Destroy(gameObject);
        }
    }

}
