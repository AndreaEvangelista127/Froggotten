using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {  get; private set; }
    
    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip clickSound;

    void Start()
    {
        // Load the previusly saved volume
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        musicSource.volume = savedVolume;
        volumeSlider.value = savedVolume;
        UpdateVolumeText(savedVolume);

        // when the slider value changes, call ChangeVolume
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    public void ChangeVolume(float volume)
    {
        musicSource.volume = volume;
        UpdateVolumeText(volume);

        // Salva il volume
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    void UpdateVolumeText(float volume)
    {
        // Converti in percentuale (0-100)
        int volumePercent = Mathf.RoundToInt(volume * 100);
        volumeText.text = $"{volumePercent}%";
    }

    public void PlayClickSound()
    {
        sfxSource.PlayOneShot(clickSound);
    }

    public void PauseMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.UnPause();
        }
    }
}