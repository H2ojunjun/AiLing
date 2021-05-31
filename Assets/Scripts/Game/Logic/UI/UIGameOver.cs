using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace AiLing
{
    public class UIGameOver : UIBase
    {
        public override string PackageName { get { return "Pkg_GameOver"; } }

        public override string componentName { get { return "UI_GameOver"; } }

        public override bool isSingle { get { return true; } }

        public Transition tr_showName;

        public override void Init()
        {
            tr_showName = mainCom.GetTransition("tr_showName");
        }

        public override void OnClose()
        {
            isShowing = false;
        }

        public override void OnHide()
        {
            isShowing = false;
        }

        public override void OnShow()
        {
            tr_showName.Play();
            isShowing = true;
        }
    }
}

