using UnityEngine;
using TMPro;
using System.Collections;
using System;

public enum BonusType { nearSand, nearObstacle, highSpeed, tookPowerUp, smashObstacle }

public class BonusScoreHander : MonoBehaviour
{
    [Header("Тип бонуса")]
    public BonusType bonusType;

    [Header("Аниматор")]
    public Animator animator;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI amount;
    [SerializeField] private TextMeshProUGUI description;

    private Player _player;
    private ScoreEventsHander _events;
    private ScoreManager _manager;

    private Coroutine _outTimerCoroutine;

    public float _score { get; private set; } = 0f;
    private float _obstacleExtraTime = 2f;  
    private float _powerUpExtraTime = 2f;   
    private float _smashObstacleExtraTime = 2f;  

    private bool _finished = false;
    private bool _collected = false;

    public void Initialize(ScoreManager manager, ScoreEventsHander eventsHander, Player player, BonusType type)
    {
        _manager = manager;
        _events = eventsHander;
        _player = player;
        bonusType = type;

        if (animator != null)
        {
            animator.SetBool("event", true);
        }

        _manager.RegisterLine(this);
    }

    private void Update()
    {
        if (_finished || _player == null || _events == null || _manager == null)
            return;

        switch (bonusType)
        {
            case BonusType.nearSand:
                HandleNearSand();
                break;

            case BonusType.nearObstacle:
                HandleNearObstacle();
                break;

            case BonusType.highSpeed:
                HandleHighSpeed();
                break;
            case BonusType.tookPowerUp:
                HandleTookPowerUp();
                break;
            case BonusType.smashObstacle:
                HandleSmashObstacle();
                break;
        }

        if (amount != null)
        {
            amount.text = ((int)_score).ToString();
        }

        transform.rotation = Quaternion.identity;
    }


    private void HandleNearSand()
    {
        if (description != null)
            description.text = "ON THE EDGE!";

        if (_events.NearSand)
        {
            _score += _player.Velocity * Time.deltaTime * 2f;
        }

        if (!_manager.NearSandActive)
        {
            StopOutTimerIfRunning();
            _outTimerCoroutine = StartCoroutine(TimerAndFinish(0.3f, collect: true));
        }
        if ((!_player.OnTrack || _player.hitObstacle)&&!_player.withPowerUp)
        {
            StopOutTimerIfRunning();

            if (animator != null)
            {
                animator.SetBool("mistake", true);
            }

            _outTimerCoroutine = StartCoroutine(TimerAndFinish(0.3f, collect: false));
        }
    }

    private void HandleNearObstacle()
    {
        if (!_collected)
        {
            _collected = true;
            _manager.CollectMultiplayerScore(30f);
        }
        if (description != null)
            description.text = "CLOSE!";

        if (_score <= 30f)
        {
            _score += Time.deltaTime * 40f;
        }

        if (_manager.NearObstacleActive)
        {
            _obstacleExtraTime = 2f;
        }
        else
        {
            _obstacleExtraTime -= Time.deltaTime;
            if (_obstacleExtraTime <= 0f)
            {
                StopOutTimerIfRunning();
                _outTimerCoroutine = StartCoroutine(TimerAndFinish(0.3f, collect: false));
            }
        }
    }

    private void HandleHighSpeed()
    {
        if (description != null)
            description.text = "HIGH SPEED!";

        if (_events.HighSpeed)
        {
            _score += _player.Velocity * Time.deltaTime * 2f;
        }

        if (!_manager.HighSpeedActive)
        {
            StopOutTimerIfRunning();
            _outTimerCoroutine = StartCoroutine(TimerAndFinish(0.3f, collect: true));
        }

        if ((!_player.OnTrack || _player.hitObstacle)&&!_player.withPowerUp)
        {
            StopOutTimerIfRunning();

            if (animator != null)
            {
                animator.SetBool("mistake", true);
            }

            _outTimerCoroutine = StartCoroutine(TimerAndFinish(0.3f, collect: false));
        }
    }

    private void HandleTookPowerUp(){
                if (!_collected)
        {
            _collected = true;
            _manager.CollectMultiplayerScore(10f);
        }
        
        if (description != null)
            description.text = "POWER-UP";

        if (_score <= 10f)
        {
            _score += Time.deltaTime * 40f;
        }
            _powerUpExtraTime -= Time.deltaTime;
            if (_powerUpExtraTime <= 0f)
            {
                StopOutTimerIfRunning();
                if (animator != null)
                {
                animator.SetBool("event", false);
                }

                _outTimerCoroutine = StartCoroutine(TimerAndFinish(0.3f, collect: false));
            }
        
    }

    private void HandleSmashObstacle(){
        
        if (!_collected)
        {
            _collected = true;
            _manager.CollectMultiplayerScore(10f);
        }
        if (description != null)
        description.text = "SMASH!";

        if (_score <= 10f)
        {
            _score += Time.deltaTime * 40f;
        }

        _smashObstacleExtraTime -= Time.deltaTime;
        if (_smashObstacleExtraTime <= 0f)
        {
            StopOutTimerIfRunning();
            if (animator != null)
            {
                animator.SetBool("event", false);
            }
            _outTimerCoroutine = StartCoroutine(TimerAndFinish(0.3f, collect: true));
        }
        
    }


    private void StopOutTimerIfRunning()
    {
        if (_outTimerCoroutine != null)
        {
            StopCoroutine(_outTimerCoroutine);
            _outTimerCoroutine = null;
        }
    }

    private IEnumerator TimerAndFinish(float delay, bool collect)
    {
        _finished = true;

        if (animator != null && collect)
        {
            animator.SetBool("event", false);
        }

        yield return new WaitForSeconds(delay);

        if (_manager != null) // если сцену не сменили
        {
            _manager.OnLineFinished(this, _score, collect);
        }
    }
}
