using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    Player hookSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hookSystem = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (hookSystem.isAlive)
        {
            case false:
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }
}
