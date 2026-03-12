using UnityEngine;

// 挂在任意 Canvas（或其根对象）上：打开 UI 时禁用相机移动/视角，关闭 UI 时恢复。
public class CanvasInputLock : MonoBehaviour
{
    static int s_lockCount;
    static Move s_move;

    public static bool IsUiLockActive => s_lockCount > 0;

    void Awake()
    {
        EnsureMoveCached();
    }

    void OnEnable()
    {
        EnsureMoveCached();
        s_lockCount++;
        if (s_move != null)
            s_move.SetControlEnabled(false);
    }

    void OnDisable()
    {
        s_lockCount = Mathf.Max(0, s_lockCount - 1);
        EnsureMoveCached();
        if (s_lockCount == 0 && s_move != null)
            s_move.SetControlEnabled(true);
    }

    void OnDestroy()
    {
        // 防止对象被销毁但没走 OnDisable 导致计数卡住
        if (isActiveAndEnabled)
        {
            s_lockCount = Mathf.Max(0, s_lockCount - 1);
            if (s_lockCount == 0 && s_move != null)
                s_move.SetControlEnabled(true);
        }
    }

    static void EnsureMoveCached()
    {
        if (s_move != null)
            return;

        var cam = Camera.main;
        if (cam != null && cam.TryGetComponent(out Move moveOnMainCam))
        {
            s_move = moveOnMainCam;
            return;
        }

        // 兜底：即使 MainCamera 没有正确设置 Tag，也能找到相机控制脚本
        s_move = Object.FindFirstObjectByType<Move>();
    }
}

