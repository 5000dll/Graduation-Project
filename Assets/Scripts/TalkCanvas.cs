using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UI.Toggle;

public class Talk : BaseWindow
{
    [Header("角色立绘")]
    public Image left;
    public Image right;

    [Header("文本组件")]
    public TMP_Text nametext;
    public TMP_Text dialogtext;

    [Header("按钮")]
    public Button button;
    public Button cancel;
    public Button replay_btn;
    public Button submit_btn;

    [Header("选项相关")]
    public GameObject Choice;
    public GameObject Judge;
    public Transform buttonGroup;

    [Header("对话数据")]
    public TextAsset dialog;

    private int dialogIndex;
    private string[] dialogRows;

    private HashSet<int> correctAnswers = new HashSet<int>();
    private int choicerightId;
    private int choiceerrorId;
    private int wrongNum = 0;

    // ===================== 生命周期 =====================

        private void OnEnable()
    {
        InitDialog();
    }

    public override void Open()
    {
        base.Open();
        InitDialog();
    }

    private void Start()
    {
        if (dialogRows == null || dialogRows.Length == 0)
        {
            InitDialog();
        }
    }

    private void InitDialog()
    {
        ReadText(dialog);
        wrongNum = 0;
        BindButtons();

        button.gameObject.SetActive(false);
        submit_btn.gameObject.SetActive(false);
        replay_btn.gameObject.SetActive(false);
        cancel.gameObject.SetActive(false);

        ShowDialogRow();
    }

    public override void Close()
    {
        base.Close();
        ClearChoices();
    }

    // ===================== 按钮绑定 =====================

    private bool isBound = false;

    private void BindButtons()
    {
        if (isBound) return;
        isBound = true;

        button.onClick.AddListener(OnClickNext);
        cancel.onClick.AddListener(OnClickLeave);
        replay_btn.onClick.AddListener(OnReplay);
        submit_btn.onClick.AddListener(OnClickSubmit);
    }


    // ===================== CSV 解析 =====================

    public void ReadText(TextAsset textAsset)
    {
        string cleanText = textAsset.text.TrimStart('\uFEFF', '\u200B');
        string[] rawRows = cleanText.Split('\n');

        List<string> validRows = new List<string>();
        for (int i = 1; i < rawRows.Length; i++)
        {
            string row = rawRows[i].TrimEnd('\r', '\n', ' ');
            if (!string.IsNullOrEmpty(row))
            {
                validRows.Add(row);
            }
        }
        dialogRows = validRows.ToArray();
    }

    public void ShowDialogRow()
    {
        ClearChoices();

        for (int i = 0; i < dialogRows.Length; i++)
        {
            string line = dialogRows[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] cells = line.Split(',');
            if (cells.Length < 2) continue;

            string type = cells[0].Trim();
            if (!int.TryParse(cells[1].Trim(), out int id)) continue;
            if (id != dialogIndex) continue;

            switch (type)
            {
                case "+":
                    HandleDialog(cells);
                    return;
                case "*":
                    HandleSingleChoice(i);
                    return;
                case "/":
                    HandleMultiChoice(i);
                    return;
                case "END":
                    HandleEnd();
                    return;
                case "RE":
                    HandleReplay();
                    return;
            }
        }
    }

    // ===================== 节点处理 =====================

    private void HandleDialog(string[] cells)
    {
        string speakerName = cells[2];
        string position = cells[3];
        string content = cells[4].Replace("|", "\n");
        int nextId = int.Parse(cells[5]);

        nametext.text = speakerName;
        dialogtext.text = content;

        left.gameObject.SetActive(position == "左");
        right.gameObject.SetActive(position == "右");

        dialogIndex = nextId;

        SetButtonsActive(nextBtn: true, submitBtn: false,
                         cancelBtn: false, replayBtn: false);
    }

    private int lastChoicePoint = 0;

