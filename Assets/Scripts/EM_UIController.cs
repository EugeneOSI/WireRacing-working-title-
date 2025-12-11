using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
public class EM_UIController : MonoBehaviour
{
    public List<GameObject> activeScreens = new List<GameObject>();

    [SerializeField] private GameManagerEM gameManager;

    [Header("Screens")]
    [SerializeField] public GameObject pauseMenu;
    [SerializeField] public GameObject optionsMenu;
    [SerializeField] public GameObject gameOverMenu;
    [SerializeField] public GameObject ScoreUI;

    [Header("Panels")]
    [SerializeField] public GameObject underBoardInformation;
    [SerializeField] public GameObject inputField;  
     [SerializeField] public GameObject loadingPanel;

    [Header("Texts")]
    [SerializeField] public GameObject scoreText;
    [SerializeField] public GameObject healthAlert;
    [SerializeField] public GameObject currentScore;
    [SerializeField] public GameObject bestScore;
    [SerializeField] public GameObject fieldAlert;
    [SerializeField] public GameObject textFieldDescription;
    [SerializeField] public GameObject playerName;
    [SerializeField] public GameObject playerPosition;
    [SerializeField] public GameObject deleteEntryText;

    [Header("Buttons")]
    [SerializeField] public GameObject submitScoreButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeactivateActiveScreen();
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

    public void DeactivateActiveScreen(){
        if (activeScreens.Count > 0)
            {
                SwitchScreenActive(activeScreens[activeScreens.Count - 1]);
            }
        }
    
    public void ActivateUI(GameObject ui, bool activate){
        ui.SetActive(activate);
    }

    public IEnumerator SwitchUIForSeconds(GameObject ui, float seconds){
        SwitchScreenActive(ui);
        yield return new WaitForSeconds(seconds);
        SwitchScreenActive(ui);
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

    public void ResetPrefs(){
        PrefsManager.Instance.ResetPrefs();
    }
}
