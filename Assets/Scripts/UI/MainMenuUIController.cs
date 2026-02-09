using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
public class MainMenuUIController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    
    public List<GameObject> activeScreens = new List<GameObject>();
    
    
    
    [SerializeField] public GameObject optionsMenu;
    [SerializeField] public GameObject guideScreen;





public void OpenOptionsMenu(){
    UIManager.Instance.SwitchVisibilty(optionsMenu);
}

public void StartEndlessMode(){
    GameManager.Instance.LoadScene("EndlessMode");
}
public void StartTimeTrial(string trackSceneName){
    GameManager.Instance.LoadScene(trackSceneName);
}
public void OpenGuideScreen(){
    UIManager.Instance.SwitchVisibilty(guideScreen);
}
public void ExitGame(){
    Application.Quit();
}
}
