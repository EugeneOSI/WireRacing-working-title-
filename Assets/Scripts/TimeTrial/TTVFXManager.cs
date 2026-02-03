using UnityEngine;
using System.Collections;

public class TTVFXManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("References")] 
    [SerializeField] private PlayerTimeTrial player;
        [SerializeField] private Transform finishLine;

    [Header("VFX")]
    [SerializeField] private GameObject barierHitVFX;
    [SerializeField] private GameObject outOnTrackVFX;
    [SerializeField] private GameObject offRoadText;
    [SerializeField] private GameObject bestTimeVFX;

    void Start()
    {
        PlayerTimeTrial.BarierHitTT += OnBarierHitTT;
        PlayerTimeTrial.OutOnTrackTT += OnOutOnTrackTT;
        TimeTrialManager.BestTimeUpdated += OnBestTimeUpdated;
    }
    void OnDestroy()
    {
        PlayerTimeTrial.BarierHitTT -= OnBarierHitTT;
        PlayerTimeTrial.OutOnTrackTT -= OnOutOnTrackTT;
        TimeTrialManager.BestTimeUpdated -= OnBestTimeUpdated;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void OnBarierHitTT(Vector2 collisionPoint)
    {
        var vfx = Instantiate(barierHitVFX);
        vfx.transform.position = collisionPoint;
        StartCoroutine(TimerAndDestroy(1f, vfx.gameObject));
    }
    void OnOutOnTrackTT()
    {
        var vfx = Instantiate(outOnTrackVFX);
        GameObject text = Instantiate(offRoadText);
        text.transform.position = player.transform.position;
        text.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Active");
        vfx.transform.position = player.transform.position;
        StartCoroutine(TimerAndDestroy(1f, vfx.gameObject));
        StartCoroutine(TimerAndDestroyText(2f, text, "Unactive"));
    }
    void OnBestTimeUpdated()
    {
        var vfx = Instantiate(bestTimeVFX);
        vfx.transform.position = finishLine.position + new Vector3(0, 5, 0);
        var vfx2 = Instantiate(bestTimeVFX);
        vfx2.transform.position = finishLine.position - new Vector3(0, 5, 0);
        StartCoroutine(TimerAndDestroy(1f, vfx.gameObject));
        StartCoroutine(TimerAndDestroy(1f, vfx2.gameObject));
    }
    IEnumerator TimerAndDestroy(float time, GameObject vfx)
    {
        yield return new WaitForSeconds(time);
        Destroy(vfx);
    }
        IEnumerator TimerAndDestroyText(float time, GameObject text, string animTrigger){
        yield return new WaitForSeconds(time);
        text.transform.GetChild(0).GetComponent<Animator>().SetTrigger(animTrigger);
        yield return new WaitForSeconds(1f);
        Destroy(text);

    }
}
