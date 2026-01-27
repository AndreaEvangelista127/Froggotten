using UnityEngine;

public class MusicManager : MonoBehaviour
{

    [SerializeField] AudioSource _audioSource;
    public static MusicManager _instance;


    private void Awake()
    {
        //we will have one main menu manager, and if we get back to the main menu we destroy the new one
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void PlaySong(AudioClip audioClip)
    {
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }


}
