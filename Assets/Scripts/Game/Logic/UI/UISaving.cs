using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace AiLing
{
    public class UISaving : UIBase
    {
        public override string PackageName { get { return "Pkg_Saving"; } }

        public override string componentName { get { return "UI_Saving"; } }

        public override bool isSingle { get { return true; } }

        private Transition tr_floating;

        public override void Init()
        {
            BindProperty();
        }

        public void BindProperty()
        {
            tr_floating = mainCom.GetTransition("tr_floating");
        }

        public override void OnClose()
        {

        }

        public override void OnHide()
        {

        }

        public override void OnShow()
        {
            tr_floating.Play(3, 0, () => { UIManager.Instance.CloseUI(this); });
        }
    }
}

