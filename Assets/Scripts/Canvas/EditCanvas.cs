using UnityEngine;
using UnityEngine.UI;

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

    private void OnEnable()
    {
        // 每次激活时强制回到帮助面板
        ShowPanel(Panel_help);

        btn_help.onClick.AddListener(OnHelpClick);
        btn_music.onClick.AddListener(OnMusicClick);
        btn_exit.onClick.AddListener(OnExitClick);
        btn_close.onClick.AddListener(OnCloseClick);
    }

    private void OnDisable()
    {
        btn_help.onClick.RemoveListener(OnHelpClick);
        btn_music.onClick.RemoveListener(OnMusicClick);
        btn_exit.onClick.RemoveListener(OnExitClick);
        btn_close.onClick.RemoveListener(OnCloseClick);
    }

    private void OnHelpClick()
    {
        ShowPanel(Panel_help);
    }

    private void OnMusicClick()
    {
        ShowPanel(Panel_music);
    }

    private void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnCloseClick()
    {
        UIMgr.CloseWindow<EditCanvas>();
    }

    private void ShowPanel(GameObject target)
    {
        Panel_help.SetActive(false);
        Panel_music.SetActive(false);
        target.SetActive(true);
    }
}