using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartCanvas : BaseWindow
{
    [Header("太阳神鸟")]
    public RectTransform rotatingImage1;   // 要旋转的 UI 图片
    public RectTransform rotatingImage2;   // 要旋转的 UI 图片
    public float rotateSpeed = 60f;       // 每秒旋转角度（度/秒）
    public Button btn_Help;
    public Button btn_Start;

    void Update()
    {
        if (rotatingImage1 != null && rotatingImage2 != null)
        {
            // UI 图像围绕自身圆心顺时针绕 Z 轴持续旋转
            rotatingImage1.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);
            rotatingImage2.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        }
        if (btn_Help != null)
        {
            btn_Help.onClick.AddListener(() =>
            {
                UIMgr.OpenWindow<EditCanvas>();
            });
        }
        if (btn_Start != null)
        {
            btn_Start.onClick.AddListener(() => 
            {
                UIMgr.CloseWindow<StartCanvas>();
            });
        }
    }
}
