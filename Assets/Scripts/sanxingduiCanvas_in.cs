using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sanxingduiCanvas_in : BaseWindow
{
    public Button closeBtn; // 建议写全称防止混淆
    void Awake()
    {
        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(() =>
            {
                UIMgr.CloseWindow<sanxingduiCanvas_in>();
            });
        }
    }
}
