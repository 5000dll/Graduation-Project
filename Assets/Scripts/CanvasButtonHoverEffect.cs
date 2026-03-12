using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CanvasButtonHoverEffect : MonoBehaviour
{
    [Header("缩放配置")]
    [Tooltip("鼠标悬停时的缩放比例")]
    public float hoverScale = 0.9f;
    [Tooltip("动画过渡时间")]
    public float transitionTime = 0.15f;

    // 内部类：用来记录每个按钮的原始缩放状态
    private class ButtonStatus
    {
        public Vector3 originalScale;
        public float currentLerpFactor = 0f; // 0 代表原始大小，1 代表目标缩放大小
    }

    // 只有“被摸过”的按钮才会被记录到这里，节省性能
    private Dictionary<RectTransform, ButtonStatus> trackedButtons = new Dictionary<RectTransform, ButtonStatus>();
    private RectTransform currentHoveredButton;

    void Update()
    {
        // 1. 每帧检测：鼠标当前指着哪个按钮？
        DetectMouseHover();

        // 2. 动画更新：平滑缩放所有在字典里的按钮
        UpdateAllAnimations();
    }

    private void DetectMouseHover()
    {
        // 如果没有 EventSystem（比如场景里没建），直接返回防止报错
        if (EventSystem.current == null) return;

        // 使用 EventSystem 射线检测当前鼠标下的所有 UI
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        RectTransform foundButton = null;

        foreach (var result in results)
        {
            // 向上查找父级直到找到带有 Button 组件的物体
            // 这样即使你鼠标指着按钮里的文字，也能让整个按钮缩放
            Button btn = result.gameObject.GetComponentInParent<Button>();
            if (btn != null)
            {
                foundButton = btn.GetComponent<RectTransform>();
                break;
            }
        }

        // 如果鼠标指向了新按钮，且还没记录过原始比例，就初始化它
        if (foundButton != null && !trackedButtons.ContainsKey(foundButton))
        {
            trackedButtons.Add(foundButton, new ButtonStatus { originalScale = foundButton.localScale });
        }

        currentHoveredButton = foundButton;
    }

    private void UpdateAllAnimations()
    {
        // 准备一个临时列表，用于存放需要停止跟踪（缩放回1且没被悬停）的按钮
        List<RectTransform> toRemove = new List<RectTransform>();

        // 我们只遍历“活跃”中的按钮，非常省 CPU
        foreach (var pair in trackedButtons)
        {
            RectTransform rt = pair.Key;
            ButtonStatus status = pair.Value;

            // 容错处理：如果按钮被动态销毁了
            if (rt == null)
            {
                toRemove.Add(rt);
                continue;
            }

            // 确定缩放目标：如果当前鼠标指着它，就往 hoverScale 靠拢
            bool isTarget = (rt == currentHoveredButton);
            float targetValue = isTarget ? 1f : 0f;

            // 这里的 currentLerpFactor 会在 0 和 1 之间平滑滑动
            status.currentLerpFactor = Mathf.MoveTowards(status.currentLerpFactor, targetValue, Time.deltaTime / transitionTime);

            // 执行缩放
            rt.localScale = Vector3.Lerp(status.originalScale, status.originalScale * hoverScale, status.currentLerpFactor);

            // 优化：如果按钮已经完全缩放回原始大小，且鼠标没指着它，就把它从字典里踢出去
            if (!isTarget && status.currentLerpFactor <= 0f)
            {
                rt.localScale = status.originalScale;
                toRemove.Add(rt);
            }
        }

        // 执行清理
        foreach (var rt in toRemove)
        {
            trackedButtons.Remove(rt);
        }
    }
}