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
    private Vector2 startCameraPosition;



    [Header("Components")]
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private TrailRenderer trailRenderer;
    private ContactFilter2D contactFilter;
    [SerializeField] private FollowCamera mainCamera;
    [SerializeField] private DirectionTracker directionTracker;
    [SerializeField] private TimeTrialManager timeTrialManager;

    [Header("Particles")]
    [SerializeField] private ParticleSystem offRoadParticles;

    [Header("Audio")]
    [SerializeField] private AudioSource cameraAudioSource;
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip hookThrowSound;
    [SerializeField] private AudioClip hoveringSound;
    [Header("AudioPitch")]
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 2f;
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private float pitchLerpSpeed = 5f;


    public static event Action OnCrossedFirstMarker;
    public static event Action<Vector2> BarierHitTT;
    public static event Action OutOnTrackTT;
    void Start()
    {
        playerAudioSource.clip = hoveringSound;
        playerAudioSource.Play();
        
        offRoadParticles.Stop();
        startPosition = transform.position;
        startCameraPosition = mainCamera.transform.position;
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
        if (!GameManager.Instance.isGamePaused){
        playerAudioSource.volume = PrefsManager.Instance.soundsVolume;

        float speed = Velocity;
        float t = Mathf.Clamp01(speed / maxSpeed);

        float targetPitch = Mathf.Lerp(minPitch, maxPitch, t);
        playerAudioSource.pitch = Mathf.Lerp(playerAudioSource.pitch, targetPitch, Time.deltaTime * pitchLerpSpeed);}
        else{
            playerAudioSource.volume = 0;
        }
        
        
        if (Input.GetKeyDown(KeyCode.R)&&!GameManager.Instance.isGamePaused){
            Destroy(tmpHookPoint);
            playerRb.linearVelocity = Vector2.zero;
            transform.position = startPosition;
            directionTracker.expectedIndex = 37;
            trailRenderer.Clear();
            timeTrialManager.lapRunning = false;
            mainCamera.transform.position = startCameraPosition;
        }
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
        else {
            ResetWirePosition();
        }
    }
    void FixedUpdate()
    {

        if (tmpHookPoint != null && !hookIsMoving)
        {
            Move();
        }
    }


    void ThrowHookPoint()
    {
        cameraAudioSource.PlayOneShot(hookThrowSound);
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


    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("sand"))
        {
            offRoadParticles.Play();
            mainCamera.Shake(1f, 0.05f);
            OutOnTrackTT?.Invoke();
            StartCoroutine(Mistake());
            onTrack = false;
        }
        if (collision.gameObject.CompareTag("DirectionMarker"))
        {
            directionTracker.CheckMarkerIndex(collision.gameObject.GetComponent<DirectionMarker>().index);
        }
        if (collision.gameObject.CompareTag("LastDirectionMarker")){
            directionTracker.CheckMarkerIndex(collision.gameObject.GetComponent<DirectionMarker>().index);
            directionTracker.ResetIndex();
        }

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("sand"))
        {
            offRoadParticles.Stop();
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
            BarierHitTT?.Invoke(closestPoint);
        }
    }
    public float Velocity
    {
        get
        {
            return playerRb.linearVelocity.magnitude;
        }
    }

    IEnumerator Mistake()
    {
        mistake = true;
        yield return new WaitForSeconds(0.1f);
        mistake = false;
    }
}
