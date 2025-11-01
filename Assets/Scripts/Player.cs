using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
public class Player : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject hookPoint;
    private GameObject tmpHookPoint;

    [Header("States")]
    public bool isAlive;
    public bool onTrack;
    private bool startRace;
    private bool hookIsMoving;
    private bool hitObstacle;

    [Header("Parameters")]
    public float health;
    public float hookSpeed;
    public float limitedSpeed;
    public float attractionForce;
    public float minSpeed;
    public float breakForce;
    public float maxHooksAmount;
    private float currentHooksAmount;
    private float maxCheckDistance = 10;
    private float maxVelocity;

    [Header("Sliders")]
    public Slider timerSlider;
    public Slider hookSlider;

    [Header("Surfaces")]
    public LayerMask surfaceLayer;
    public LayerMask roadEdgeMask;

    [Header("Components")]
    private Rigidbody2D playerRb;
    private LineRenderer lineRenderer;
    private Collider2D playerCol;
    private ContactFilter2D contactFilter;
    private Collider2D[] surfaceCollidersHit = new Collider2D[10];

    Surface.SurfaceType drivingSurface = Surface.SurfaceType.Road;
    
    

    void Start()
    {
        contactFilter = new ContactFilter2D();
        contactFilter.layerMask = surfaceLayer;
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = true;
        
        timerSlider.value = health;
        //currentHooksAmount = 0;
        
        hookIsMoving = false;
        startRace = false;
        isAlive = true;
        hitObstacle = false;

        playerRb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        playerCol = GetComponent<Collider2D>(); 
    }

    void Update()
    {

        CheckDistance();
        CheckCurrentSpeed();
        //HookCooldown();

        if (Input.GetMouseButtonDown(0) /*&& currentHooksAmount < maxHooksAmount*/)
        {
            ResetWirePosition();
            ThrowHookPoint();
        }
        if (health <= 0)
        {
            isAlive = false;
            Debug.Log("You Died");
        }

        if (tmpHookPoint != null)
        {
            HookPoint hookPoint = tmpHookPoint.GetComponent<HookPoint>();
            DrawWire();
            if (!hookPoint.moveStatus) hookIsMoving = false;
        }
    }


    void FixedUpdate()
    {
        GetSurfaceBehavior();

        if (tmpHookPoint != null && !hookIsMoving)
        {
            Move();
        }
    }

    void CheckDistance()
    {
        RaycastHit2D upHit = Physics2D.Raycast(transform.position, Vector2.up, maxCheckDistance, roadEdgeMask);
        RaycastHit2D downHit = Physics2D.Raycast(transform.position, Vector2.down, maxCheckDistance, roadEdgeMask);

        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, maxCheckDistance, roadEdgeMask);
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, maxCheckDistance, roadEdgeMask);

        RaycastHit2D upRightHit = Physics2D.Raycast(transform.position, new Vector2(1, 1).normalized, maxCheckDistance, roadEdgeMask);
        RaycastHit2D upLeftHit = Physics2D.Raycast(transform.position, new Vector2(-1, 1).normalized, maxCheckDistance, roadEdgeMask);

        RaycastHit2D downRightHit = Physics2D.Raycast(transform.position, new Vector2(1, -1).normalized, maxCheckDistance, roadEdgeMask);
        RaycastHit2D downLeftHit = Physics2D.Raycast(transform.position, new Vector2 (-1,-1).normalized, maxCheckDistance, roadEdgeMask);


        float upDist = upHit.collider ? upHit.distance : maxCheckDistance;
        float downDist = downHit.collider ? downHit.distance : maxCheckDistance;

        float rightDist = rightHit.collider ? rightHit.distance : maxCheckDistance;
        float leftDist = leftHit.collider ? leftHit.distance : maxCheckDistance;

        float upRightDist = upRightHit.collider ? upRightHit.distance : maxCheckDistance;
        float upLeftDist = upLeftHit.collider ? upLeftHit.distance : maxCheckDistance;

        float downRightDist = downRightHit.collider ? downRightHit.distance : maxCheckDistance;
        float downLeftDist = downLeftHit.collider ? downLeftHit.distance : maxCheckDistance;


        Debug.DrawRay(transform.position, Vector2.up * upDist, Color.red);
        Debug.DrawRay(transform.position, Vector2.down * downDist, Color.red);

        Debug.DrawRay(transform.position, Vector2.right * rightDist, Color.red);
        Debug.DrawRay(transform.position, Vector2.left * leftDist, Color.red);

        Debug.DrawRay(transform.position, new Vector2(1, 1).normalized * upRightDist, Color.red);
        Debug.DrawRay(transform.position, new Vector2(-1, 1).normalized * upLeftDist, Color.red);

        Debug.DrawRay(transform.position, new Vector2(1, -1).normalized * downRightDist, Color.red);
        Debug.DrawRay(transform.position, new Vector2 (-1,-1).normalized * downLeftDist, Color.red);

        if (leftDist <= 1 || rightDist <= 1 || rightDist <=1 || leftDist <= 1 || upRightDist <=1 || upLeftDist <= 1 || downRightDist <= 1 || downLeftDist <=1)
        {
            Debug.Log("По краю!");
        }
    }
    void ThrowHookPoint()
    {
        Vector3 hookTargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hookTargetPos.z = 0;

        tmpHookPoint = Instantiate(hookPoint, transform.position, Quaternion.identity);

        HookPoint hookPointSetting = tmpHookPoint.GetComponent<HookPoint>();
        hookPointSetting.targetPosition = hookTargetPos;
        hookPointSetting.flyingSpeed = hookSpeed;

        hookIsMoving = true;
        currentHooksAmount++;
    }
    void HookCooldown()
    {
        hookSlider.value = currentHooksAmount;
        if (currentHooksAmount > 0)
        {
            currentHooksAmount-= Time.deltaTime;
        }
    }

    void Move()
    {
        float newAttractionForce = attractionForce;
        if (!hitObstacle)
        {
            switch (drivingSurface)
            {
                case Surface.SurfaceType.Sand:
                    newAttractionForce = attractionForce / 2;
                    maxVelocity = limitedSpeed;
                    health = 0;
                    break;
                case Surface.SurfaceType.Road:
                    maxVelocity = 1000;
                    break;
            }
        }
        if (hitObstacle)
        {
            maxVelocity = limitedSpeed;
        }
        Vector2 playerMoveVector = (tmpHookPoint.transform.position - transform.position).normalized;
        playerRb.AddForce(playerMoveVector * newAttractionForce, ForceMode2D.Force);
        if (playerRb.linearVelocity.magnitude > maxVelocity)
        {
            float currentVelocity = playerRb.linearVelocity.magnitude;
            playerRb.linearVelocity = playerRb.linearVelocity.normalized * Mathf.MoveTowards(currentVelocity, maxVelocity, Time.deltaTime * breakForce);
        }

    }
    
    void ResetWirePosition()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
    }
    void DrawWire()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, tmpHookPoint.transform.position);
    }

    void GetSurfaceBehavior()
    {
        int numberOfHits = Physics2D.OverlapCollider(playerCol, contactFilter, surfaceCollidersHit);

        float lastSurfaceZValue = -1000;
        for (int i = 0; i < numberOfHits; i++)
        {
            Surface surface = surfaceCollidersHit[i].GetComponent<Surface>();
            if (surface.transform.position.z > lastSurfaceZValue)
            {
                drivingSurface = surface.surfaceType;
                lastSurfaceZValue = surface.transform.position.z;
            }
        }

        if (numberOfHits == 0)
        {
            drivingSurface = Surface.SurfaceType.Road;
        }

    }

    void CheckCurrentSpeed()
    {
        if (playerRb.linearVelocity.magnitude < minSpeed && startRace)
        {
            health = Mathf.MoveTowards(health, 0, Time.deltaTime * 2);
            timerSlider.value = health;
        }

        if (playerRb.linearVelocity.magnitude >= minSpeed && startRace)
        {
            health = Mathf.MoveTowards(health, 5, Time.deltaTime * 1);
            timerSlider.value = health;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("StartRaceZone"))
        {
            startRace = true;
        }

        if (collision.CompareTag("Obstacle"))
        {
            StartCoroutine(HitObstacle());
        }
    }

    IEnumerator HitObstacle()
    {
        hitObstacle = true;
        yield return new WaitForSeconds(2);
        hitObstacle = false;
    }
}
