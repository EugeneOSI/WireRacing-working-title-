using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundsVolumeSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    void Start()
    {
        
        musicVolumeSlider.value = PrefsManager.Instance.musicVolume;
        soundsVolumeSlider.value = PrefsManager.Instance.soundsVolume;
    }
    void Update()
    {
        PrefsManager.Instance.musicVolume = musicVolumeSlider.value ;
        PrefsManager.Instance.soundsVolume = soundsVolumeSlider.value;
    }
}
