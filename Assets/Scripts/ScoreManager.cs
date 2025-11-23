using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour
{
    [Header("Main score")]
    [SerializeField] private float mainScore;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private ScoreEventsHander scoreEventsHander;
    [SerializeField] private Transform linesRoot;      // контейнер для строк (панель/вертикальный layout)
    [SerializeField] private GameObject bonusScorePrefab;

    [Header("Multiplayer")]
    public int multiplayerScore { get; set; }
    public int multiplayerAmount { get; set; }
    [SerializeField] private TextMeshProUGUI multiplayerScoreText;
    [SerializeField] private TextMeshProUGUI multiplayerAmountText;
    [SerializeField] private Slider multiplayerSlider;

    [Header("Lines layout")]
    [SerializeField] private float lineHeight = 40f;

    // состояние событий
    public bool NearSandActive     { get; private set; }
    public bool NearObstacleActive { get; private set; }
    public bool HighSpeedActive    { get; private set; }
    public bool WithPowerUpActive { get; private set; }
    public bool SmashObstacleActive { get; private set; }
    public bool MultiplayerActive { get; private set; }

    // корутины
    private Coroutine _nearSandInCoroutine;
    private Coroutine _nearSandOutCoroutine;
    private Coroutine _highSpeedOutCoroutine;
    private Coroutine _nearObstacleInCoroutine;

    private readonly List<BonusScoreHander> _activeLines = new();

    private void Awake()
    {
        // подстраховка, если не проставлено в инспекторе
        if (player == null)
        {
            var pObj = GameObject.Find("Player");
            if (pObj != null) player = pObj.GetComponent<Player>();
        }

        if (scoreEventsHander == null && player != null)
        {
            scoreEventsHander = player.GetComponent<ScoreEventsHander>();
        }
    }

    private void Update()
    {
        UpdateMainScore();
        UpdateMultiplayerScore();
        CheckEvents();
    }

    // ---------- Работа с линиями ----------

    public void RegisterLine(BonusScoreHander line)
    {
        if (!_activeLines.Contains(line))
        {
            _activeLines.Add(line);
            UpdateLinesPositions();
        }
    }

    public void UnregisterLine(BonusScoreHander line)
    {
        if (_activeLines.Remove(line))
        {
            UpdateLinesPositions();
        }
    }

    private void UpdateLinesPositions()
    {
        for (int i = 0; i < _activeLines.Count; i++)
        {
            var mover = _activeLines[i].GetComponent<LineMover>();
            if (mover != null)
            {
                float targetY = -i * lineHeight;  // каждая следующая строка ниже на lineHeight
                mover.StartMove(targetY);
            }
        }
    }


    public void OnLineFinished(BonusScoreHander line, float bonusScore, bool collectToMainScore)
    {
        /*if (collectToMainScore)
        {
            mainScore += bonusScore;
        }*/

        UnregisterLine(line);

        if (line != null)
        {
            Destroy(line.gameObject);
        }
    }

    // ---------- Логика событий ----------

    private void CheckEvents()
    {
        if (scoreEventsHander == null || player == null) return;

        HandleSandEvent();
        HandleObstacleEvent();
        HandleHighSpeedEvent();
        HandlePowerUpEvent();
        HandleSmashObstacleEvent();
    }

    // --- Near Sand ---
    private void HandleSandEvent()
    {
        bool detected = scoreEventsHander.NearSand;

        // вход
        if (detected && !NearSandActive && _nearSandInCoroutine == null)
        {
            _nearSandInCoroutine = StartCoroutine(SandEnterAfterDelay(0.3f));
        }

        // отмена входа, если быстро ушли
        if (!detected && !NearSandActive && _nearSandInCoroutine != null)
        {
            StopCoroutine(_nearSandInCoroutine);
            _nearSandInCoroutine = null;
        }

        // выход
        if (!detected && NearSandActive && _nearSandOutCoroutine == null)
        {
            _nearSandOutCoroutine = StartCoroutine(SandExitAfterDelay(2f));
        }

        // вернулись к краю — отменяем выход
        if (detected && _nearSandOutCoroutine != null)
        {
            StopCoroutine(_nearSandOutCoroutine);
            _nearSandOutCoroutine = null;
        }
    }

    private IEnumerator SandEnterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NearSandActive = true;
        IncreaseMultiplayerSlider();
        CreateLine(BonusType.nearSand);
        _nearSandInCoroutine = null;
    }

    private IEnumerator SandExitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NearSandActive = false;
        _nearSandOutCoroutine = null;
    }

    // --- Near Obstacle ---
    private void HandleObstacleEvent()
    {
        if (scoreEventsHander.NearObstacle && !NearObstacleActive && _nearObstacleInCoroutine == null)
        {
            _nearObstacleInCoroutine = StartCoroutine(ObstacleEnterAfterDelay(0.5f));
        }

        // если врезались — не даём бонус за "почти"
        if (_nearObstacleInCoroutine != null && (player.hitObstacle||player.smashObstacle))
        {
            StopCoroutine(_nearObstacleInCoroutine);
            _nearObstacleInCoroutine = null;
        }
    }

    private IEnumerator ObstacleEnterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NearObstacleActive = true;
        IncreaseMultiplayerSlider();
        CreateLine(BonusType.nearObstacle);
        StartCoroutine(ObstacleExitAfterDelay(0.8f));
        _nearObstacleInCoroutine = null;
    }

    private IEnumerator ObstacleExitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NearObstacleActive = false;
    }

    // --- High Speed ---
    private void HandleHighSpeedEvent()
    {
        bool detected = scoreEventsHander.HighSpeed;

        if (detected && !HighSpeedActive)
        {
            //Debug.Log("High Speed Event");
            HighSpeedActive = true;
            IncreaseMultiplayerSlider();
            CreateLine(BonusType.highSpeed);
        }

        if (!detected && HighSpeedActive && _highSpeedOutCoroutine == null)
        {
            _highSpeedOutCoroutine = StartCoroutine(HighSpeedExitAfterDelay(2f));
        }

        if (detected && _highSpeedOutCoroutine != null)
        {
            StopCoroutine(_highSpeedOutCoroutine);
            _highSpeedOutCoroutine = null;
        }
    }

    private IEnumerator HighSpeedExitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HighSpeedActive = false;
        _highSpeedOutCoroutine = null;

    }

    // --- With Power Up ---
    private void HandlePowerUpEvent()
    {
        bool detected = scoreEventsHander.WithPowerUp;
        if (detected && !WithPowerUpActive){
        WithPowerUpActive = true;
        CreateLine(BonusType.tookPowerUp);
        IncreaseMultiplayerSlider();
        StartCoroutine(PowerUpExitAfterDelay(5f));}
    }

    private IEnumerator PowerUpExitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        WithPowerUpActive = false;

    }

    // --- Smash Obstacle ---
    private void HandleSmashObstacleEvent()
    {
        bool detected = scoreEventsHander.SmashObstacle;
        if (detected && !SmashObstacleActive){
        SmashObstacleActive = true;
        CreateLine(BonusType.smashObstacle);
        IncreaseMultiplayerSlider();
        StartCoroutine(SmashObstacleExitAfterDelay(2f));}
    }
    private IEnumerator SmashObstacleExitAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SmashObstacleActive = false;

    }
    
    // ---------- Вспомогательное ----------

    private void CreateLine(BonusType type)
    {
        if (bonusScorePrefab == null || linesRoot == null) return;

        GameObject instance = Instantiate(bonusScorePrefab, linesRoot);
        var handler = instance.GetComponent<BonusScoreHander>();

        if (handler != null)
        {
            handler.Initialize(this, scoreEventsHander, player, type);
        }

    }

    private void UpdateMainScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + (int)mainScore;
        }
    }

    private void UpdateMultiplayerScore()
    {
        multiplayerAmountText.text = "X" + multiplayerAmount;
        multiplayerScore = (int)GetActiveLinesScore();
        multiplayerScoreText.text = "" + multiplayerScore;
        if (_activeLines.Count > 0){
            MultiplayerActive = true;}

        else {MultiplayerActive = false;}
        MultiplayerScoreHandler();
    }

    private void MultiplayerScoreHandler(){
        
        if(MultiplayerActive) {
     if (multiplayerSlider.value >= multiplayerSlider.maxValue)
        {
            IncreaseMultiplayer();
        }}
        if (!MultiplayerActive) {
            
            if(multiplayerScore > 0){
            mainScore += multiplayerScore*multiplayerAmount;
            multiplayerAmount = 1;
            multiplayerScore = 0;
            multiplayerSlider.value = 0;
            multiplayerSlider.maxValue = 1;}
        }

    }

    private void IncreaseMultiplayer(){
        multiplayerAmount++;
        MultiplayerSliderMaxValue(multiplayerSlider.maxValue+0.4f);
    }
    private void MultiplayerSliderMaxValue(float amount)
    {
        multiplayerSlider.maxValue = amount;
    }
    private void IncreaseMultiplayerSlider()
    {
        multiplayerSlider.value += 0.2f;
    }

    public float GetActiveLinesScore()
{
    float total = 0f;

    foreach (var go in _activeLines)
    {
        if (go == null) 
            continue;

        var handler = go.GetComponent<BonusScoreHander>();
        if (handler == null) 
            continue;

        total += handler._score;
    }

    return total;
}

}
