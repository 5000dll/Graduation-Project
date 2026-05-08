using UnityEngine;
using UnityEngine.UI;

// 确保挂载此脚本的对象一定有 CanvasGroup，方便做显隐控制
[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseWindow : MonoBehaviour
{
    protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
        // 如果想配合 CanvasGroup 做淡入，可以在这里写逻辑
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        // 每次窗口激活时绑定音效
        BindClickSounds();
    }

    private void BindClickSounds()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            // 移除旧的音效监听（防止重复绑定）
            btn.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlayClick();
            });
        }
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayClick();
    }
}