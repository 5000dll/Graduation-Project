using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sanxingduiCanvas : MonoBehaviour
{
    public Transform target; // 目标物体的 Transform
    public float maxDistance = 3f; // 最大检测距离
    public float bufferDistance = 0.5f; // 缓冲距离，避免边界闪烁

    private bool hasOpenedTipWindow = false;
    private bool isPlayerInRange = false;
    
    void Update()
    {
         // 计算当前物体与目标物体之间的距离
        float distance = Vector3.Distance(transform.position, target.position);

        // 检查距离是否在指定范围内
        if (distance <= maxDistance - bufferDistance)
        {
            // 进入范围
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                if (!hasOpenedTipWindow)
                {
                    UIMgr.OpenWindow<tipsCanvas>();
                    hasOpenedTipWindow = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                UIMgr.CloseWindow<tipsCanvas>();
                UIMgr.OpenWindow<sanxingduiCanvas_in>();
            }
        }
        else if (distance > maxDistance)
        {
            // 离开范围
            if (isPlayerInRange)
            {
                isPlayerInRange = false;
                hasOpenedTipWindow = false;
                UIMgr.CloseWindow<tipsCanvas>();
            }
        }
    }
}
