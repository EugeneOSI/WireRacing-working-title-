using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject target;
    Transform targetTransform;
    Rigidbody2D targetRb; 
    Camera cam;
    public float smoothSpeed;
    private Vector3 offset;
    public float offsetValue;

    public float defaultCamSize;

    public float sizeReduce;
    public float smoothSize;

    void Start()
    {
        targetRb = target.GetComponent<Rigidbody2D>();
        targetTransform = target.GetComponent<Transform>();
        cam = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        CamSize();
        MoveCamera();
    }

    void CamSize()
    {
        float currentCamSize = cam.orthographicSize;
        float newCamSize = Mathf.Lerp(currentCamSize, defaultCamSize + targetRb.linearVelocity.magnitude/sizeReduce, smoothSize);
        cam.orthographicSize = newCamSize;
    }

    void MoveCamera()
    {
        offset = targetRb.linearVelocity + new Vector2(0, 0) * offsetValue;
        offset.z = -10;        
        Vector3 desiredPosition = targetTransform.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
