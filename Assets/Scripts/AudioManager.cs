using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {  get; private set; }

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;

    [Header("SFX")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioClip _clickSound;
    [SerializeField][Range(0f, 1f)] private float _clickVolume = 1f; 

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

    [Header("Enemy SFX")]
    [SerializeField] private AudioClip _enemyShootSound;
    [SerializeField][Range(0f, 1f)] private float _enemyShootingVolume = 0.6f;
    [SerializeField] private AudioClip _enemyDeathSound;
    [SerializeField][Range(0f, 1f)] private float _enemyDeathVolume = 0.6f;

    [Header("References")]
    [SerializeField] private StatePlayerMovement _statePlayerMovement;

    private StatePlayerMovement.MoveState _currentState = StatePlayerMovement.MoveState.None;

    private void Start()
    {
        // Load the previusly saved volume
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        musicSource.volume = savedVolume;
        volumeSlider.value = savedVolume;
        UpdateVolumeText(savedVolume);

        // when the slider value changes, call ChangeVolume
        volumeSlider.onValueChanged.AddListener(ChangeVolume);

        // Setup footstep audio source
        if (_footstepSource != null)
        {
            _footstepSource.volume = _footstepVolume;
            _footstepSource.clip = _footstepSound;
            _footstepSource.loop = true; 
        }
    }

    private void Update()
    {
        if(_statePlayerMovement != null)
        {
            StatePlayerMovement.MoveState newState = _statePlayerMovement.currentMoveState;

            //if the state has changed, update the current state 
            if (newState != _currentState)
            {
                HandlePlayerStateChange(newState);
                _currentState = newState;
            }
        }
    }

    //Method to handle player state changes and control footstep sounds accordingly
    private void HandlePlayerStateChange(StatePlayerMovement.MoveState newState)
    {
        // ========= FOOTSTEP CONTROLS ==========
        if (newState == StatePlayerMovement.MoveState.Run)
        {
            StartFootsteps();
        }
        else
        {
            StopFootsteps();
        }

        // ========= JUMP SFX CONTROLS ==========
        if (newState == StatePlayerMovement.MoveState.Jump)
        {
            PlayJumpSound();
        }
        else if (newState == StatePlayerMovement.MoveState.Double_Jump)
        {
            PlayDoubleJumpSound();
        }
        else if (newState == StatePlayerMovement.MoveState.Wall_Jump)
        {
            PlayWallJumpSound();
        }

    }

    // ========== FOOTSTEP PLAY ==========
    private void StartFootsteps()
    {
        if (_footstepSource != null && !_footstepSource.isPlaying)
        {
            _footstepSource.Play();
        }
    }

    private void StopFootsteps()
    {
        if (_footstepSource != null && _footstepSource.isPlaying)
        {
            _footstepSource.Stop();
        }
    }

    // ========== JUMP SFX ==========

    private void PlayJumpSound()
    {
        if (_jumpSound != null)
        {
            _sfxSource.PlayOneShot(_jumpSound, _jumpVolume);
        }
    }

    private void PlayDoubleJumpSound()
    {
        if (_doubleJumpSound != null)
        {
            _sfxSource.PlayOneShot(_doubleJumpSound, _doubleJumpVolume);
        }
    }

    private void PlayWallJumpSound()
    {
        if (_wallJumpSound != null)
        {
            _sfxSource.PlayOneShot(_wallJumpSound, _wallJumpVolume);
        }
    }

    // ========== HEALTH SFX ==========

    public void PlayTakeDamageSound()
    {
        if (_takeDamageSound != null)
        {
            _sfxSource.PlayOneShot(_takeDamageSound, _takeDamageVolume);
        }
    }

    public void PlayDeathSound()
    {
        if (_deathSound != null)
        {
            _sfxSource.PlayOneShot(_deathSound, _deathVolume);
        }
    }

    // ========== CHECKPOINT SFX ==========

    public void PlayCheckpointSound()
    {
        if (_checkpointSound != null)
        {
            _sfxSource.PlayOneShot(_checkpointSound, _checkpointVolume);
        }
    }

    // ========== COLLECTIBLE SFX ==========
    public void PlayCollectibleSound()
    {
        if (_collectibleSound != null)
        {
            _sfxSource.PlayOneShot(_collectibleSound, _collectibleVolume);
        }
    }

    // ========== ENEMY SFX ==========

    public void PlayEnemyShootSound()
    {
        if (_enemyShootSound != null)
        {
            _sfxSource.PlayOneShot(_enemyShootSound, _enemyShootingVolume);
        }
    }

    public void PlayEnemyDeathSound()
    {
        if(_enemyDeathSound != null)
        {
            Debug.Log("Playing enemy death sound");
            _sfxSource.PlayOneShot(_enemyDeathSound, _enemyDeathVolume);
        }
    }

    // ========== MUSIC CONTROLS ==========

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

    // ========== SFX CONTROLS ==========

    public void PlayClickSound()
    {
        _sfxSource.PlayOneShot(_clickSound, _clickVolume);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        if (clip != null)
        {
            _sfxSource.PlayOneShot(clip, volume);
        }
    }

}