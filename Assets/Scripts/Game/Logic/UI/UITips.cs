using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace AiLing{
    public class UITips : UIBase
    {
        public override string PackageName => "Pkg_Tips";

        public override string componentName => "UI_Tips";

        private GTextField t_tips;

        public string content { private get; set; }

        public Color color { get; set; }

        public override bool isSingle { get { return true; } }

        public override void Init()
        {
            BindProperty();
        }

        public void BindProperty()
        {
            t_tips = mainCom.GetChild("t_tips").asTextField;
        }

        public override void OnClose()
        {
            
        }

        public override void OnHide()
        {
            
        }

        public override void OnShow()
        {
            t_tips.text = content;
            t_tips.color = color;
        }
    }
}

