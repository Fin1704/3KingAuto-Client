using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    public static MusicManager Instance
    {
        get { return instance; }
    }

    private AudioSource musicSource;
    private float defaultVolume = 1f;
    private bool isFading = false;

    [SerializeField] private AudioClip[] musicTracks;
    private int currentTrackIndex = 0;

    void Awake()
    {
        // Singleton pattern implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else
        {
            Destroy(gameObject);
        }
    }
public void Start()
{
    PlayMusic(0);
}
    private void InitializeAudioSource()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = defaultVolume;
    }

    public void PlayMusic(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= musicTracks.Length)
            return;

        if (musicSource.isPlaying && currentTrackIndex == trackIndex)
            return;

        currentTrackIndex = trackIndex;
        musicSource.clip = musicTracks[trackIndex];
        musicSource.Play();
    }

    public void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % musicTracks.Length;
        PlayMusic(currentTrackIndex);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void SetVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        musicSource.volume = volume;
    }

    public IEnumerator FadeMusic(float targetVolume, float duration)
    {
        if (isFading) yield break;

        isFading = true;
        float startVolume = musicSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / duration);
            musicSource.volume = newVolume;
            yield return null;
        }

        musicSource.volume = targetVolume;
        isFading = false;
    }

    public void FadeIn(float duration)
    {
        StartCoroutine(FadeMusic(defaultVolume, duration));
    }

    public void FadeOut(float duration)
    {
        StartCoroutine(FadeMusic(0f, duration));
    }

    public bool IsMusicPlaying()
    {
        return musicSource.isPlaying;
    }
}