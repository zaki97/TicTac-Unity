using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource soundEffectsSource;

    public AudioClip[] backgroundMusic;
    public AudioClip[] soundEffects;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        //if (backgroundMusic.Length > 0)
        //{
        //    PlayMusic(0);
        //}
    }

    public void PlayMusic(int index)
    {
        if (index >= 0 && index < backgroundMusic.Length)
        {
            musicSource.clip = backgroundMusic[index];
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Invalid music index");
        }
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PlaySoundEffect(int index)
    {
        if (index >= 0 && index < soundEffects.Length)
        {
            soundEffectsSource.PlayOneShot(soundEffects[index]);
        }
        else
        {
            Debug.LogWarning("Invalid sound effect index");
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSoundEffectsVolume(float volume)
    {
        soundEffectsSource.volume = Mathf.Clamp01(volume);
    }

}
