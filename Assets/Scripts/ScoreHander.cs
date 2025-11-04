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






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        WatchDistance();
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
                sandDetected = true;
                Debug.Log("NearSand");
            }
            else if (hit.collider.tag == "obstacle")
            {
                obstacleDetected = true;
                Debug.Log("NearObstacle");
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
}
