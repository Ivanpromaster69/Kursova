using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    
    void Start()
    {
        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
    }
}
