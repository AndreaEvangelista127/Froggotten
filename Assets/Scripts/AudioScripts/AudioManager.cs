using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private TextMeshProUGUI _volumeText;

    [Header("SFX")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI _sfxVolumeText;
    [SerializeField] private AudioClip _sfxPreviewSound;
    [SerializeField][Range(0f, 1f)] private float _sfxPreviewVolume = 0.8f;

    [Header("Player Jump")]
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField][Range(0f, 1f)] private float _jumpVolume = 0.8f; 
    [SerializeField] private AudioClip _doubleJumpSound;
    [SerializeField][Range(0f, 1f)] private float _doubleJumpVolume = 0.9f; 
    [SerializeField] private AudioClip _wallJumpSound;
    [SerializeField][Range(0f, 1f)] private float _wallJumpVolume = 0.85f; 

    [Header("Player Footsteps")]
    [SerializeField] private AudioSource _footstepSource;
    [SerializeField] private AudioClip _footstepSound;
    [SerializeField][Range(0f, 1f)] private float _footstepVolume = 0.3f;

    [Header("Player Health SFX")]
    [SerializeField] private AudioClip _takeDamageSound;
    [SerializeField][Range(0f, 1f)] private float _takeDamageVolume = 1f; 
    [SerializeField] private AudioClip _deathSound;
    [SerializeField][Range(0f, 1f)] private float _deathVolume = 1f; 

    [Header("Checkpoint SFX")]
    [SerializeField] private AudioClip _checkpointSound;
    [SerializeField][Range(0f, 1f)] private float _checkpointVolume = 0.7f; 

    [Header("Collectibles SFX")]
    [SerializeField] private AudioClip _collectibleSound;
    [SerializeField][Range(0f, 1f)] private float _collectibleVolume = 0.6f;

    [Header("Win SFX")]
    [SerializeField] private AudioClip _winSound;
    [SerializeField][Range(0f, 1f)] private float _winVolume = 0.8f;

    [Header("Enemy SFX")]
    [SerializeField] private AudioClip _enemyShootSound;
    [SerializeField][Range(0f, 1f)] private float _enemyShootingVolume = 0.6f;
    [SerializeField] private AudioClip _enemyDeathSound;
    [SerializeField][Range(0f, 1f)] private float _enemyDeathVolume = 0.6f;

    [Header("References")]
    [SerializeField] private StatePlayerMovement _statePlayerMovement;

    private StatePlayerMovement.MoveState _currentState = StatePlayerMovement.MoveState.None;

    private Coroutine _sfxPreviewCoroutine;

    public static AudioManager Instance { get; private set; }

    protected void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);


    }

    private void Start()
    {
        // ========= BACKGROUND MUSIC SETUP ==========
        // Load the previusly saved volume
        if (_musicSource != null && _volumeSlider != null)
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f); 
            _musicSource.volume = savedVolume;
            _volumeSlider.value = savedVolume;
            UpdateVolumeText(savedVolume);

            // when the slider value changes, call ChangeVolume
            _volumeSlider.onValueChanged.AddListener(ChangeVolume);
        }

        // ========= SFX SETUP ==========
        if(_sfxSource != null && _sfxVolumeSlider != null)
        {
            float savedSfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            _sfxSource.volume = savedSfxVolume;
            _sfxVolumeSlider.value = savedSfxVolume;
            UpdateSFXVolumeText(savedSfxVolume);

            _sfxVolumeSlider.onValueChanged.AddListener(ChangeSFXVolume);
        }

        // Setup footstep audio source
        if (_footstepSource != null)
        {
            float savedSfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            _footstepSource.volume = _footstepVolume * savedSfxVolume;
            _footstepSource.clip = _footstepSound;
            _footstepSource.loop = true;
        }

    }

    private void Update()
    {
        if (_statePlayerMovement == null) return;

        StatePlayerMovement.MoveState newState = _statePlayerMovement.currentMoveState;
        if (newState != _currentState)
        {
            HandleFootstepStateChange(newState);
            _currentState = newState;
        }

    }

    /// <summary>
    /// Reacts to player movement state changes, starting or stopping footsteps.
    /// </summary>
    /// <param name="newState">The new movement state the player has entered.</param>
    private void HandleFootstepStateChange(StatePlayerMovement.MoveState newState)
    {
        if (newState == StatePlayerMovement.MoveState.Run)
            StartFootsteps();
        else
            StopFootsteps();
    }

    // ========== FOOTSTEP PLAY ==========
    private void StartFootsteps()
    {
        if (_footstepSource != null && !_footstepSource.isPlaying)
        {
            _footstepSource.Play();
        }
    }

    public void StopFootsteps()
    {
        if (_footstepSource != null && _footstepSource.isPlaying)
        {
            _footstepSource.Stop();
        }
    }

    // ========== JUMP SFX ==========

    public void PlayJumpSound()
    {
        if (_sfxSource != null && _jumpSound != null)
        {
            _sfxSource.PlayOneShot(_jumpSound, _jumpVolume);
        }
    }

    public void PlayDoubleJumpSound()
    {
        if (_sfxSource != null && _doubleJumpSound != null)
        {
            _sfxSource.PlayOneShot(_doubleJumpSound, _doubleJumpVolume);
        }
    }

    public void PlayWallJumpSound()
    {
        if (_sfxSource != null && _wallJumpSound != null)
        {
            _sfxSource.PlayOneShot(_wallJumpSound, _wallJumpVolume);
        }
    }

    // ========== HEALTH SFX ==========

    public void PlayTakeDamageSound()
    {
        if (_sfxSource != null && _takeDamageSound != null)
        {
            _sfxSource.PlayOneShot(_takeDamageSound, _takeDamageVolume);
        }
    }

    public void PlayDeathSound()
    {
        if (_sfxSource != null && _deathSound != null)
        {
            _sfxSource.PlayOneShot(_deathSound, _deathVolume);
        }
    }

    // ========== CHECKPOINT SFX ==========

    public void PlayCheckpointSound()
    {
        if (_sfxSource != null && _checkpointSound != null)
        {
            _sfxSource.PlayOneShot(_checkpointSound, _checkpointVolume);
        }
    }

    // ========== COLLECTIBLE SFX ==========
    public void PlayCollectibleSound()
    {
        if (_sfxSource != null && _collectibleSound != null)
        {
            _sfxSource.PlayOneShot(_collectibleSound, _collectibleVolume);
        }
    }

    // ========== ENEMY SFX ==========

    public void PlayEnemyShootSound()
    {
        if (_sfxSource != null && _enemyShootSound != null)
        {
            _sfxSource.PlayOneShot(_enemyShootSound, _enemyShootingVolume);
        }
    }

    public void PlayEnemyDeathSound()
    {
        if(_sfxSource != null && _enemyDeathSound != null)
        {
            Debug.Log("Playing enemy death sound");
            _sfxSource.PlayOneShot(_enemyDeathSound, _enemyDeathVolume);
        }
    }

    // ========== WIN SFX ==========

    public void PlayWinSound()
    {
        if (_sfxSource != null && _winSound != null)
        {
            Debug.Log("Playing win sound");
            _sfxSource.PlayOneShot(_winSound, _winVolume);
        }
    }

    // ========== MUSIC CONTROLS ==========
    /// <summary>
    /// Updates the music volume, refreshes the UI text, and saves the value to PlayerPrefs.
    /// </summary>
    /// <param name="volume">The new volume value between 0 and 1.</param>
    public void ChangeVolume(float volume)
    {
        if (_musicSource == null) return;

        _musicSource.volume = volume;
        UpdateVolumeText(volume);

        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
    /// <summary>
    /// Converts a 0-1 volume value to a percentage and displays it in the UI text.
    /// </summary>
    /// <param name="volume">The volume value between 0 and 1.</param>
    public void UpdateVolumeText(float volume)
    {
        if (_volumeText == null) return;

        int volumePercent = Mathf.RoundToInt(volume * 100);
        _volumeText.text = $"{volumePercent}%";
    }

    /// <summary>
    /// Updates the SFX volume, refreshes the UI text, and saves the value to PlayerPrefs.
    /// </summary>
    /// <param name="volume">The new volume value between 0 and 1.</param>
    public void ChangeSFXVolume(float volume)
    {
        if (_sfxSource == null) return;

        _sfxSource.volume = volume;

        // Two separate AudioSources are required here: _footstepSource handles the looping footstep audio,
        // while _sfxSource handles all one-shot SFX (jumps, damage, collectibles, etc.).
        // Using a single AudioSource for both caused PlayOneShot calls to interfere with each other,
        // resulting in some audio clips being cut off mid-playback.
        if (_footstepSource != null)
            _footstepSource.volume = _footstepVolume * volume;// Footstep volume is proportional to SFX volume


        UpdateSFXVolumeText(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);

        // Cancel previous preview and restart � plays only when slider stops moving
        if (_sfxPreviewCoroutine != null)
            StopCoroutine(_sfxPreviewCoroutine);

        _sfxPreviewCoroutine = StartCoroutine(PreviewSFXVolumeDelayed());
    }

    /// <summary>
    /// Converts a 0-1 SFX volume value to a percentage and displays it in the UI text.
    /// </summary>
    /// <param name="volume">The volume value between 0 and 1.</param>
    private void UpdateSFXVolumeText(float volume)
    {
        if (_sfxVolumeText == null) return;

        int volumePercent = Mathf.RoundToInt(volume * 100);
        _sfxVolumeText.text = $"{volumePercent}%";
    }

    public void PauseMusic()
    {
        if (_musicSource.isPlaying)
        {
            _musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (!_musicSource.isPlaying)
        {
            _musicSource.UnPause();
        }
    }

    /// <summary>
    /// Waits until the SFX slider stops moving, then plays a short preview sound
    /// so the player can hear the current volume level without overlapping clips.
    /// Uses WaitForSecondsRealtime to work correctly when Time.timeScale is 0 (pause menu).
    /// </summary>
    private IEnumerator PreviewSFXVolumeDelayed()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        if (_sfxSource != null && _sfxPreviewSound != null)
            _sfxSource.PlayOneShot(_sfxPreviewSound, _sfxPreviewVolume);
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}