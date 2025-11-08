/*using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;

public class ScoreManager : MonoBehaviour
{
    float mainScore;

    public float lineSpeed;

    [Header("События")]
    bool nearSand;
    bool nearObstacle;
    bool highSpeed;
    GameObject nearSandInstance;
    GameObject nearObstacleInstance;
    GameObject highSpeedInstance;
    public float nearSandTimerInDefault = 0.2f;
    public float nearSandTimerOutDefault = 0.3f;
    float nearSandTimerIn;
    float nearSandTimerOut;

    public float hightSpeedTimerInDefault = 0.1f;
    public float hightSpeedTimerOutDefault = 0.5f;
    float hightSpeedTimerIn;
    float hightSpeedTimerOut;

    public float nearObstacleTimerInDefault = 0.05f;
    public float nearObstacleTimerOutDefault = 1f;
    float nearObstacleTimerIn;
    float nearObstacleTimerOut;
   

    [SerializeField] GameObject bonusScoreprefab;
    private Player player;
    private ScoreHander scoreHander;
    public TextMeshProUGUI scoreText;

    List<GameObject> lines;

    void Start()
    {

        nearSandTimerIn = nearSandTimerInDefault;
        nearSandTimerOut = nearSandTimerOutDefault;

        nearObstacleTimerIn = nearObstacleTimerInDefault;
        nearObstacleTimerOut = nearObstacleTimerOutDefault;

        hightSpeedTimerIn = hightSpeedTimerInDefault;
        hightSpeedTimerOut = hightSpeedTimerOutDefault;

        nearSand = false;
        nearObstacle = false;
        lines = new List<GameObject>();
        mainScore = 0;
        player = GameObject.Find("Player").GetComponent<Player>();
        scoreHander = GameObject.Find("Player").GetComponent<ScoreHander>();
  

    }

    void Update()
    {
        UpdateMainScore();
        CheckEvents();     
    }

    void CheckEvents()
    {

        if (scoreHander.NearSand && !nearSand)
        {
            nearSandTimerIn -= Time.deltaTime;
            if (nearSandTimerIn <= 0)
            {
                CreateLine("sand");
                nearSand = true;
                nearSandTimerOut = nearSandTimerOutDefault;
            }

        }
        if (!scoreHander.NearSand && !nearSand)
        {
            nearSandTimerIn = nearSandTimerInDefault;
        }
        if (!scoreHander.NearSand && nearSand)
        {
            nearSandTimerOut -= Time.deltaTime;
            if (nearSandTimerOut <= 0)
            {
                nearSandInstance.GetComponent<BonusScoreUI>().animator.SetBool("event", false);
                StartCoroutine(TimerAndRemoveLine("sand", 0.3f));
                nearSandTimerIn = nearSandTimerInDefault;
                nearSand = false;
            }
        }
        if (scoreHander.NearObstacle && !nearObstacle)
        {
            nearObstacleTimerIn -= Time.deltaTime;
            if (nearObstacleTimerIn <= 0)
            {
                CreateLine("obstacle");
                nearObstacle = true;
                nearObstacleTimerOut = nearObstacleTimerOutDefault;
            }
        }
        if (!scoreHander.NearObstacle && !nearObstacle)
        {
            nearObstacleTimerIn = nearObstacleTimerInDefault;
        }
        if (!scoreHander.NearObstacle && nearObstacle)
        {
            nearObstacleTimerOut -= Time.deltaTime;
            if (nearObstacleTimerOut <= 0)
            {
                nearObstacle = false;
                nearObstacleInstance.GetComponent<BonusScoreUI>().animator.SetBool("event", false);
                StartCoroutine(TimerAndRemoveLine("obstacle", 0.3f));
                nearObstacleTimerIn = nearObstacleTimerInDefault;
            }
        }
        if (scoreHander.HighSpeed && !highSpeed)
        {
            hightSpeedTimerIn -= Time.deltaTime;
            if (hightSpeedTimerIn <= 0)
            {
                CreateLine("speed");
                highSpeed = true;
                hightSpeedTimerOut = hightSpeedTimerOutDefault;
            }
        }
        if (!scoreHander.HighSpeed && !highSpeed)
        {
            hightSpeedTimerIn = hightSpeedTimerInDefault;
        }
        if (!scoreHander.HighSpeed && highSpeed)
        {
            hightSpeedTimerOut -= Time.deltaTime;
            if (hightSpeedTimerOut <= 0)
            {
                highSpeedInstance.GetComponent<BonusScoreUI>().animator.SetBool("event", false);
                StartCoroutine(TimerAndRemoveLine("speed", 0.3f));
                highSpeed = false;
                hightSpeedTimerIn = hightSpeedTimerInDefault;
            }
        }


    }

    void CreateLine(string name)
    {
        GameObject bonusScoreInstance = Instantiate(bonusScoreprefab);

        if (lines.Count > 0)
        {
            foreach (var line in lines)
            {
                line.GetComponent<LineMover>().StartMove("up");
            }
        }
        bonusScoreInstance.transform.position = player.transform.position;
        bonusScoreInstance.transform.SetParent(player.transform);
        bonusScoreInstance.GetComponent<BonusScoreUI>().animator.SetBool("event", true);
        switch (name)
        {
            case "sand":
                bonusScoreInstance.GetComponent<BonusScoreUI>().bonusType = BonusType.nearSand;
                nearSandInstance = bonusScoreInstance;
                lines.Add(nearSandInstance);
                break;
            case "obstacle":
                bonusScoreInstance.GetComponent<BonusScoreUI>().bonusType = BonusType.nearObstacle;
                nearObstacleInstance = bonusScoreInstance;
                lines.Add(nearObstacleInstance);
                break;
            case "speed":
            bonusScoreInstance.GetComponent<BonusScoreUI>().bonusType = BonusType.highSpeed;
                highSpeedInstance = bonusScoreInstance;
                lines.Add(highSpeedInstance);
                break;

        }

    }
    
    void RemoveLine(string name)
    {
        switch (name)
        {
            case "sand":
                lines.Remove(nearSandInstance);
                Destroy(nearSandInstance.gameObject);
                break;
            case "obstacle":
                lines.Remove(nearObstacleInstance);
                Destroy(nearObstacleInstance);
                break;
            case "speed":
                lines.Remove(highSpeedInstance);
                Destroy(highSpeedInstance);
                break;
        }

        if (lines.Count > 0)
        {
            foreach (var line in lines)
            {
                line.GetComponent<LineMover>().StartMove("down");
            }
            
        }
    }
    void UpdateMainScore()
    {
        mainScore += player.Velocity * Time.deltaTime;
        scoreText.text = "Score: " + (int)mainScore;
    }

    IEnumerator TimerAndRemoveLine(string lineName, float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveLine(lineName);
    }
    IEnumerator TimerAndCreateLine(string lineName, float delay)
    {
        yield return new WaitForSeconds(delay);
        CreateLine(lineName);
    }
}*/

