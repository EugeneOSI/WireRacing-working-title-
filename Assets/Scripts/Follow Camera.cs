using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject target;

    [Header("Components")]
    Transform targetTransform;
    Rigidbody2D targetRb;
    Camera cam;
    
    [Header("Camera Parameters")]
    public float smoothSpeed;  
    public float offsetValue;
    public float defaultCamSize;
    public float sizeReduce;
    public float smoothSize;
    private Vector3 originalPos;
    private Coroutine shakeCoroutine;

    [Header("Shake Parameters")]
    public float duration = 0.5f;
    public float magnitude = 0.3f;
    public bool fadeOut = true;
    public bool useLocalPosition = true;
    private Vector3 shakeOffset = Vector3.zero;
    private Vector3 basePosition; // последняя позиция камеры после следования

    [Header("Transform")]
    private Vector3 offset;

    void Start()
    {
        targetRb = target.GetComponent<Rigidbody2D>();
        targetTransform = target.GetComponent<Transform>();
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shake();
        }
    }

    void FixedUpdate()
    {
        CamSize();
        MoveCamera();
    }

    void LateUpdate()
{
    // сохраняем позицию после FixedUpdate (MoveCamera уже отработал)
    basePosition = transform.position;

    // применяем смещение тряски поверх готового движения
    if (shakeOffset != Vector3.zero)
        transform.position = basePosition + shakeOffset;
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

public void Shake()
{
    if (shakeCoroutine != null)
        StopCoroutine(shakeCoroutine);

    shakeCoroutine = StartCoroutine(DoShake());
}

public void Shake(float duration, float magnitude)
{
    this.duration = duration;
    this.magnitude = magnitude;
    Shake();
}

private IEnumerator DoShake()
{
    float elapsed = 0f;

    while (elapsed < duration)
    {
        float strength = fadeOut ? Mathf.Lerp(magnitude, 0, elapsed / duration) : magnitude;
        shakeOffset = Random.insideUnitSphere * strength;
        shakeOffset.z = 0; // не трясём по Z

        elapsed += Time.deltaTime;
        yield return null;
    }

    shakeOffset = Vector3.zero;
    shakeCoroutine = null;
}
    

}
