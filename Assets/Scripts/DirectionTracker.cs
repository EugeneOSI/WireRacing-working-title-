using UnityEngine;

public class DirectionTracker : MonoBehaviour
{
    [SerializeField] private DirectionMarker[] directionMarkers;
    public bool correctDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //resetDirectionMarkers();
        //TimeTrialManager.NewLapStarted += () => resetDirectionMarkers();
        PlayerTimeTrial.OnCrossedFirstMarker += () => resetDirectionMarkers();
        DirectionMarker.onCorrectDirection += () => correctDirection = true;
        DirectionMarker.onWrongDirection += () => correctDirection = false;
        TimeTrialManager.CrossedFinishLineWrongSide += () => passAllDirectionMarkers();
    }
    void OnDestroy()
    {
        //TimeTrialManager.NewLapStarted -= () => resetDirectionMarkers();
        PlayerTimeTrial.OnCrossedFirstMarker -= () => resetDirectionMarkers();
        TimeTrialManager.CrossedFinishLineWrongSide -= () => passAllDirectionMarkers();
        TimeTrialManager.CrossedFinishLineWrongSide -= () => passAllDirectionMarkers();
    }
    void resetDirectionMarkers()
    {
    foreach (DirectionMarker directionMarker in directionMarkers)
    {
        directionMarker.passed = false;
    }}
    void passAllDirectionMarkers()
    {
    foreach (DirectionMarker directionMarker in directionMarkers)
    {
        directionMarker.passed = true;
    }
    }
}

