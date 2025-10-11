using UnityEngine;

public class HookSystem : MonoBehaviour
{
    public GameObject player;
    public GameObject hookPoint;


    Rigidbody2D playerRb;

    GameObject tmpHookPoint;

    public bool hookIsMoving = true;

    public float hookForce;
    public float playerAttractionForce;

    Vector3 hookTargetPos;
    void Start()
    {
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ThrowHookPoint();
        }


        if (tmpHookPoint != null)
        {
            HookPoint hookPoint = tmpHookPoint.GetComponent<HookPoint>();
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
    }

    void MovePlayer()
    {
        Vector2 playerMoveVector = (tmpHookPoint.transform.position - player.transform.position).normalized;
        playerRb.AddForce(playerMoveVector * playerAttractionForce, ForceMode2D.Force);
    }


}
