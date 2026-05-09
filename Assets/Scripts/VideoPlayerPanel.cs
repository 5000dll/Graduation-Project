using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class VideoPlayerPanel : BaseWindow
{
    [Header("控制组件")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text totalTimeText;
    [SerializeField] private Button btnPause;
    [SerializeField] private Button btnClose;
    [SerializeField] private Sprite playIcon;
    [SerializeField] private Sprite pauseIcon;

    private VideoPlayer videoPlayer;
    private bool isPaused = false;
    private bool isDragging = false;

    private void OnEnable()
    {
        Video1Canvas v1c = FindFirstObjectByType<Video1Canvas>();
        if (v1c == null)
        {
            Debug.LogError("找不到 Video1Canvas");
            return;
        }

        videoPlayer = v1c.Player;
        isPaused = false;
        isDragging = false;

        // 从头播放并开启声音
        videoPlayer.time = 0;
        videoPlayer.SetDirectAudioMute(0, false);
        videoPlayer.loopPointReached += OnVideoEnd;

        // 直接初始化UI，不依赖 prepareCompleted
        InitUI();

        AudioManager.Instance?.PauseBGM();
        UpdatePauseIcon();
        SetupEvents();
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
            videoPlayer.SetDirectAudioMute(0, true);
            videoPlayer = null;
        }

        CleanupEvents();
        AudioManager.Instance?.ResumeBGM();
    }

    private void InitUI()
    {
        if (videoPlayer == null) return;

        // 等一帧让 VideoPlayer 完成 time 设置
        float videoLength = (float)videoPlayer.length;

        if (totalTimeText != null)
            totalTimeText.text = FormatTime(videoLength);

        progressSlider.minValue = 0;
        progressSlider.maxValue = videoLength > 0 ? videoLength : 100f;
        progressSlider.value = 0;

        Debug.Log($"视频时长: {videoLength}秒");
    }

    private void SetupEvents()
    {
        btnPause.onClick.RemoveAllListeners();
        btnClose.onClick.RemoveAllListeners();
        btnPause.onClick.AddListener(OnPauseClick);
        btnClose.onClick.AddListener(OnCloseClick);

        if (progressSlider == null) return;

        EventTrigger trigger = progressSlider.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = progressSlider.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();

        EventTrigger.Entry down = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener((data) => { isDragging = true; });
        trigger.triggers.Add(down);

        EventTrigger.Entry up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener((data) =>
        {
            isDragging = false;
            if (videoPlayer != null)
                videoPlayer.time = progressSlider.value;
        });
        trigger.triggers.Add(up);

        progressSlider.onValueChanged.RemoveListener(OnSliderChanged);
        progressSlider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void CleanupEvents()
    {
        btnPause.onClick.RemoveListener(OnPauseClick);
        btnClose.onClick.RemoveListener(OnCloseClick);

        if (progressSlider != null)
        {
            progressSlider.onValueChanged.RemoveListener(OnSliderChanged);
            EventTrigger trigger = progressSlider.GetComponent<EventTrigger>();
            if (trigger != null) trigger.triggers.Clear();
        }
    }

    private void Update()
    {
        if (videoPlayer == null) return;

        if (!isDragging)
        {
            progressSlider.value = (float)videoPlayer.time;
        }

        if (currentTimeText != null)
            currentTimeText.text = FormatTime(videoPlayer.time);
    }

    private void OnPauseClick()
    {
        if (videoPlayer == null) return;

        if (isPaused)
        {
            videoPlayer.Play();
            isPaused = false;
        }
        else
        {
            videoPlayer.Pause();
            isPaused = true;
        }
        UpdatePauseIcon();
    }

    private void OnCloseClick()
    {
        UIMgr.CloseWindow<VideoPlayerPanel>();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        UIMgr.CloseWindow<VideoPlayerPanel>();
    }

    private void OnSliderChanged(float value)
    {
        if (isDragging && currentTimeText != null)
            currentTimeText.text = FormatTime(value);
    }

    private void UpdatePauseIcon()
    {
        Image img = btnPause.GetComponent<Image>();
        if (img != null && playIcon != null && pauseIcon != null)
            img.sprite = isPaused ? playIcon : pauseIcon;
    }

    private string FormatTime(double seconds)
    {
        int mins = Mathf.FloorToInt((float)seconds / 60f);
        int secs = Mathf.FloorToInt((float)seconds % 60f);
        return string.Format("{0:00}:{1:00}", mins, secs);
    }
}
