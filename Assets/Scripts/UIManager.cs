using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance {get; private set;}

    public List<GameObject> ActiveScreens = new List<GameObject>();
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        GameManager.LoadSceneEvent += OnLoadScene;
        GameManager.OnUnpauseEvent += DeactivateActiveScreen;
        GameManager.whilePausedEvent += DeactivateActiveScreen;
    }
    void OnDestroy()
    {
        GameManager.LoadSceneEvent -= OnLoadScene;
        GameManager.OnUnpauseEvent -= DeactivateActiveScreen;
        GameManager.whilePausedEvent -= DeactivateActiveScreen;
    }

    void Update(){
            /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ActiveScreens.Count > 0)
            {
                SwitchVisibilty(ActiveScreens[ActiveScreens.Count - 1]);
            }
        }*/
    }
    public void SwitchVisibilty(GameObject ui){
        ui.SetActive(!ui.activeSelf);
        if (ui.activeSelf)
        {
            ActiveScreens.Add(ui);
        }
        else
        {
            ActiveScreens.Remove(ui);
        }
    }
    
    public IEnumerator SwitchVisibiltyForSeconds(GameObject ui, float seconds){
        SwitchVisibilty(ui);
        yield return new WaitForSeconds(seconds);
        SwitchVisibilty(ui);
    }
    public void DeactivateActiveScreen(){
        if (ActiveScreens.Count > 0)
        {
            SwitchVisibilty(ActiveScreens[ActiveScreens.Count - 1]);
        }
        }
    public void SetVisibilty(GameObject ui, bool visible){
        ui.SetActive(visible);
    }

    public void SetText(GameObject ui, string text){
        ui.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void SwitchButtonInteractable(GameObject button){
        button.GetComponent<UnityEngine.UI.Button>().interactable = !button.GetComponent<UnityEngine.UI.Button>().interactable;
    }
    public void SetButtonInteractable(GameObject button, bool interactable){
        button.GetComponent<UnityEngine.UI.Button>().interactable = interactable;
    }

    public IEnumerator SwitchButtonInteractableForSeconds(GameObject button, float seconds){
        yield return new WaitForSeconds(seconds);
        SwitchButtonInteractable(button);
    }
    public string FormatTime(float time, string type)
    {
        int minutes = (int)(time / 60f);
        float seconds = time - minutes * 60f;
        string result = "";
        switch (type){
            case "lap":
                if (time <= 0f) return "--:--.---";
               result = string.Format("{0:00}:{1:00.000}", minutes, seconds);
               break;
            case "delta":
            result = string.Format("{0:0.000}", time);
             break;
        }
        return result;

    }
    public void OnLoadScene(){
        ActiveScreens.Clear();
    }
}

