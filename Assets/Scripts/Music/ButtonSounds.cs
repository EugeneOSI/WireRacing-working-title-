using UnityEngine;

public class ButtonSounds : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private AudioSource audioSource;

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = PrefsManager.Instance.soundsVolume;
    }
}
