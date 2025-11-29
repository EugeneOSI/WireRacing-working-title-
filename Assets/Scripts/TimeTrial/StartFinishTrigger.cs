using UnityEngine;

public class StartFinishTrigger : MonoBehaviour
{
private void OnTriggerEnter2D(Collider2D other)
{
    if (!other.CompareTag("Player")) return;

    TimeTrialManager.Instance.OnStartFinishCrossed();
}
}
