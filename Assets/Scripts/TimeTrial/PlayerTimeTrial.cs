using UnityEngine;
using System.Collections;
using System;
public class PlayerTimeTrial : MonoBehaviour
{
    
    [Header("Game Objects")]
    public GameObject hookPoint;
    private GameObject tmpHookPoint;
    [SerializeField] private GameObject playerSprite;

    [Header("States")]
    public bool onTrack;
    public bool mistake { get; private set; }
    private bool hookIsMoving;

    [Header("Parameters")]
    public float hookSpeed;
    public float limitedSpeed;
    public float attractionForce;
    public float breakForce;
    private float maxVelocity;
    private Vector2 startPosition;



    [Header("Components")]
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Collider2D playerCol;
    private ContactFilter2D contactFilter;
    private Collider2D[] surfaceCollidersHit = new Collider2D[10];
    [SerializeField] private FollowCamera mainCamera;

    Surface.SurfaceType drivingSurface = Surface.SurfaceType.Road;

    public static event Action OnCrossedFirstMarker;
    void Start()
    {
        startPosition = transform.position;
        contactFilter = new ContactFilter2D();
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = true;
        hookIsMoving = false;
        mistake = false;
        onTrack = true;

        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.R)){
            Destroy(tmpHookPoint);
            playerRb.linearVelocity = Vector2.zero;
            transform.position = startPosition;
        }
        /*if (playerSprite == null)
        Debug.LogError("playerSprite = null на " + name);*/

    /*if (playerRb == null)
        Debug.LogError("playerRb = null на " + name);*/
        transform.rotation = Quaternion.Euler(0, 0, 0);
        playerSprite.transform.rotation = Quaternion.Euler(0, 0, (Mathf.Atan2(playerRb.linearVelocity.y, playerRb.linearVelocity.x) * Mathf.Rad2Deg)-90);
        
        if (Input.GetMouseButtonDown(0)&&Time.timeScale != 0)
        {
            ResetWirePosition();
            ThrowHookPoint();
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
        //GetSurfaceBehavior();

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
    }

    void Move()
    {
        float newAttractionForce = attractionForce;
            switch (onTrack)
            {
                case false:
                    newAttractionForce = attractionForce / 2;
                    maxVelocity = limitedSpeed;
                    break;
                case true:
                    maxVelocity = 1000;
                    break;
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

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("sand"))
        {
            mainCamera.Shake(1f, 0.05f);
            StartCoroutine(Mistake());
            onTrack = false;
        }
        if (collision.gameObject.CompareTag("DirectionMarker"))
        {
            collision.gameObject.GetComponent<DirectionMarker>().SetPassed();
        }
        if (collision.gameObject.CompareTag("FirstDirectionMarker")){
            collision.gameObject.GetComponent<DirectionMarker>().SetPassed();
            OnCrossedFirstMarker?.Invoke();
        }

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("sand"))
        {
            onTrack = true;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Barier"))
        {
            Destroy(tmpHookPoint);
            mainCamera.Shake(0.5f, 0.3f);
            Vector2 closestPoint = collision.contacts[0].point;
            playerRb.AddForce((((Vector2)transform.position-closestPoint)+playerRb.linearVelocity/2).normalized * 10, ForceMode2D.Impulse);
        }
    }

    IEnumerator Mistake()
    {
        mistake = true;
        yield return new WaitForSeconds(0.1f);
        mistake = false;
    }
}
