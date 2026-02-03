using UnityEngine;
using System;
public class DirectionTracker : MonoBehaviour
{
    [SerializeField] private DirectionMarker[] directionMarkers;

    public int expectedIndex;
    public bool correctDirection;
    public static event Action onWrongDirection;
    public static event Action onCorrectDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        expectedIndex = 37;
        //resetDirectionMarkers();
        //TimeTrialManager.NewLapStarted += () => ResetIndex();
        //PlayerTimeTrial.OnCrossedFirstMarker += () => resetDirectionMarkers();
        onCorrectDirection += () => correctDirection = true;
        onWrongDirection += () => correctDirection = false;
        //TimeTrialManager.CrossedFinishLineWrongSide += () => passAllDirectionMarkers();
    }
    void OnDestroy()
    {
        //TimeTrialManager.NewLapStarted -= () => ResetIndex();
        //PlayerTimeTrial.OnCrossedFirstMarker -= () => resetDirectionMarkers();
        //TimeTrialManager.CrossedFinishLineWrongSide -= () => passAllDirectionMarkers();
        //TimeTrialManager.CrossedFinishLineWrongSide -= () => passAllDirectionMarkers();
    }
    
    public void CheckMarkerIndex (int index){
        if (index == expectedIndex){
            onCorrectDirection?.Invoke();
            expectedIndex = index + 1;
        }
        else{
            onWrongDirection?.Invoke();
            expectedIndex = index;
        }
        
    }

    public void ResetIndex(){
        expectedIndex = 0;
    }
    /*void resetDirectionMarkers()
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
    }*/
}

