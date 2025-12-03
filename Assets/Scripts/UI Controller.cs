using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class UICOntroller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private TextMeshProUGUI healthAlert;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject ScoreUI;
    [SerializeField] public TextMeshProUGUI currentScore;
    [SerializeField] public TextMeshProUGUI bestScore;
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
            default:
                Debug.LogError("UI not found: " + uiName);
                break;
        }
     }
}
