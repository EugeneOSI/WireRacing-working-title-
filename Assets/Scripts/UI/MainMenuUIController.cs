using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
public class MainMenuUIController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> activeScreens = new List<GameObject>();
    
    [SerializeField] private LeaderBoardData leaderBoardData;
    [SerializeField] public GameObject optionsMenu;
    [SerializeField] private GameObject gameModeMenu;
    [SerializeField] private GameObject timeTrialMenu;

    [SerializeField] public GameObject loadingPanel;
    [SerializeField] public GameObject submitScoreButton;
    [SerializeField] public GameObject underBoardInformation;
    [SerializeField] public GameObject inputField;
    [SerializeField] public GameObject fieldAlert;
    [SerializeField] public GameObject playerName;
    [SerializeField] public GameObject playerPosition;
    [SerializeField] public GameObject bestMonzaTime;
    [SerializeField] public GameObject monzalaps;
    [SerializeField] public GameObject deleteEntryText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (activeScreens.Count > 0)
            {
                SwitchScreenActive(activeScreens[activeScreens.Count - 1]);
            }
        }
    }

    public void SwitchScreenActive(GameObject screen){
        screen.SetActive(!screen.activeSelf);
        if (screen.activeSelf)
        {
            activeScreens.Add(screen);
        }
        else
        {
            activeScreens.Remove(screen);
        }
    }

    public void ActivateUI(GameObject ui, bool activate){
        ui.SetActive(activate);
    }

    public void SetText(GameObject ui, string text){
        ui.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void SwitchButtonInteractable(GameObject button){
        button.GetComponent<UnityEngine.UI.Button>().interactable = !button.GetComponent<UnityEngine.UI.Button>().interactable;
    }

    public IEnumerator SwitchButtonInteractableForSeconds(GameObject button, float seconds){
        yield return new WaitForSeconds(seconds);
        SwitchButtonInteractable(button);
    }
    public void QuitGame(){
        Application.Quit();
    }

        public IEnumerator SwitchUIForSeconds(GameObject ui, float seconds){
        SwitchScreenActive(ui);
        yield return new WaitForSeconds(seconds);
        SwitchScreenActive(ui);
    }

private void SetUnderBoardInformation(){


}


}
