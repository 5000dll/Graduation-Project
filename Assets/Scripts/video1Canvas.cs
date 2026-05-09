using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Video1Canvas : BaseWindow
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RenderTexture renderTexture;

    public VideoPlayer Player => videoPlayer;

    private void Start()
    {
        // 配置视频播放器
        videoPlayer.isLooping = true;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
        videoPlayer.SetDirectAudioMute(0, true);
        videoPlayer.Play();
    }

    /// <summary>外部调用：从头播放并开启声音</summary>
    public void PlayWithSound()
    {
        videoPlayer.time = 0;
        videoPlayer.SetDirectAudioMute(0, false);
        videoPlayer.Play();
    }

    /// <summary>外部调用：静音并重置到开头</summary>
    public void ResetAndMute()
    {
        videoPlayer.time = 0;
        videoPlayer.SetDirectAudioMute(0, true);
        videoPlayer.Play();
    }
}

