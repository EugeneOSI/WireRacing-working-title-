using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "LeaderBoardData", menuName = "Scriptable Objects/LeaderBoardData")]
public class LeaderBoardData : ScriptableObject
{
    public GameObject enteryPrefab;
    public ScrollRect scrollRect;
    public Transform entryParent;
}
