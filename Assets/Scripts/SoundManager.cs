using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("오디오 소스 (재생기)")]
    public AudioSource bgmSource; // 배경음악용
    public AudioSource sfxSource; // 효과음용

    [Header("배경음악(BGM) 클립")]
    public AudioClip MainMenuBGM;
    public AudioClip DayEventBGM;
    public AudioClip NightEventBGM;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayMainMenuBGM()
    {
        PlayBGM(MainMenuBGM);
    }

    public void PlayDayBGM()
    {
        PlayBGM(DayEventBGM);
    }

    public void PlayNightBGM()
    {
        PlayBGM(NightEventBGM);
    }

    public void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayBGM(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && bgmSource != null)
        {
            if (bgmSource.clip == clip && bgmSource.isPlaying) return;

            bgmSource.clip = clip;
            bgmSource.volume = volume;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }
}
