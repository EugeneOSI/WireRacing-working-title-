using UnityEngine;
using UnityEngine.UI;

public class HookSystem : MonoBehaviour
{
    public GameObject player;
    public GameObject hookPoint;
    Rigidbody2D playerRb;
    GameObject tmpHookPoint;
    public bool hookIsMoving = true;
    //private bool hookIsActive = false;
    public float hookForce;
    public float playerAttractionForce;
    public float limitedSpeed;
    public float breakForce;
    public float maxHooksAmount;
    private float currentHooksAmount;

    public Slider hookSlider;

    private LineRenderer lineRenderer;

    Vector3 hookTargetPos;
    void Start()
    {
        playerRb = player.GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        hookSlider.maxValue = maxHooksAmount;
        currentHooksAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        HookCooldown();
        if (Input.GetMouseButtonDown(0)&& currentHooksAmount<maxHooksAmount)
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

    private void FixedUpdate()
    {
        if (tmpHookPoint != null && !hookIsMoving)
        {
            MovePlayer();
        }
    }

    void ThrowHookPoint()
    {
        hookTargetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hookTargetPos.z = 0;

        tmpHookPoint = Instantiate(hookPoint, player.transform.position, Quaternion.identity);

        HookPoint hookPointSetting = tmpHookPoint.GetComponent<HookPoint>();
        hookPointSetting.targetPosition = hookTargetPos;
        hookPointSetting.flyingForce = hookForce;

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
    void ResetWirePosition()
    {
        lineRenderer.SetPosition(0, player.transform.position);
        lineRenderer.SetPosition(1, player.transform.position);
    }
    void DrawWire()
    {
        lineRenderer.SetPosition(0, player.transform.position);
        lineRenderer.SetPosition(1, tmpHookPoint.transform.position);
    }

    void MovePlayer()
    {
        float attractionForce;
        float maxVelocity;
        switch (player.GetComponent<Player>().onTrack)
        {
            case true:
        attractionForce = playerAttractionForce;
        maxVelocity = float.MaxValue;
                break;
            case false:
                attractionForce = playerAttractionForce / 2;
                maxVelocity = limitedSpeed;
                break;

        }
        


        Vector2 playerMoveVector = (tmpHookPoint.transform.position - player.transform.position).normalized;
        playerRb.AddForce(playerMoveVector * attractionForce, ForceMode2D.Force);
        if (playerRb.linearVelocity.magnitude > maxVelocity)
        {
            float currentVelocity = playerRb.linearVelocity.magnitude;
            playerRb.linearVelocity = playerRb.linearVelocity.normalized * Mathf.MoveTowards(currentVelocity, maxVelocity, Time.deltaTime * breakForce);
        }

    }


}
