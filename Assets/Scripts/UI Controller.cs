using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.UI;
public class UICOntroller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TextMeshProUGUI healthAlert;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject ScoreUI;
    [SerializeField] private GameObject UnderBoardInformation; 
    [SerializeField] private GameObject InputField; 
    [SerializeField] public TextMeshProUGUI currentScore;
    [SerializeField] public TextMeshProUGUI bestScore;
    [SerializeField] public TextMeshProUGUI enptyFieldAlert;
    [SerializeField] public TextMeshProUGUI textfieldDescription;
    [SerializeField] public TextMeshProUGUI playerName;
    [SerializeField] public TextMeshProUGUI playerPosition;
    [SerializeField] public TextMeshProUGUI deleteEntryText;
    [SerializeField] public GameObject loadingPanel;

    
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        public void LoadScene(string sceneName){
        SceneManager.LoadScene(sceneName);
     }

     public void ActivateUI(string uiName, bool activate){
        switch (uiName){
            case "healthAlert":
                healthAlert.gameObject.SetActive(activate);
                break;
            case "pauseMenu":
                pauseMenu.gameObject.SetActive(activate);
                break;
            case "gameOverMenu":
                gameOverMenu.gameObject.SetActive(activate);
                break;
            case "ScoreUI":
                ScoreUI.gameObject.SetActive(activate);
                break;
            case "enptyFieldAlert":
                enptyFieldAlert.gameObject.SetActive(activate);
                break;
            case "textfieldDescription":
                textfieldDescription.gameObject.SetActive(activate);
                break;
            case "UnderBoardInformation":
                UnderBoardInformation.gameObject.SetActive(activate);
                break;
            case "InputField":
                InputField.gameObject.SetActive(activate);
                break;
            case "deleteEntryText":
                deleteEntryText.gameObject.SetActive(activate);
                break;
            case "loadingPanel":
                loadingPanel.gameObject.SetActive(activate);
                break;
            default:
                Debug.LogError("UI not found: " + uiName);
                break;
                
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

    public void MakeButtonInactive(GameObject button){
        button.GetComponent<UnityEngine.UI.Button>().interactable = false;

    }
}
