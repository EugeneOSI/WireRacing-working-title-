using UnityEngine;
using UnityEngine.EventSystems;
public class UI_MenuButton : MonoBehaviour
{
public void OnButtonClick(){
    EventSystem.current.SetSelectedGameObject(null);
}
}
