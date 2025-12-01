using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeTrialMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            gameObject.SetActive(false);
        }
    }
        public void LoadScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }

}
