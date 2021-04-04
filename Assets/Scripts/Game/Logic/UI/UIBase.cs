using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase
{
    public UIPanel panel;

    public GComponent mainCom;

    //protected virtual void Awake()
    //{
    //    panel = GetComponent<UIPanel>();
    //    mainCom = panel.ui;
    //}

    protected abstract void Init();

    protected abstract void OnShow();

    protected abstract void OnClose();

    protected abstract void OnHide();
}
