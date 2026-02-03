using UnityEngine;

public class LineMover : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float targetY = .5f;
    private bool isActive = false;
    RectTransform rt;


    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }
    void Update()
    {

        if (!isActive) return;

        Vector3 pos = rt.anchoredPosition;

        float newY = Mathf.Lerp(pos.y, targetY, moveSpeed * Time.deltaTime);
        rt.anchoredPosition = new Vector3(0f, newY, 0f);

        if (Mathf.Abs(newY - targetY) < 0.01f)
        {
            rt.anchoredPosition = new Vector3(0f, targetY, 0f);
            //StopMove();
        }
    }

    public void StartMove(float target)
    {
        targetY = target;
        isActive = true;
    }

    public void StopMove()
    {
        isActive = false;
    }
}

