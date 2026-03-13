using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    private static bool _isNext = false;
    private static bool _isLock = false;
    public static bool IsNext
    {
        get { return _isNext; }
        set
        {
            if (_isNext != value)
            {
                _isNext = value;
                OnIsNextChanged(); // 触发事件通知
            }
        }
    }

    public static bool IsLock
    {
        get { return _isLock; }
        set
        {
            if(_isLock != value)
            {
                _isLock = value;
                OnOpenLock();
            }
        }
    }

    // 数据变化通知事件
    public static event Action<bool> IsNextChanged;
    public static event Action<bool> IsLocked;

    private static void OnIsNextChanged()
    {
        IsNextChanged?.Invoke(_isNext);
    }

    private static void OnOpenLock()
    {
        IsLocked?.Invoke(_isLock);
    }
}
