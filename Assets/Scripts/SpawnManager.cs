using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    Transform currentCameraPos;

    public GameObject obstacle;

    public float ySpawnRange;
    public float spawnTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentCameraPos = GameObject.Find("Main Camera").GetComponent<Transform>();
        StartCoroutine(SpawnObstacle());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnObstacle()
    {
        while (true)
        {
            float ySpawnPos = Random.Range(-ySpawnRange, ySpawnRange);
            Instantiate(obstacle, new Vector2(currentCameraPos.position.x + 11f, ySpawnPos), Quaternion.identity);
            yield return new WaitForSeconds(spawnTime);
        }
    }
}
