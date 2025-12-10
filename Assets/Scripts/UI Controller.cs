using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.UI;
using System.Collections.Generic;
public class UICOntroller : MonoBehaviour
{
    public static UICOntroller Instance { get; private set; }
    
    [Header("Menus//Screens")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] public GameObject optionsMenu;
    [SerializeField] private GameObject gameModeMenu;
    [SerializeField] private GameObject timeTrialMenu;
    [SerializeField] private GameObject ScoreUI;
    public List<GameObject> activeMenus = new List<GameObject>();

    [Header("Panels")]
    [SerializeField] private GameObject underBoardInformation;
    [SerializeField] private GameObject inputField;  
     [SerializeField] public GameObject loadingPanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI healthAlert;
    [SerializeField] public TextMeshProUGUI currentScore;
    [SerializeField] public TextMeshProUGUI bestScore;
    [SerializeField] public TextMeshProUGUI fieldAlert;
    [SerializeField] public TextMeshProUGUI textFieldDescription;
    [SerializeField] public TextMeshProUGUI playerName;
    [SerializeField] public TextMeshProUGUI playerPosition;
    [SerializeField] public TextMeshProUGUI deleteEntryText;

    [Header("Buttons")]
    [SerializeField] public GameObject submitScoreButton;
    


    


    
    void Awake()
    {
            var all = FindObjectsOfType<UICOntroller>(true);
    Debug.Log($"[UIManager] Awake on {gameObject.name}, id={GetInstanceID()}, " +
              $"current Instance={(Instance == null ? "null" : Instance.gameObject.name)}");
    Debug.Log($"[UIManager] Всего объектов UIManager в сцене: {all.Length}");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else{
            Instance = this;
        }

        
        DontDestroyOnLoad(gameObject);

        activeMenus = new List<GameObject>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            DeactivateActiveMenu();
        }
    }
        public void LoadScene(string sceneName){
        SceneManager.LoadScene(sceneName);
     }

     public void SwitchUI(GameObject ui){
        ui.gameObject.SetActive(!ui.gameObject.activeSelf);
     }
     public void ActivateUI(string uiName, bool activate){
        switch (uiName){

            //panels
            case "ScoreUI":
                ScoreUI.gameObject.SetActive(activate);
                break;
            case "underBoardInformation":
                underBoardInformation.gameObject.SetActive(activate);
                break;
            case "inputField":
                inputField.gameObject.SetActive(activate);
                break;
            case "loadingPanel":
                loadingPanel.gameObject.SetActive(activate);
                break;

            //texts
            case "healthAlert":
                healthAlert.gameObject.SetActive(activate);
                break;
            case "currentScore":
                currentScore.gameObject.SetActive(activate);
                break;
            case "bestScore":
                bestScore.gameObject.SetActive(activate);
                break;
            case "fieldAlert":
                fieldAlert.gameObject.SetActive(activate);
                break;
            case "textFieldDescription":
                textFieldDescription.gameObject.SetActive(activate);
                break;
            case "playerName":
                playerName.gameObject.SetActive(activate);
                break;
            case "playerPosition":
                playerPosition.gameObject.SetActive(activate);
                break;
            case "deleteEntryText":
                deleteEntryText.gameObject.SetActive(activate);
                break;

            //buttons
            case "submitScoreButton":
                submitScoreButton.gameObject.SetActive(activate);
                break;
            default:
                Debug.LogError("UI not found: " + uiName);
                break;
                
        }
     }

    public void ActivateMenu(string menuName){
        switch (menuName){
            case "pauseMenu":
                pauseMenu.gameObject.SetActive(true);
                activeMenus.Add(pauseMenu);
                break;
            case "optionsMenu":
                optionsMenu.gameObject.SetActive(true);
                activeMenus.Add(optionsMenu);
                break;
            case "gameModeMenu":
                gameModeMenu.gameObject.SetActive(true);
                activeMenus.Add(gameModeMenu);
                break;
            case "timeTrialMenu":
                timeTrialMenu.gameObject.SetActive(true);
                activeMenus.Add(timeTrialMenu);
                break;
            default:
                Debug.LogError("Menu not found: " + menuName);
                break;
        }
    }

    public void DeactivateActiveMenu(){
        if (activeMenus.Count > 0){
            activeMenus[activeMenus.Count - 1].gameObject.SetActive(false);
            activeMenus.RemoveAt(activeMenus.Count - 1);
        }
    }
    public void ActivateEmptyFieldAlert(){
        StartCoroutine(WaitForSeconds(2));
    }
    
    public IEnumerator WaitForSeconds(float seconds){
        ActivateUI("enptyFieldAlert", true);
        ActivateUI("textfieldDescription", false);
        yield return new WaitForSeconds(seconds);
        ActivateUI("enptyFieldAlert", false);
        ActivateUI("textfieldDescription", true);
    }

    public IEnumerator showDeleteEntryText(){
        ActivateUI("deleteEntryText", true);
        yield return new WaitForSeconds(5);
        ActivateUI("deleteEntryText", false);
    }

    public void SwitchButtonInteractable(GameObject button){
        button.GetComponent<UnityEngine.UI.Button>().interactable = !button.GetComponent<UnityEngine.UI.Button>().interactable;
    }

    public IEnumerator ActivateButtonForSeconds(GameObject button, float seconds){
        yield return new WaitForSeconds(seconds);
        SwitchButtonInteractable(button);
    }

}
