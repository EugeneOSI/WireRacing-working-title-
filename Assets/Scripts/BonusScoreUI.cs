using UnityEngine;
using TMPro;

public class BonusScoreUI : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] TextMeshProUGUI amount;
    [SerializeField] TextMeshProUGUI description;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void SetBonus(string eventName)
    {
        switch (eventName)
        {
            
        }

    }
}
