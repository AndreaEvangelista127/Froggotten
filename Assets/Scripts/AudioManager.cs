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
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioClip _clickSound;

    [Header("Player Jump")]
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private AudioClip _doubleJumpSound;
    [SerializeField] private AudioClip _wallJumpSound;

    [Header("Player Footsteps")]
    [SerializeField] private AudioSource _footstepSource; 
    [SerializeField] private AudioClip _footstepSound;
    [SerializeField] private float _footstepVolume = 0.3f;

    [Header("Player Health SFX")]
    [SerializeField] private AudioClip _takeDamageSound;
    [SerializeField] private AudioClip _deathSound;

    [Header("Checkpoint SFX")]
    [SerializeField] private AudioClip _checkpointSound;

    [Header("Collectibles SFX")]
    [SerializeField] private AudioClip _collectibleSound;

    [Header("Enemy SFX")]
    [SerializeField] private AudioClip _enemyShootSound;

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
            _sfxSource.PlayOneShot(_jumpSound);
        }
    }

    private void PlayDoubleJumpSound()
    {
        if (_doubleJumpSound != null)
        {
            _sfxSource.PlayOneShot(_doubleJumpSound);
        }
    }

    private void PlayWallJumpSound()
    {
        if (_wallJumpSound != null)
        {
            _sfxSource.PlayOneShot(_wallJumpSound);
        }
    }

    // ========== HEALTH SFX ==========

    public void PlayTakeDamageSound()
    {
        if (_takeDamageSound != null)
        {
            _sfxSource.PlayOneShot(_takeDamageSound);
        }
    }

    public void PlayDeathSound()
    {
        if (_deathSound != null)
        {
            _sfxSource.PlayOneShot(_deathSound);
        }
    }

    // ========== CHECKPOINT SFX ==========

    public void PlayCheckpointSound()
    {
        if (_checkpointSound != null)
        {
            _sfxSource.PlayOneShot(_checkpointSound);
        }
    }

    // ========== COLLECTIBLE SFX ==========
    public void PlayCollectibleSound()
    {
        if (_collectibleSound != null)
        {
            _sfxSource.PlayOneShot(_collectibleSound);
        }
    }

    // ========== ENEMY SFX ==========

    public void PlayEnemyShootSound()
    {
        if (_enemyShootSound != null)
        {
            _sfxSource.PlayOneShot(_enemyShootSound);
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
        _sfxSource.PlayOneShot(_clickSound);
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