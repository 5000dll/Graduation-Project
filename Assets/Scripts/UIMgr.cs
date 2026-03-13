using System.Collections.Generic;
using UnityEngine;

public class UIMgr : MonoBehaviour
{
    // 单例，方便全局访问
    public static UIMgr Instance { get; private set; }

    // 存储场景中所有的 UI 窗口
    private Dictionary<string, BaseWindow> _windowDic = new Dictionary<string, BaseWindow>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 找到场景中所有继承了 BaseWindow 的脚本并注册
            InitRegisterWindows();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitRegisterWindows()
    {
        // 这里的 true 表示即使是隐藏状态的物体也能找到
        BaseWindow[] windows = GetComponentsInChildren<BaseWindow>(true);
        foreach (var win in windows)
        {
            string winName = win.GetType().Name;
            if (!_windowDic.ContainsKey(winName))
            {
                _windowDic.Add(winName, win);
            }
        }
    }

    // --- 核心方法：打开窗口 ---
    public static void OpenWindow<T>() where T : BaseWindow
    {
        string name = typeof(T).Name;
        if (Instance._windowDic.TryGetValue(name, out BaseWindow win))
        {
            win.Open();
        }
        else
        {
            Debug.LogWarning($"UIMgr: 未找到名为 {name} 的窗口！");
        }
    }

    // --- 核心方法：关闭窗口 ---
    public static void CloseWindow<T>() where T : BaseWindow
    {
        string name = typeof(T).Name;
        if (Instance._windowDic.TryGetValue(name, out BaseWindow win))
        {
            win.Close();
        }
    }
}