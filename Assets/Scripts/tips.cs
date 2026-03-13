using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tips : MonoBehaviour
{
    //public Dialogue_xqcxWindow script;
    private bool isNextValue;
    public Transform target; // 目标物体的 Transform
    public const float maxDistance = 5f; // 最大检测距离

    private bool hasOpenedTipWindow = false;
    void Update()
    {  
         // 计算当前物体与目标物体之间的距离
        float distance = Vector3.Distance(transform.position, target.position);
        isNextValue = GameData.IsNext;

        // 检查距离是否在指定范围内
        if (distance <= maxDistance)
        {
            if (!hasOpenedTipWindow)
            {
                UIMgr.OpenWindow<tipsCanvas>();
                hasOpenedTipWindow = true; // 设置标志为 true，表示已经打开过
            }
            
            if (isNextValue == true)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    UIMgr.CloseWindow<tipsCanvas>();
                    //UIMgr.OpenWindow<Dialogue_mirrorWindow>();
                }
            }
            else if (isNextValue == false)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    UIMgr.CloseWindow<tipsCanvas>();
                    //UIMgr.OpenWindow<Dialogue_xqcxWindow>();
                }
            }
        }
        else
        {
            hasOpenedTipWindow = false;
            UIMgr.CloseWindow<tipsCanvas>();
        }
    }
}