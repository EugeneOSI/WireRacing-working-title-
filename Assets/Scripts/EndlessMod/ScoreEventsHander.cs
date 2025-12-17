using UnityEngine;

public class ScoreEventsHander : MonoBehaviour
{
    [Header("Raycast settings")]
    public LayerMask roadEdgeMask;
    [SerializeField] private float maxCheckDistance = 3f;

    [SerializeField] private Player player;
    private Rigidbody2D _playerRb;

    public bool NearSand { get; private set; }
    public bool NearObstacle { get; private set; }
    public bool HighSpeed { get; private set; }
    public bool WithPowerUp { get; private set; }
    public bool SmashObstacle { get; private set; }

    private void Awake()
    {
        _playerRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        WatchDistance();

        // high speed
        if (_playerRb != null)
        {
            HighSpeed = _playerRb.linearVelocity.magnitude >= 20f;
        }
        else
        {
            HighSpeed = false;
        }
    }

    private void WatchDistance()
    {
        RaycastHit2D[] hits = {
            Physics2D.Raycast(transform.position, Vector2.up,    maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, Vector2.down,  maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, Vector2.right, maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, Vector2.left,  maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, new Vector2( 1,  1).normalized, maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, new Vector2(-1,  1).normalized, maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, new Vector2( 1, -1).normalized, maxCheckDistance, roadEdgeMask),
            Physics2D.Raycast(transform.position, new Vector2(-1, -1).normalized, maxCheckDistance, roadEdgeMask),
        };

        bool sandDetected = false;
        bool obstacleDetected = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null) continue;

            if (hit.collider.CompareTag("sand")){
                sandDetected = true;} //Debug.Log("Sand Touched");}
            else if (hit.collider.CompareTag("Obstacle"))
                obstacleDetected = true;
        }
        if (player.withPowerUp)
            WithPowerUp = true;
        else
            WithPowerUp = false;
        if (player.smashObstacle)
            SmashObstacle = true;
        else
            SmashObstacle = false;

        NearSand = sandDetected;
        NearObstacle = obstacleDetected;

        // дебаг-лучи оставил как были
        Debug.DrawRay(transform.position, Vector2.up    * maxCheckDistance, Color.red);
        Debug.DrawRay(transform.position, Vector2.down  * maxCheckDistance, Color.red);
        Debug.DrawRay(transform.position, Vector2.right * maxCheckDistance, Color.red);
        Debug.DrawRay(transform.position, Vector2.left  * maxCheckDistance, Color.red);
        Debug.DrawRay(transform.position, new Vector2( 1,  1).normalized * maxCheckDistance, Color.red);
        Debug.DrawRay(transform.position, new Vector2(-1,  1).normalized * maxCheckDistance, Color.red);
        Debug.DrawRay(transform.position, new Vector2( 1, -1).normalized * maxCheckDistance, Color.red);
        Debug.DrawRay(transform.position, new Vector2(-1, -1).normalized * maxCheckDistance, Color.red);
    }
}
