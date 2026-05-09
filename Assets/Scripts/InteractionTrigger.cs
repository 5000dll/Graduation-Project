using UnityEngine;
using System.Collections.Generic;

public class InteractionTrigger : MonoBehaviour
{
    public enum InteractionType
    {
        Talk,
        Video,
        CardGame
    }

    [Header("交互类型")]
    public InteractionType interactionType = InteractionType.Talk;

    [Header("检测范围")]
    public float detectRadius = 5f;

    private Transform player;
    private bool isInRange = false;

    // 全局注册表：所有触发器共享
    private static List<InteractionTrigger> allTriggers = new List<InteractionTrigger>();
    private static InteractionTrigger currentActive = null;

    private void OnEnable()
    {
        allTriggers.Add(this);
    }

    private void OnDisable()
    {
        allTriggers.Remove(this);
        if (currentActive == this)
        {
            currentActive = null;
        }
    }

    private void Update()
    {
        if (player == null)
        {
            CharacterController cc = FindFirstObjectByType<CharacterController>();
            if (cc != null)
                player = cc.transform;
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectRadius)
        {
            if (!isInRange)
            {
                isInRange = true;
                UpdateActiveTrigger();
            }
        }
        else
        {
            if (isInRange)
            {
                isInRange = false;
                if (currentActive == this)
                {
                    UIMgr.CloseWindow<tipsCanvas>();
                    currentActive = null;
                    UpdateActiveTrigger();
                }
            }
        }

        // 只有当前激活的触发器响应E键
        if (currentActive == this && Input.GetKeyDown(KeyCode.E))
        {
            currentActive = null;
            isInRange = false;
            UIMgr.CloseWindow<tipsCanvas>();

            switch (interactionType)
            {
                case InteractionType.Talk:
                    UIMgr.OpenWindow<Talk>();
                    break;
                case InteractionType.Video:
                    UIMgr.OpenWindow<VideoPlayerPanel>();
                    break;
                case InteractionType.CardGame:
                    UIMgr.OpenWindow<GameCanvas>();
                    break;
            }
        }
    }

    /// <summary>从所有在范围内的触发器中选出最近的一个</summary>
    private static void UpdateActiveTrigger()
    {
        InteractionTrigger nearest = null;
        float minDist = float.MaxValue;

        foreach (var trigger in allTriggers)
        {
            if (!trigger.isInRange) continue;

            float dist = Vector3.Distance(
                trigger.transform.position, trigger.player.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = trigger;
            }
        }

        // 切换激活目标
        if (nearest != currentActive)
        {
            if (currentActive != null)
                UIMgr.CloseWindow<tipsCanvas>();

            currentActive = nearest;

            if (currentActive != null)
                UIMgr.OpenWindow<tipsCanvas>();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
