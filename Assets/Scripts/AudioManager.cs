using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音源")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("音频文件")]
    [SerializeField] private AudioClip bgmClip;
    [SerializeField] private AudioClip clickClip;

    private float bgmVolume = 1f;
    private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.playOnAwake = true;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    public void PlayClick()
    {
        if (clickClip != null && sfxSource != null)
            sfxSource.PlayOneShot(clickClip, sfxVolume);
    }

    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    public void ResumeBGM()
    {
        bgmSource.UnPause();
    }

    /// <summary>设置背景音乐音量（0~1）</summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = bgmVolume;
    }

    /// <summary>设置音效音量（0~1）</summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }

    public float GetBGMVolume() => bgmVolume;
    public float GetSFXVolume() => sfxVolume;
}
