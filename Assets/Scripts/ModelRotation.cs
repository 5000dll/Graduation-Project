using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModelRotation : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Transform modelTransform; // 要旋转的模型的Transform
    public float rotationSpeed = 5f; // 旋转速度
    
    private bool isDragging = false;
    private Vector2 lastMousePosition;

    // 用于保存初始的旋转角度
    private Quaternion initialRotation;
    
    void Start()
    {
        // 如果没有指定模型，则查找子对象中的Lion1模型
        if (modelTransform == null)
        {
            modelTransform = transform.Find("Lion1");
            if (modelTransform == null)
            {
                Debug.LogError("未找到模型对象，请在Inspector中指定模型Transform");
            }
        }

        // 在初始化时记录下模型最初的旋转状态
        if (modelTransform != null)
        {
            initialRotation = modelTransform.rotation;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        lastMousePosition = eventData.position;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && modelTransform != null)
        {
            // 计算鼠标移动的差值
            Vector2 delta = eventData.position - lastMousePosition;
            
            // 绕Y轴和X轴旋转模型（Y轴旋转为主，X轴旋转为辅）
            modelTransform.Rotate(Vector3.up, -delta.x * rotationSpeed * Time.deltaTime, Space.World);
            modelTransform.Rotate(Vector3.right, delta.y * rotationSpeed * Time.deltaTime, Space.World);
            
            // 更新上一次鼠标位置
            lastMousePosition = eventData.position;
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void SmoothResetRotation()
    {
        StartCoroutine(ResetCoroutine());
        Debug.Log("模型已复位");
    }

    IEnumerator ResetCoroutine()
    {   
        float elapsed = 0f;
        float duration = 0.5f; // 复位动画持续0.5秒
        Quaternion currentRot = modelTransform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 使用球形插值平滑旋转
            modelTransform.rotation = Quaternion.Slerp(currentRot, initialRotation, elapsed / duration);
            yield return null;
        }
        modelTransform.rotation = initialRotation;
    }
}
