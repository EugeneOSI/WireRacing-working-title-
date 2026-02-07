using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance {get; private set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
}
