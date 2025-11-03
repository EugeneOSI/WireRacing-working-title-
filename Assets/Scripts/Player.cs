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
    private float maxVelocity;
    private float currentSpeed;

    [Header("Sliders")]
    public Slider timerSlider;

    [Header("Surfaces")]
    public LayerMask surfaceLayer;

    [Header("Components")]
    private Rigidbody2D playerRb;
    private LineRenderer lineRenderer;
    private Collider2D playerCol;
    private ContactFilter2D contactFilter;
    private Collider2D[] surfaceCollidersHit = new Collider2D[10];
    private FollowCamera mainCamera;

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
        mainCamera = GameObject.Find("Main Camera").GetComponent<FollowCamera>();
    }

    void Update()
    {
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
    /*void HookCooldown()
    {
        hookSlider.value = currentHooksAmount;
        if (currentHooksAmount > 0)
        {
            currentHooksAmount-= Time.deltaTime;
        }
    }*/

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
                    //health = 0;
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

        currentSpeed = playerRb.linearVelocity.magnitude;
        if (currentSpeed < minSpeed && startRace)
        {
            health = Mathf.MoveTowards(health, 0, Time.deltaTime * 2);
            timerSlider.value = health;
        }

        if (currentSpeed >= minSpeed && startRace)
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
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            StartCoroutine(HitObstacle());
        }
        if (collision.CompareTag("sand"))
        {
            mainCamera.Shake(1f, 0.05f);
        }
    }

    public float GetCurrentSpeed
    {
        get
        {
            return currentSpeed;
        }
    }
    IEnumerator HitObstacle()
    {
        hitObstacle = true;
        yield return new WaitForSeconds(2);
        hitObstacle = false;
    }
}
