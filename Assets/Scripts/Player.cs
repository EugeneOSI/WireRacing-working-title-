using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Splines;
public class Player : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject hookPoint;
    private GameObject tmpHookPoint;
    [SerializeField] private GameObject powerUp;
    [SerializeField] private GameObject playerSprite;

    [Header("Splines")]
    [SerializeField] private SplineContainer firstSplineContainer;
    private SplineContainer splineContainer;
    private Spline _spline;
    private float previousT;
    private float currentT;

    [Header("States")]
    public bool isAlive;
    public bool onTrack;
    private bool startRace;
    private bool hookIsMoving;
    public bool hitObstacle;
    public bool withPowerUp;

    [Header("Parameters")]
    public float health;
    public float hookSpeed;
    public float limitedSpeed;
    public float attractionForce;
    public float minSpeed;
    public float breakForce;
    public float maxHooksAmount;
    private float maxVelocity;
    private float currentSpeed;

    [Header("Sliders")]
    //public Slider timerSlider;

    [Header("Surfaces")]
    public LayerMask surfaceLayer;

    [Header("Components")]
    private Rigidbody2D playerRb;
    private LineRenderer lineRenderer;
    private Collider2D playerCol;
    private ContactFilter2D contactFilter;
    private Collider2D[] surfaceCollidersHit = new Collider2D[10];
    private FollowCamera mainCamera;
    private ScoreEventsHander scoreEventsHander;
    private ScoreManager scoreManager;

    private GameManager gameManager;

    Surface.SurfaceType drivingSurface = Surface.SurfaceType.Road;



    void Start()
    {
        contactFilter = new ContactFilter2D();
        contactFilter.layerMask = surfaceLayer;
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = true;

        //timerSlider.value = health;
        //currentHooksAmount = 0;

        hookIsMoving = false;
        startRace = false;
        isAlive = true;
        hitObstacle = false;
        withPowerUp = false;

        powerUp.SetActive(false);
        SetSplineContainer(firstSplineContainer);

        scoreEventsHander = GetComponent<ScoreEventsHander>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerRb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        playerCol = GetComponent<Collider2D>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<FollowCamera>();
    }

    void Update()
    {
        CheckCurrentSpeed();
        transform.rotation = Quaternion.Euler(0, 0, 0);
        playerSprite.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(playerRb.linearVelocity.y, playerRb.linearVelocity.x) * Mathf.Rad2Deg);
        
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

    public void SetSplineContainer(SplineContainer splineContainer){

            this.splineContainer = splineContainer;
            _spline = this.splineContainer.Spline;
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
    }

    void Move()
    {
        float newAttractionForce = attractionForce;
        if (!hitObstacle)
        {
            if (withPowerUp)
            {
                    maxVelocity = 1000;
            }
            else
            {
            switch (onTrack)
            {
                case false:
                    newAttractionForce = attractionForce / 2;
                    maxVelocity = limitedSpeed;
                    onTrack = false;
                    break;
                case true:
                    onTrack = true;
                    maxVelocity = 1000;
                    break;
            }
            }
        }
        if (hitObstacle&&!withPowerUp)
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

        switch (drivingSurface)
        {
            case Surface.SurfaceType.Sand:
                onTrack = false;
                break;
            case Surface.SurfaceType.Road:
                onTrack = true;
                break;
        }

    }
    void CheckCurrentSpeed()
    {


        if (Velocity >= 10 && startRace)
        {
            health += Mathf.MoveTowards(health, 4, Time.deltaTime * 1);
            //timerSlider.value = health;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("StartRaceZone"))
        {
            gameManager.GameStarted = true;
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
        if (collision.CompareTag("Enemy")){
            if (withPowerUp){
                collision.gameObject.GetComponent<PursuingEnemy>().Freeze();
            }
            else{
                health = 0;
            }
        }
        if (collision.CompareTag("PowerUp")){
            Destroy(collision.gameObject);
            StartCoroutine(WithPowerUp());
        }
        if (collision.CompareTag("SegmentIn")){

            SetSplineContainer(collision.transform.parent.GetChild(1).GetComponent<SplineContainer>());
        }
        if (collision.CompareTag("Coin")){
            Destroy(collision.gameObject);
            if (withPowerUp){
                scoreManager.AddBonusToMainScore(10);
            }
            else{
            scoreManager.AddBonusToMainScore(5);}
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Barier"))
        {
            Destroy(tmpHookPoint);
            Vector2 closestPoint = collision.contacts[0].point;
            playerRb.AddForce((((Vector2)transform.position-closestPoint)+playerRb.linearVelocity/2).normalized * 10, ForceMode2D.Impulse);
            if (!withPowerUp){
            health--;}
        }
    }

    public float GetCurrentSpeed
    {
        get
        {
            return currentSpeed;
        }
    }
    public bool OnTrack
    {
        get { return onTrack; }
    }
    IEnumerator HitObstacle()
    {
        if (!withPowerUp){
        health--;
        hitObstacle = true;
        yield return new WaitForSeconds(2);
        hitObstacle = false;}
    }
    IEnumerator WithPowerUp()
    {
        withPowerUp = true;
        powerUp.SetActive(true);
        if (health < 5)
        {
            health++;
        }
        yield return new WaitForSeconds(5);
        withPowerUp = false;
        powerUp.SetActive(false);
    }

    public float Velocity
    {
        get{ return playerRb.linearVelocity.magnitude; }
    }

    public float Health
    {
        get{ return health; }
    }
    
}

