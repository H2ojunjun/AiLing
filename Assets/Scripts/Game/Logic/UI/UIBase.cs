using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public abstract class UIBase
    {
        public abstract string PackageName { get; }

        public abstract string componentName { get; }

        public abstract bool isSingle { get; }

        public GameObject owner;

        public UIPanel panel;

        public GComponent mainCom;

        public bool isShowing;

        public abstract void Init();

        public abstract void OnShow();

        public abstract void OnClose();

        public abstract void OnHide();
    }
}

