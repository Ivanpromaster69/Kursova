using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources (Optional)")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadVolumeSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        if (musicSource) musicSource.volume = musicVolume;
        if (sfxSource) sfxSource.volume = sfxVolume;
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        if (musicSource) musicSource.volume = value;
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        if (sfxSource) sfxSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
}