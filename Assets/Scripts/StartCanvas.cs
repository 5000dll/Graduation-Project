using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCanvas : MonoBehaviour
{
    [Header("太阳神鸟")]
    public RectTransform rotatingImage1;   // 要旋转的 UI 图片
    public RectTransform rotatingImage2;   // 要旋转的 UI 图片
    public float rotateSpeed = 60f;       // 每秒旋转角度（度/秒）

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rotatingImage1 != null && rotatingImage2 != null)
        {
            // UI 图像围绕自身圆心顺时针绕 Z 轴持续旋转
            rotatingImage1.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);
            rotatingImage2.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        }
    }

    public void CloseCanvas()
    {
        gameObject.SetActive(false);
    }
}
