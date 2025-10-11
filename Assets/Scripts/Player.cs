using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isAlive;

    public float health;
    void Start()
    {
        isAlive = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

}
