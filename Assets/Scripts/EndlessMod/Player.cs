using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Splines;
using System;
public class Player : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject hookPoint;
    private GameObject tmpHookPoint;
    [SerializeField] private GameObject powerUp;
    [SerializeField] private GameObject playerSprite;

    [Header("States")]
    public bool isAlive;
    public bool onTrack;
    private bool hookIsMoving;
    public bool hitObstacle { get; private set; }
    public bool withPowerUp{ get; private set; }
    public bool smashObstacle{ get; private set; }

    [Header("Parameters")]
    public float health;
    public float hookSpeed;
    public float limitedSpeed;
    public float attractionForce;
    public float breakForce;
    public float maxHooksAmount;
    private float maxVelocity;
    private float currentSpeed;


    [Header("Sliders")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider powerUpBar;

    [Header("Components")]
    private Rigidbody2D playerRb;
    private LineRenderer lineRenderer;
    private Collider2D playerCol;
    private FollowCamera mainCamera;

    [SerializeField] private EM_GameManager gameManager;

    [Header("Coroutines")]
    private Coroutine withPowerUpCoroutine;

    public static event Action<Vector2> BarierHit;
    public static event Action<Transform> PowerUpHit;
    public static event Action<Transform> ObstacleHit;
    public static event Action<Transform> ObstacleSmash;
    public static event Action PowerUpActive;
    public static event Action PowerUpEnd;
    public static event Action OutOnTrack;

    [Header("VFX")]
    [SerializeField] private ParticleSystem offRoadParticles;
    [SerializeField] private ParticleSystem healingParticles;
    [SerializeField] private ParticleSystem powerUpParticles;
    [SerializeField] private GameObject stunEffect;
    [SerializeField] private Animator powerUpHalo;




    void Start()
    {
        
        healingParticles.Stop();
        offRoadParticles.Stop();
        powerUpParticles.Stop();
        stunEffect.SetActive(false);
        powerUpBar.gameObject.SetActive(false);


        hookIsMoving = false;
        isAlive = true;
        hitObstacle = false;
        withPowerUp = false;
        onTrack = true;
        powerUp.SetActive(false);


        playerRb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        playerCol = GetComponent<Collider2D>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<FollowCamera>();
    }

    void Update()
    {
        CheckCurrentSpeed();
        transform.rotation = Quaternion.Euler(0, 0, 0);
        playerSprite.transform.rotation = Quaternion.Euler(0, 0, (Mathf.Atan2(playerRb.linearVelocity.y, playerRb.linearVelocity.x) * Mathf.Rad2Deg)-90);
        
        if (Input.GetMouseButtonDown(0) && !gameManager.IsPaused && !gameManager.GameOver)
        {
            ResetWirePosition();
            ThrowHookPoint();
        }
        if (health <= 0)
        {
            isAlive = false;
            //Debug.Log("You Died");
        }

        if (tmpHookPoint != null)
        {
            HookPoint hookPoint = tmpHookPoint.GetComponent<HookPoint>();
            DrawWire();
            if (!hookPoint.moveStatus) hookIsMoving = false;
        }

        healthBar.value = health;

        if (withPowerUp)
        {
            DigressPowerUp();
        }
    }


    void FixedUpdate()
    {
        //GetSurfaceBehavior();

        if (tmpHookPoint != null && !hookIsMoving)
        {
            Move();
        }
        if (gameManager.GameOver)
        {
            maxVelocity = 0.001f;
            float currentVelocity = playerRb.linearVelocity.magnitude;
            playerRb.linearVelocity = playerRb.linearVelocity.normalized * Mathf.MoveTowards(currentVelocity, maxVelocity, Time.deltaTime * breakForce);
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
        if (!hitObstacle&&!gameManager.GameOver)
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

    void CheckCurrentSpeed()
    {
        if (Velocity >= 20 && health < 4)
        {
            if (!healingParticles.isPlaying)
        {
            healingParticles.Play();
        }
            health = Mathf.MoveTowards(health, 4, Time.deltaTime * 0.5f);
        }
        else{
             if (healingParticles.isPlaying)
        {
            healingParticles.Stop();
        }
        }
    }


    void DisableHook(){
        if (tmpHookPoint != null){
            Destroy(tmpHookPoint);
            lineRenderer.enabled = false;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("StartRaceZone"))
        {
            gameManager.GameStarted = true;
        }
        if (collision.CompareTag("sand"))
        {
            onTrack = true;
            offRoadParticles.Stop();
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            StartCoroutine(HitObstacle(collision));
        }
        if (collision.CompareTag("sand"))
        {
            offRoadParticles.Play();
            if (!withPowerUp)
            {
                OutOnTrack?.Invoke();
                mainCamera.Shake(1f, 0.05f);
                onTrack = false;
            }
        }
        if (collision.CompareTag("Enemy")){
            if (withPowerUp){
                collision.gameObject.GetComponent<PursuingEnemy>().Freeze();
            }
            else{
                health = 0;
                DisableHook();
            }
        }
        if (collision.CompareTag("PowerUp")){
            PowerUpHit?.Invoke(collision.transform);

            Destroy(collision.gameObject);
            if (withPowerUp){       
            StopCoroutine(withPowerUpCoroutine);
            withPowerUpCoroutine = null;
            withPowerUpCoroutine = StartCoroutine(WithPowerUp());}
            else{
                withPowerUpCoroutine = StartCoroutine(WithPowerUp());
            }
        }
        if (collision.CompareTag("DeadZone")){
            DisableHook();
            health = 0;
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Barier"))
        {
            Destroy(tmpHookPoint);
            mainCamera.Shake(0.5f, 0.3f);
            Vector2 closestPoint = collision.contacts[0].point;
            playerRb.AddForce((((Vector2)transform.position-closestPoint)+playerRb.linearVelocity/2).normalized * 5, ForceMode2D.Impulse);
            if (!withPowerUp){
            health--;}
            BarierHit?.Invoke(closestPoint);
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
    IEnumerator HitObstacle(Collider2D collision)
    {
        if (!withPowerUp){
        stunEffect.SetActive(true);
        ObstacleHit?.Invoke(collision.transform);
        mainCamera.Shake(0.5f, 0.3f);
        health--;
        hitObstacle = true;
        yield return new WaitForSeconds(2);
        stunEffect.SetActive(false);
        hitObstacle = false;}
        else{
            ObstacleSmash?.Invoke(collision.transform);
            mainCamera.Shake(0.2f, 0.1f);
            smashObstacle = true;
            yield return new WaitForSeconds(2);
            smashObstacle = false;
        }
    }
    IEnumerator WithPowerUp()
    {
        PowerUpActive?.Invoke();
        powerUpParticles.Play();
        powerUpHalo.SetTrigger("Active");
        withPowerUp = true;
        powerUpBar.gameObject.SetActive(true);
        powerUpBar.value = 5;
        powerUp.SetActive(true);
        if (health < 5)
        {
            health++;
        }
        yield return new WaitForSeconds(5);
        withPowerUp = false;
        powerUpBar.gameObject.SetActive(false);
        powerUp.SetActive(false);
        powerUpParticles.Stop();
        powerUpHalo.SetTrigger("Unactive");
        PowerUpEnd?.Invoke();
    }
    void DigressPowerUp()
    {
        if (powerUpBar.value > 0)
        {
            powerUpBar.value-=Time.deltaTime;
        }
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

