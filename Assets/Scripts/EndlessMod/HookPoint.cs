using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class HookPoint : MonoBehaviour
{
    float speed;
    Vector2 targPos;

    public float lifeTime;
    bool onMove = true;
    void Awake()
    {
        StartCoroutine(LifeTime());
    }
    private void Update()
    {
        transform.position = Vector2.Lerp(transform.position, targPos, speed*Time.deltaTime);
        Vector2 currentPos = new Vector2 (transform.position.x, transform.position.y);
        if ((currentPos - targPos).magnitude<0.01) onMove = false;
    }

    void FixedUpdate()
    {

    }

    public bool moveStatus
    {
        get
        {
            return onMove;
        }
    }

    public Vector2 targetPosition
    {
        get
        {
            return targPos;
        }
        set
        {
            targPos = value;
        }
    }

    public float flyingSpeed
    {
        set
        {
            speed = value;
        }
    }
    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
    }


}
