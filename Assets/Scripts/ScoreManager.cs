using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    float mainScore;

    private Player player;
    public TextMeshPro scoreText;

    private Rigidbody2D playerRb;
    void Start()
    {
        mainScore = 0;

        player = GameObject.Find("Player").GetComponent<Player>();
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        float playerSpeed = player.GetCurrentSpeed;
        if (playerSpeed >= 0) mainScore += Time.deltaTime;
        
    }
}
