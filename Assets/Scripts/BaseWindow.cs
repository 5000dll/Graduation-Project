using UnityEngine;

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
}