    private void HandleSingleChoice(int startIndex)
    {
        lastChoicePoint = dialogIndex; // 记录进入选择前的节点 ID

        SetButtonsActive(nextBtn: false, submitBtn: false,
                        cancelBtn: false, replayBtn: false);

        for (int i = startIndex; i < dialogRows.Length; i++)
        {
            string[] cells = dialogRows[i].Split(',');
            if (cells[0].Trim() != "*") break;

            int choiceId = int.Parse(cells[5]);
            GameObject btn = Instantiate(Choice, buttonGroup);
            btn.GetComponentInChildren<TMP_Text>().text = cells[4];
            btn.GetComponent<Button>().onClick.AddListener(
                () => OnChoiceClick(choiceId)
            );
        }
    }

    private void HandleReplay()
    {
        dialogIndex = lastChoicePoint; // 回到选择节点，而非 RE 节点
        UIMgr.CloseWindow<Talk>();
    }


    private void HandleMultiChoice(int startIndex)
    {
        SetButtonsActive(nextBtn: false, submitBtn: true,
                         cancelBtn: false, replayBtn: false);

        correctAnswers.Clear();

        for (int i = startIndex; i < dialogRows.Length; i++)
        {
            string[] cells = dialogRows[i].Split(',');
            if (cells[0].Trim() != "/") break;

            GameObject toggle = Instantiate(Judge, buttonGroup);
            toggle.GetComponentInChildren<Text>().text = cells[4];

            if (cells.Length > 6 && cells[6].Trim() == "1")
            {
                correctAnswers.Add(int.Parse(cells[7]));
                choicerightId = int.Parse(cells[5]);
            }
            else
            {
                choiceerrorId = int.Parse(cells[5]);
            }
        }
    }

    private void HandleEnd()
    {
        /*
        nametext.text = "";
        dialogtext.text = "对话结束";
        left.gameObject.SetActive(false);
        right.gameObject.SetActive(false);

        SetButtonsActive(nextBtn: false, submitBtn: false,
                         cancelBtn: true, replayBtn: false);
        */
        UIMgr.CloseWindow<Talk>();
    }

    // ===================== 按钮回调 =====================

    public void OnClickNext()
    {
        ShowDialogRow();
    }

    public void OnClickSubmit()
    {
        HashSet<int> userAnswers = new HashSet<int>();

        foreach (Transform child in buttonGroup)
        {
            Toggle toggle = child.GetComponent<Toggle>();
            if (toggle != null && toggle.isOn)
            {
                userAnswers.Add(child.GetSiblingIndex());
            }
        }

        if (correctAnswers.SetEquals(userAnswers))
        {
            dialogIndex = choicerightId;
            correctAnswers.Clear();
            ShowDialogRow();
        }
        else
        {
            wrongNum++;
            correctAnswers.Clear();

            if (wrongNum >= 2)
            {
                nametext.text = "";
                dialogtext.text = "你再逛逛吧";
                ClearChoices();
                SetButtonsActive(nextBtn: false, submitBtn: false,
                                 cancelBtn: true, replayBtn: false);
                return;
            }

            dialogIndex = choiceerrorId;
            ShowDialogRow();
        }
    }

    public void OnChoiceClick(int id)
    {
        dialogIndex = id;
        ShowDialogRow();
    }

    private void OnClickLeave()
    {
        UIMgr.CloseWindow<Talk>();
    }

    private void OnReplay()
    {
        dialogIndex = 0;
        wrongNum = 0;
        ShowDialogRow();
    }

    // ===================== 工具方法 =====================

    private void SetButtonsActive(bool nextBtn, bool submitBtn,
                                   bool cancelBtn, bool replayBtn)
    {
        button.gameObject.SetActive(nextBtn);
        submit_btn.gameObject.SetActive(submitBtn);
        cancel.gameObject.SetActive(cancelBtn);
        replay_btn.gameObject.SetActive(replayBtn);
    }

    private void ClearChoices()
    {
        foreach (Transform child in buttonGroup)
        {
            Destroy(child.gameObject);
        }
    }
}
