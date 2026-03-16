using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Talk : BaseWindow
{
        public Image left;
        public Image left1;
        public Image right;
        public TMP_Text nametext;
        public TMP_Text dialogtext;
        public Button button;
        public Button cancel;

        public GameObject Choice;
        public Transform buttonGroup;

        public TextAsset dialog;
        public int dialogIndex;
        string[] dialogRows;

        public void OnEnter()
        {
            //base.OnEnter();
            OnBtnBind();
        }

        void Start()
        {
            ReadText(dialog);
            ShowDialogRow();
        }

        public void UpdateText(string _name, string _text)
        {
            nametext.text = _name;
            dialogtext.text = _text;
        }

        public void UpdateImage(string _name, string _positon)
        {
            if (_positon == "左")
            {
                left.gameObject.SetActive(true);
                left1.gameObject.SetActive(false);
                right.gameObject.SetActive(false);
            }
            else if (_positon == "右")
            {
                left.gameObject.SetActive(false);
                left1.gameObject.SetActive(false);
                right.gameObject.SetActive(true);
            }
            else if( _positon == "左1")
            {
                left.gameObject.SetActive(false);
                left1.gameObject.SetActive(true);
                right.gameObject.SetActive(false);
            }
        }

        public void ReadText(TextAsset _textAsset)
        {
            dialogRows = _textAsset.text.Split('\n');
        }

        public void ShowDialogRow()
        {
            for (int i = 0; i < dialogRows.Length; i++)
            {
                string[] cells = dialogRows[i].Split(',');
                if (cells[0] == "+" && int.Parse(cells[1]) == dialogIndex)
                {
                    Debug.Log("111");
                    UpdateText(cells[2], cells[4]);
                    UpdateImage(cells[2], cells[3]);
                    dialogIndex = int.Parse(cells[5]);
                    button.gameObject.SetActive(true);
                    break;
                }

                else if (cells[0] == "*" && int.Parse(cells[1]) == dialogIndex)
                {
                    Debug.Log("222");
                    button.gameObject.SetActive(false);
                    ChoiceB(i);
                }

                else if (cells[0] == "END" && int.Parse(cells[1]) == dialogIndex)
                {
                    button.gameObject.SetActive(false);
                    cancel.gameObject.SetActive(true);
                    Debug.Log("close");
                }
            }
        }
        public void OnClickNext()
        {
            ShowDialogRow();
        }

        public void ChoiceB(int _index)
        {
            string[] cells = dialogRows[_index].Split(",");

            if (cells[0] == "*")
            {
                GameObject button = Instantiate(Choice, buttonGroup);

                button.GetComponentInChildren<TMP_Text>().text = cells[4];

                button.GetComponent<Button>().onClick.AddListener
                    (
                        delegate
                        {
                            OnChoiceClick(int.Parse(cells[5]));
                        }
                    );

                ChoiceB(_index + 1);
            }
        }
        public void OnChoiceClick(int _id)
        {
            dialogIndex = _id;
            ShowDialogRow();
            for (int i = 0; i < buttonGroup.childCount; i++)
            {
                Destroy(buttonGroup.GetChild(i).gameObject);
            }
        }

        public void CloseBtnHandler()
        {
            //GamePlayVideoWindow.PlayVideo("结尾");
            //UIMgr.CloseWindow<TalkWindow>();
            //UIMgr.OpenWindow<transitionWindow>();
        }

        private void OnBtnBind()
        {
            //AddBtnListener(cancel, CloseBtnHandler);;
        }
}
