using UnityEngine;
using UnityEngine.Splines;
using System.Collections;
using System;

public class PursuingEnemy : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private SplineContainer splineContainer;
    [SerializeField] private TrackGenarator trackGenarator;
    [SerializeField] private SplineContainer firstSplineContainer;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip policeSirene;
    private Spline _spline;
    public float defaultSpeed;
    float currentSpeed;
    private float _t;
    private float _splineLength;

    bool freezed;
    bool gameOver = false;
    bool paused = false;

    [SerializeField] private Player player;
    [SerializeField] private EM_GameManager gameManager;
    
    public static event Action<Transform> EnemyFreezed;
    public static event Action EnemyUnfreezed;

    [Header("VFX")]
    [SerializeField] private GameObject enemyFreezedIndicator;

    void Start()
    {
        
        audioSource.clip = policeSirene;
        audioSource.Play();

        enemyFreezedIndicator.SetActive(false);
        splineContainer = firstSplineContainer;
        _spline = splineContainer.Spline;
        CalculateSplineLength();

        GameManager.OnPauseEvent += OnPause;
        GameManager.OnUnpauseEvent += OnUnpause;
        EM_GameManager.OnGameOver += OnGameOver;
    }
    void OnDestroy()
    {
        GameManager.OnPauseEvent -= OnPause;
        GameManager.OnUnpauseEvent -= OnUnpause;
        EM_GameManager.OnGameOver -= OnGameOver;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime * 3f);
        }
        else if (paused)
        {
            audioSource.volume = 0;
        }
        else
        {
            audioSource.volume = PrefsManager.Instance.soundsVolume-0.3f;
        }

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
    enemyFreezedIndicator.SetActive(true);
    EnemyFreezed?.Invoke(transform);
    StartCoroutine(FreezeTimer());
}

void OnPause(){
    paused = true;
}
void OnUnpause(){
    paused = false;
}
void OnGameOver(){
    gameOver = true;
}
IEnumerator FreezeTimer(){
    yield return new WaitForSeconds(3);
    freezed = false;
    enemyFreezedIndicator.SetActive(false);

}
}
