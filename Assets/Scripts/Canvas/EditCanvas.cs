using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class EditCanvas : BaseWindow
{
    [Header("按钮")]
    [SerializeField] private Button btn_help;
    [SerializeField] private Button btn_music;
    [SerializeField] private Button btn_exit;
    [SerializeField] private Button btn_close;

    [Header("面板")]
    [SerializeField] private GameObject Panel_help;
    [SerializeField] private GameObject Panel_music;

    [Header("音量控制")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text bgmValueText;
    [SerializeField] private TMP_Text sfxValueText;

    protected override void OnEnable()
    {
        base.OnEnable();
        ShowPanel(Panel_help);
        InitVolumeSliders();

        // 锁定视角
        Move move = FindFirstObjectByType<Move>();
        if (move != null) move.SetControlEnabled(false);
    }

    private void OnDisable()
    {
        // 解锁视角
        Move move = FindFirstObjectByType<Move>();
        if (move != null) move.SetControlEnabled(true);

        Time.timeScale = 1f;
    }

    private void Start()
    {
        btn_help.onClick.AddListener(() => ShowPanel(Panel_help));
        btn_music.onClick.AddListener(() => ShowPanel(Panel_music));
        btn_exit.onClick.AddListener(OnExitClick);
        btn_close.onClick.AddListener(() => UIMgr.CloseWindow<EditCanvas>());
    }

    private void InitVolumeSliders()
    {
        if (AudioManager.Instance == null) return;

        bgmSlider.value = AudioManager.Instance.GetBGMVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();
        UpdateVolumeText();

        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void OnBGMVolumeChanged(float value)
    {
        AudioManager.Instance.SetBGMVolume(value);
        UpdateVolumeText();
    }

    private void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        UpdateVolumeText();
    }

    private void UpdateVolumeText()
    {
        if (bgmValueText != null)
            bgmValueText.text = Mathf.RoundToInt(bgmSlider.value * 100) + "%";
        if (sfxValueText != null)
            sfxValueText.text = Mathf.RoundToInt(sfxSlider.value * 100) + "%";
    }

    private void ShowPanel(GameObject target)
    {
        Panel_help.SetActive(false);
        Panel_music.SetActive(false);
        target.SetActive(true);
    }

    private void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
