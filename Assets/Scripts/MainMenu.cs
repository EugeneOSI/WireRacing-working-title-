using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetScreen(string screenName, bool state){
        GameObject.Find(screenName).SetActive(state);
    }
    
    public void QuitGame(){
        Application.Quit();
    }


}
