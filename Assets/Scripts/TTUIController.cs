using UnityEngine;
using System.Collections.Generic;
public class TTUIController : MonoBehaviour
{
    [SerializeField] private TimeTrialManager timeTrialManager;
    [SerializeField] public GameObject pauseMenu;

    public List<GameObject> ActiveScreens = new List<GameObject>();
    
    public bool IsPaused {get; set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TimeTrialManager.OnPauseEvent += OnPauseEvent;
        TimeTrialManager.OnUnpauseEvent += DeactivateActiveScreen;
        TimeTrialManager.whilePausedEvent += DeactivateActiveScreen;
    }

    // Update is called once per frame
    void Update()
    {
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

    public void DeactivateActiveScreen(){
    if (ActiveScreens.Count > 0)
    {
            SwitchVisibilty(ActiveScreens[ActiveScreens.Count - 1]);
    }
    }

    public void OnPauseEvent(){
        SwitchVisibilty(pauseMenu);
    }
}
