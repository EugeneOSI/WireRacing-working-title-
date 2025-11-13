using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private PursuingEnemy pursuingEnemy;
    [SerializeField] private float difficultyIncreaseInterval = 10f;
    [SerializeField] private Rigidbody2D playerRb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(DifficultyIncrease());
    }

    // Update is called once per frame
    void Update()
    {
        switch (player.isAlive)
        {
            case false:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }

    IEnumerator DifficultyIncrease()
    {
        while (true){
        yield return new WaitForSeconds(difficultyIncreaseInterval);
        player.attractionForce += 0.4f;
        playerRb.linearDamping += 0.01f;
        spawnManager.obstacleCount++;
        pursuingEnemy.speed += 0.3f;}
    }
}
