using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PursuingEnemy pursuingEnemy;
    [SerializeField] private float difficultyIncreaseInterval = 10f;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private TextMeshProUGUI healthAlert;
    float time;
    const float difficultyK = 0.0025f;

    bool gameStarted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(DifficultyIncrease());
        healthAlert.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //time = Time.timeSinceLevelLoad;
        //SetDifficulty();
        if (player.Health < 2){
            healthAlert.gameObject.SetActive(true);
        }
        else{healthAlert.gameObject.SetActive(false);}
        
        switch (player.isAlive)
        {
            case false:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }
    float GetDifficulty(float t, float start, float max, float k)
{
    return start + (max - start) * (1f - Mathf.Exp(-k * t));
}

void SetDifficulty(){
    player.attractionForce = GetDifficulty(time, 18, 25, difficultyK);
    playerRb.linearDamping = GetDifficulty(time, 0.3f, 0.6f, difficultyK);
    spawnManager.obstacleCount = (int)GetDifficulty(time, 5, 20, difficultyK);
    pursuingEnemy.defaultSpeed = GetDifficulty(time, 12, 18, difficultyK);
}

    IEnumerator DifficultyIncrease()
    {
        while (true){
        yield return new WaitForSeconds(difficultyIncreaseInterval);
        player.attractionForce += 0.4f;
        playerRb.linearDamping += 0.01f;
        spawnManager.obstacleCount++;
        pursuingEnemy.defaultSpeed += 0.3f;}
    }

public bool GameStarted{
    get {return gameStarted;}
    set {gameStarted = value;}}
}