// ScoreManager.cs — ПОЛНАЯ ЗАМЕНА класса на этот вариант
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GameObject bonusScorePrefab;
    [SerializeField] TextMeshProUGUI scoreText;

    Player player;
    ScoreHander scoreHander;

    float mainScore;

    // ----- Конфиг на тип бонуса -----
    enum BonusKind { Continuous, OneShot }
    class BonusConfig
    {
        public BonusKind kind;
        public string key;            // "sand" | "speed" | "obstacle"
        public float inDelay;         // задержка входа
        public float outDelay;        // задержка выхода
        public BonusType uiType;      // что положить в BonusScoreUI.bonusType
    }

    Dictionary<BonusType, BonusConfig> cfg;

    // ----- Состояние непрерывных -----
    class ContinuousState
    {
        public bool isActiveLogic;         // текущее логическое состояние (из ScoreHander)
        public bool isShown;               // линия создана и показана
        public float inT;                  // таймер входа
        public float outT;                 // таймер выхода
        public GameObject instance;        // текущая UI-линия
    }

    Dictionary<BonusType, ContinuousState> cont = new();

    // Все активные линии для выравнивания (и одноразовые, и непрерывные)
    readonly List<GameObject> allLines = new();

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        scoreHander = GameObject.Find("Player").GetComponent<ScoreHander>();

        // Конфиги: правь цифры в одном месте
        cfg = new Dictionary<BonusType, BonusConfig>()
        {
            { BonusType.nearSand, new BonusConfig{ kind=BonusKind.Continuous, key="sand", inDelay=0.2f, outDelay=0.3f, uiType=BonusType.nearSand } },
            { BonusType.highSpeed, new BonusConfig{ kind=BonusKind.Continuous, key="speed", inDelay=0.1f, outDelay=0.5f, uiType=BonusType.highSpeed } },
            // ВНИМАНИЕ: nearObstacle — теперь OneShot. Он спавнится по пульсу ScoreHander.OnNearObstaclePulse
            { BonusType.nearObstacle, new BonusConfig{ kind=BonusKind.OneShot, key="obstacle", inDelay=0.05f, outDelay=1f, uiType=BonusType.nearObstacle } },
        };

        // Инициализация трекеров непрерывных
        cont[BonusType.nearSand]  = new ContinuousState { inT = cfg[BonusType.nearSand].inDelay,  outT = cfg[BonusType.nearSand].outDelay };
        cont[BonusType.highSpeed] = new ContinuousState { inT = cfg[BonusType.highSpeed].inDelay, outT = cfg[BonusType.highSpeed].outDelay };
    }

    void OnEnable()
    {
        // Подписки на «ошибки»
        scoreHander.OnLeftRoad += HandleLeftRoad;          // сброс всех непрерывных без начисления
        scoreHander.OnObstacleHit += HandleObstacleHit;    // гашение/запрет по твоим правилам

        // Пульс одноразового события:
        scoreHander.OnNearObstaclePulse += HandleNearObstaclePulse;
    }

    void OnDisable()
    {
        scoreHander.OnLeftRoad -= HandleLeftRoad;
        scoreHander.OnObstacleHit -= HandleObstacleHit;
        scoreHander.OnNearObstaclePulse -= HandleNearObstaclePulse;
    }

    void Update()
    {
        UpdateMainScore();
        UpdateContinuous(BonusType.nearSand,  scoreHander.NearSand);
        UpdateContinuous(BonusType.highSpeed, scoreHander.HighSpeed);
        // nearObstacle НЕ обновляем тут как «continuous» — он обрабатывается событиями-пульсами
    }

    // ---------- Непрерывные ----------
