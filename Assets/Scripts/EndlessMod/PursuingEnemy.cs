using UnityEngine;
using UnityEngine.Splines;
using System.Collections;


public class PursuingEnemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private SplineContainer splineContainer;
    [SerializeField] private TrackGenarator trackGenarator;
    [SerializeField] private SplineContainer firstSplineContainer;
    private Spline _spline;
    public float defaultSpeed;
    float currentSpeed;
    private float _t;
    private float _splineLength;

    bool freezed;

    [SerializeField] private Player player;
    GameManagerEM gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerEM>();
        splineContainer = firstSplineContainer;
        _spline = splineContainer.Spline;
        CalculateSplineLength();
    }

    // Update is called once per frame
    void Update()
    {
        CheckConditions();

    if (splineContainer==null){
        SetSplineContainer(trackGenarator.GetPrevSegment.transform.GetChild(1).GetComponent<SplineContainer>());
        _t = 0f;
        CalculateSplineLength();
    }

    if (splineContainer != null&&_splineLength>0f)
   {
    _t += (currentSpeed * Time.deltaTime) / _splineLength;
    if (_t >= 1f) _t = 0f;
    
    Vector3 localPos = _spline.EvaluatePosition(_t);
    Vector3 localTangent = _spline.EvaluateTangent(_t);
    
    transform.position = splineContainer.transform.TransformPoint(localPos);
    Vector3 worldTangent = splineContainer.transform.TransformDirection(localTangent).normalized;
    

    float angle = Mathf.Atan2(worldTangent.y, worldTangent.x) * Mathf.Rad2Deg;
    transform.rotation = Quaternion.Euler(0, 0, angle);
   }
    }

    public void SetSplineContainer(SplineContainer splineContainer){

            this.splineContainer = splineContainer;
            _spline = this.splineContainer.Spline;
            CalculateSplineLength();
    }

        void CalculateSplineLength()
    {
        if (_spline == null) return;
        
        // Calculate spline length by sampling points
        _splineLength = 0f;
        int samples = 100; // Number of samples for accuracy
        Vector3 prevPos = splineContainer.transform.TransformPoint(_spline.EvaluatePosition(0f));
        
        for (int i = 1; i <= samples; i++)
        {
            float t = (float)i / samples;
            Vector3 currentPos = splineContainer.transform.TransformPoint(_spline.EvaluatePosition(t));
            _splineLength += Vector3.Distance(prevPos, currentPos);
            prevPos = currentPos;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SegmentIn")){

            SetSplineContainer(collision.transform.parent.GetChild(1).GetComponent<SplineContainer>());
            _t = 0f;
        }
    }

    void CheckConditions(){
        if (!gameManager.GameOver){
        if (player.Velocity<9&&gameManager.GameStarted){
            currentSpeed = defaultSpeed+10;
        }
        if (player.Velocity>=5&&gameManager.GameStarted)
        {
            currentSpeed = defaultSpeed;
        }
        if (!gameManager.GameStarted||freezed)
        {
            currentSpeed = 0;
        }}
        else{
        if (Vector3.Distance(transform.position, player.transform.position) < 15){
        currentSpeed = Mathf.MoveTowards(currentSpeed, 0, Time.deltaTime * 15);}
    }
    }
public void Freeze(){
    freezed = true;
    StartCoroutine(FreezeTimer());
}

IEnumerator FreezeTimer(){
    yield return new WaitForSeconds(3);
    freezed = false;
}
}
