using System.Collections.Generic;
using UnityEngine;

public class UIMgr : MonoBehaviour
{
    // 单例，方便全局访问
    public static UIMgr Instance { get; private set; }
    private bool isMenuOpen = false;

    // 存储场景中所有的 UI 窗口
    private Dictionary<string, BaseWindow> _windowDic = new Dictionary<string, BaseWindow>();

    private void Awake()
    {
        Debug.Log("UIMgr Awake 已执行");
        if (Instance == null)
        {
            Instance = this;
            // 找到场景中所有继承了 BaseWindow 的脚本并注册
            InitRegisterWindows();
            Debug.Log($"注册完成，共 {_windowDic.Count} 个窗口");
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
            win.gameObject.SetActive(true);
            win.OpenWithAnim();
            //win.Open();
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
            win.CloseWithAnim(() =>
            {
                win.Close();
            });
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }

                // 按 F1 打印所有已注册的窗口
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("===== UIMgr 已注册窗口列表 =====");
            foreach (var kvp in _windowDic)
            {
                Debug.Log($"窗口名: {kvp.Key} | 对象: {kvp.Value.gameObject.name} | 激活状态: {kvp.Value.gameObject.activeSelf}");
            }
            Debug.Log($"共 {_windowDic.Count} 个窗口");
            Debug.Log("================================");
        }
    }

    private void ToggleMenu()
    {
        if (!isMenuOpen)
        {
            UIMgr.OpenWindow<EditCanvas>();
            Time.timeScale = 0f;
            isMenuOpen = true;
        }
        else
        {
            UIMgr.CloseWindow<EditCanvas>();
            Time.timeScale = 1f;
            isMenuOpen = false;
        }
    }
}