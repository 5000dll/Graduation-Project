using UnityEngine;
using DG.Tweening; // 记得安装 DOTween
using UnityEngine.UI;

public static class UIExtensions
{
    private static float duration = 0.5f; 

    /// <summary>
    /// 通用打开动画：从缩小状态弹出并淡入
    /// </summary>
    public static void OpenWithAnim(this MonoBehaviour target)
    {
        // 获取组件
        CanvasGroup group = target.GetComponent<CanvasGroup>();
        if (group == null) group = target.gameObject.AddComponent<CanvasGroup>();

        // 1. 强制重置状态（非常重要，防止动画重叠）
        DOTween.Kill(target.transform);
        DOTween.Kill(group);

        // 2. 设定夸张的初始值
        group.alpha = 0f;
        target.transform.localScale = Vector3.one * 0.4f; // 从更小开始弹出

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(group.DOFade(1f, 0.35f).SetEase(Ease.OutQuad));
        seq.Join(target.transform.DOScale(1f, 0.35f).SetEase(Ease.OutQuad));
        seq.OnComplete(() =>
        {
            group.alpha = 1f;
            group.blocksRaycasts = true;
            group.interactable = true;
        });
    }

    /// <summary>
    /// 通用关闭动画：缩小并淡出，完成后关闭窗口
    /// </summary>
    public static void CloseWithAnim(this MonoBehaviour target, System.Action onComplete)
    {
        CanvasGroup group = target.GetComponent<CanvasGroup>();
        if (group == null) group = target.gameObject.AddComponent<CanvasGroup>();

        // 动画序列
        Sequence seq = DOTween.Sequence();
        seq.Append(target.transform.DOScale(0.8f, 0.2f).SetEase(Ease.InBack)).SetUpdate(true);
        seq.Join(group.DOFade(0, 0.2f));
        
        // 动画结束后的清理工作
        seq.OnComplete(() => {
            onComplete?.Invoke();
            // 重置状态，确保下次 OpenWindow 时能正常显示
            target.transform.localScale = Vector3.one;
            group.alpha = 1;
        });
    }
}