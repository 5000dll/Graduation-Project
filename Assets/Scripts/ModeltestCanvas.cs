using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeltestCanvas : BaseWindow
{
    public Button closeBtn; 

    private void Awake()
    {
        if (closeBtn != null)
        {
            closeBtn.onClick.AddListener(() =>
            {
                UIMgr.CloseWindow<ModeltestCanvas>();
            });
        }
    }

}
