using UnityEngine;

public class SFXVolumer : MonoBehaviour
{
    public AudioSource sfxSource;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.sfxSource = sfxSource;
            sfxSource.volume = AudioManager.Instance.GetSFXVolume();
        }
    }
}

