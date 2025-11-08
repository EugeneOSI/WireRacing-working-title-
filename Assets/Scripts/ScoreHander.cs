using System.Collections.Generic;
using UnityEngine;

public class ScoreHander : MonoBehaviour
{
    public LayerMask roadEdgeMask;
    private float maxCheckDistance = 2f;

    Rigidbody2D playerRb;
    float distance;

    public bool nearSand;
    public bool nearObstacle;

    public bool highSpeed;

    public System.Action OnNearObstaclePulse;     // проезд мимо препятствия (одноразовый)
    public System.Action OnLeftRoad;              // вылет с трассы (сброс непрерывных)
    public System.Action OnObstacleHit;           // удар о препятствие

    bool prevNearObstacle = false;
    bool hadCollisionWhileNearObstacle = false;






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        WatchDistance();
        if (playerRb.linearVelocity.magnitude >= 20) highSpeed = true;
        else highSpeed = false;

        if (prevNearObstacle && !nearObstacle)
    {
        if (!hadCollisionWhileNearObstacle)
            OnNearObstaclePulse?.Invoke();
        hadCollisionWhileNearObstacle = false;
    }
    prevNearObstacle = nearObstacle;

        //distance += playerRb.linearVelocity.magnitude * Time.deltaTime;
    }

    void WatchDistance()
    {


        RaycastHit2D[] hits = {
            Physics2D.Raycast(transform.position, Vector2.up, maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, Vector2.down, maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, Vector2.right, maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, Vector2.left, maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, new Vector2(1, 1).normalized, maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, new Vector2(-1, 1).normalized, maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, new Vector2(1, -1).normalized, maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, new Vector2(-1, -1).normalized, maxCheckDistance, roadEdgeMask),
        };

        bool sandDetected = false;
        bool obstacleDetected = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;

            if (hit.collider.tag == "sand")
            {
                NotifyLeftRoad();
                sandDetected = true;
                //Debug.Log("NearSand");
            }
            else if (hit.collider.tag == "Obstacle")
            {
                obstacleDetected = true;
                //Debug.Log("NearObstacle");
            }
        }
        nearSand = sandDetected;
        nearObstacle = obstacleDetected;


        Debug.DrawRay(transform.position, Vector2.up * maxCheckDistance, Color.red);
        Debug.DrawRay(transform.position, Vector2.down * maxCheckDistance, Color.red);

        Debug.DrawRay(transform.position, Vector2.right * maxCheckDistance, Color.red);
        Debug.DrawRay(transform.position, Vector2.left * maxCheckDistance, Color.red);

        Debug.DrawRay(transform.position, new Vector2(1, 1).normalized * maxCheckDistance, Color.red);
        Debug.DrawRay(transform.position, new Vector2(-1, 1).normalized * maxCheckDistance, Color.red);

        Debug.DrawRay(transform.position, new Vector2(1, -1).normalized * maxCheckDistance, Color.red);
        Debug.DrawRay(transform.position, new Vector2(-1, -1).normalized * maxCheckDistance, Color.red);

    }
    
    // вызови это из OnCollisionEnter2D/OnTriggerEnter2D (там где у тебя фиксируется удар):
       public void NotifyObstacleHit()
{
    OnObstacleHit?.Invoke();
    if (nearObstacle) hadCollisionWhileNearObstacle = true;
}

// вызови это, когда фиксируешь «вылет с трассы» (например, ray’и вообще не находят дорогу или твой собственный флаг offRoad):
       public void NotifyLeftRoad()
{
    OnLeftRoad?.Invoke();
}
    public float Distance
    {
        get { return distance; }
    }

    public bool NearSand
    {
        get { return nearSand; }
    }

    public bool NearObstacle
    {
        get { return nearObstacle; }
    }

    public bool HighSpeed
    {
        get { return highSpeed; }
    }
}
