using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // 确保项目已安装 DOTween
using TMPro;

public class CardController : MonoBehaviour
{
    public int cardId;
    public GameObject frontObject; // 正面物体（包含图片和文字）
    public GameObject backObject;  // 背面物体
    public Image iconImage;        // 正面图标
    public TextMeshProUGUI nameLabel; // 文化符号文字标签（无障碍支持）

    private MemoryGameLogic manager;
    private bool isFlipped = false;
    private bool canClick = true;

    public void Init(int id, Sprite icon, string cName, MemoryGameLogic mgr)
    {
        cardId = id;
        iconImage.sprite = icon;
        nameLabel.text = cName;
        manager = mgr;
        
        // 初始状态：显示背面
        isFlipped = false;
        backObject.SetActive(true);
        frontObject.SetActive(false);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void OnCardClick()
    {
        if (!canClick || isFlipped) return;
        manager.OnCardSelected(this);
    }

    public void FlipOpen()
    {
        isFlipped = true;
        // 翻转动画：旋转到90度时切换显隐
        transform.DOLocalRotate(new Vector3(0, 90, 0), 0.15f).OnComplete(() => {
            backObject.SetActive(false);
            frontObject.SetActive(true);
            transform.DOLocalRotate(Vector3.zero, 0.15f);
        });
    }

    public void FlipBack()
    {
        transform.DOLocalRotate(new Vector3(0, 90, 0), 0.15f).OnComplete(() => {
            backObject.SetActive(true);
            frontObject.SetActive(false);
            isFlipped = false;
            transform.DOLocalRotate(Vector3.zero, 0.15f);
        });
    }

    public void SetClickable(bool state) => canClick = state;
}