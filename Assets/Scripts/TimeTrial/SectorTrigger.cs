using UnityEngine;

public class SectorTrigger : MonoBehaviour
{
public int sectorIndex;

private void OnTriggerEnter2D(Collider2D other)
{
    if (!other.CompareTag("Player")) return;

    TimeTrialManager.Instance.OnSectorCrossed(sectorIndex);
}
}