void UpdateContinuous(BonusType type, bool logicActive)
{
    var s = cont[type];
    var c = cfg[type];

    s.isActiveLogic = logicActive;

    if (logicActive)
    {
        // антидребезг: пока логика активна — держим таймер выхода «полным»
        s.outT = c.outDelay;

        if (!s.isShown)
        {
            s.inT -= Time.deltaTime;
            if (s.inT <= 0f)
            {
                s.instance = CreateLine(c.uiType);
                s.isShown = true;
                // после показа— обнулим входной таймер к дефолту
                s.inT = c.inDelay;
            }
        }
    }
    else
    {
        // состояние не активно
        if (s.isShown)
        {
            s.outT -= Time.deltaTime;
            if (s.outT <= 0f)
            {
                // аккуратное закрытие с зачислением
                StartCoroutine(CloseContinuousAndCredit(type));
                // s.instance обнулим внутри корутины (см. правку выше),
                // а здесь подготовим входной таймер для следующего входа
                s.inT = c.inDelay;
            }
        }
        else
        {
            // линия не показана — восстанавливаем входной таймер
            s.inT = c.inDelay;
        }
    }
}

IEnumerator CloseContinuousAndCredit(BonusType type)
{
    var s = cont[type];
    var go = s.instance;
    s.instance = null;      // <— срезаем повторные закрытия
    s.isShown = false;

    if (go == null) yield break;
    var ui = go.GetComponent<BonusScoreUI>();
    int gained = ui != null ? ui.TakeScoreAndReset() : 0;
    mainScore += gained;

    if (ui != null)
        yield return ui.CloseAndDestroy(credited:true, delay:0.3f);

    RemoveFromStacks(go);
}

    // ---------- Одноразовый nearObstacle ----------
    void HandleNearObstaclePulse()
    {
        // Создаём ЛИНИЮ ТОЛЬКО если не было удара — это условие уже проверяет ScoreHander (hadCollisionWhileNearObstacle=false)
        var go = CreateLine(BonusType.nearObstacle);

        // такая линия живёт сама по себе: краткая анимация/набор до 30 — затем закрытие с зачислением
        StartCoroutine(AutoCloseOneShot(go, credit:true, life: 0.6f)); // life можно подправить
    }

    IEnumerator AutoCloseOneShot(GameObject go, bool credit, float life)
    {
        yield return new WaitForSeconds(life);

        if (go == null) yield break;
        var ui = go.GetComponent<BonusScoreUI>();
        int gained = credit ? ui.TakeScoreAndReset() : 0;
        mainScore += gained;

        yield return ui.CloseAndDestroy(credited:credit, delay:0.3f);
        RemoveFromStacks(go);
    }

    // ---------- Ошибки / Сбросы ----------
    void HandleLeftRoad()
    {
        // Сброс ТОЛЬКО непрерывных: отменяем без зачисления
        CancelContinuous(cont[BonusType.nearSand]);
        CancelContinuous(cont[BonusType.highSpeed]);
    }

    void HandleObstacleHit()
    {
        // (Вариант) При ударе отменяем непрерывные без начисления
        CancelContinuous(cont[BonusType.nearSand]);
        CancelContinuous(cont[BonusType.highSpeed]);

        // А «одноразовые» nearObstacle просто не создаются, т.к. suppress встроен в Hander (см. hadCollisionWhileNearObstacle)
        // Если хочешь ещё и текущие уже появившиеся «one-shot» погасить без очков — раскомментируй:
        // CancelAllOneShotsWithoutCredit();
    }

    void CancelContinuous(ContinuousState s)
    {
        if (s == null || !s.isShown || s.instance == null) return;
        StartCoroutine(CancelLineNoCredit(s));
    }

