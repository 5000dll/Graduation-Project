using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro; // 使用TextMeshPro显示文字
using UnityEngine.UI;

[System.Serializable]
public class CardData {
    public int id;
    public string name;
    public Sprite icon;
}

public class MemoryGameLogic : MonoBehaviour
{
    [Header("配置")]
    public List<CardData> cardDataLibrary; 
    public GameObject cardPrefab;
    public Transform gridParent; 
    
    [Header("UI引用-状态显示")]
    public TextMeshProUGUI attemptsText;  // 显示：翻牌次数: 0
    public TextMeshProUGUI progressText;  // 显示：进度: 0 / 12

    [Header("UI引用-面板")]
    public GameObject gamePanel; // 游戏主界面
    public GameObject winPanel;  // 通关画面

    private List<CardController> flippedCards = new List<CardController>();
    private int matchedCount = 0;
    private int currentAttempts = 0; // 翻牌次数统计
    private bool isProcessing = false;

    void Start()
    {
        if (gameObject.activeSelf) 
        {
            StartGame();
        }
    }

    // 开始/重置游戏
    public void StartGame()
    {
        StopAllCoroutines();
        ResetGameData();
        gamePanel.SetActive(true);
        winPanel.SetActive(false);
        GenerateCards();
        UpdateUI();
    }

    private void GenerateCards()
    {
        // 清理旧卡片
        foreach (Transform child in gridParent) Destroy(child.gameObject);

        List<int> ids = new List<int>();
        for (int i = 0; i < 12; i++) { ids.Add(i); ids.Add(i); }

        for (int i = 0; i < ids.Count; i++)
        {
            int rnd = Random.Range(i, ids.Count);
            int temp = ids[i];
            ids[i] = ids[rnd];
            ids[rnd] = temp;
        }

        foreach (int id in ids)
        {
            GameObject go = Instantiate(cardPrefab, gridParent);
            CardController card = go.GetComponent<CardController>();
            card.Init(id, cardDataLibrary[id].icon, cardDataLibrary[id].name, this);
        }
    }

    public void OnCardSelected(CardController card)
    {
        if (isProcessing) return;

        card.FlipOpen();
        flippedCards.Add(card);

        if (flippedCards.Count == 2)
        {
            currentAttempts++; // 每次翻开两张，次数+1
            StartCoroutine(CheckMatchRoutine());
        }
    }

    private IEnumerator CheckMatchRoutine()
    {
        isProcessing = true;
        UpdateUI(); // 更新次数显示

        CardController c1 = flippedCards[0];
        CardController c2 = flippedCards[1];

        if (c1.cardId == c2.cardId)
        {
            yield return new WaitForSeconds(0.4f);
            matchedCount++;
            UpdateUI(); // 更新进度显示
            if (matchedCount >= 12) winPanel.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(0.8f);
            c1.FlipBack();
            c2.FlipBack();
        }

        flippedCards.Clear();
        yield return new WaitForSeconds(0.3f); 
        isProcessing = false;
    }

    private void UpdateUI()
    {
        if (attemptsText != null) attemptsText.text = "翻牌次数: " + currentAttempts;
        if (progressText != null) progressText.text = "已配对: " + matchedCount + " / 12";
    }

    private void ResetGameData()
    {
        matchedCount = 0;
        currentAttempts = 0;
        flippedCards.Clear();
        isProcessing = false;
    }

    // --- 按钮功能 ---

    // 退出游戏（关闭界面）
    public void ExitGame()
    {
        //gamePanel.SetActive(false);
        //winPanel.SetActive(false);
        // 如果是在NPC交互中，这里可以恢复玩家移动控制
        UIMgr.CloseWindow<GameCanvas>();
    }

    // 重玩（通关后点击）
    public void PlayAgain()
    {
        StartGame();
    }
}