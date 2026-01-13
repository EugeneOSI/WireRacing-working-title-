using UnityEngine;
using UnityEngine.UI;

public class TabsController : MonoBehaviour
{
    
    public GameObject[] tabs;
    public Image[] TabButtons;
    public Sprite InactiveTabBG, ActiveTabBG;
    public Vector2 InactiveTabButtonSize, ActiveTabButtonSize;

    public void SwitchToTab(int TabID){
        foreach (GameObject go in tabs){
            go.SetActive(false);
        }
        tabs[TabID].SetActive(true);

        foreach (Image img in TabButtons){
            img.sprite = InactiveTabBG;
            img.rectTransform.sizeDelta = InactiveTabButtonSize;
        }
        TabButtons[TabID].sprite = ActiveTabBG;
        TabButtons[TabID].rectTransform.sizeDelta = ActiveTabButtonSize;
    }

}