IEnumerator CancelLineNoCredit(ContinuousState s)
{
    var go = s.instance;
    s.instance = null;      // <— срезаем повторные закрытия
    s.isShown = false;

    if (go == null) yield break;

    var ui = go.GetComponent<BonusScoreUI>();
    if (ui != null)
        yield return ui.CloseAndDestroy(credited:false, delay:0.3f);

    RemoveFromStacks(go);

    // сброс таймеров
    foreach (var kv in cont)
    {
        if (kv.Value == s)
        {
            var c = cfg[kv.Key];
            s.inT = c.inDelay;
            s.outT = c.outDelay;
            break;
        }
    }
}

    // ---------- Общие утилиты ----------
    GameObject CreateLine(BonusType type)
    {
        var go = Instantiate(bonusScorePrefab);

        // пододвинуть уже висящие линии вверх
        if (allLines.Count > 0)
            foreach (var line in allLines)
                line.GetComponent<LineMover>()?.StartMove("up");

        // позиция/иерархия
        go.transform.position = player.transform.position;
        go.transform.SetParent(player.transform);

        // инициализация UI
        var ui = go.GetComponent<BonusScoreUI>();
        ui.bonusType = type;
        ui.animator.SetBool("event", true);

        allLines.Add(go);
        return go;
    }

    void RemoveFromStacks(GameObject go)
    {
        allLines.Remove(go);

        // опустить оставшиеся линии
        if (allLines.Count > 0)
            foreach (var line in allLines)
                line.GetComponent<LineMover>()?.StartMove("down");
    }

    void UpdateMainScore()
    {
        mainScore += player.Velocity * Time.deltaTime;
        scoreText.text = "Score: " + (int)mainScore;
    }
}

