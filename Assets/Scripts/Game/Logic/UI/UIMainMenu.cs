using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace AiLing
{
    public class UIMainMenu : UIBase
    {
        public override string PackageName { get { return "Pkg_MainMenu"; } }
        public override string componentName { get { return "UI_MainMenu"; } }

        public GComponent c_chooseArchives;

        public GList l_choose;

        public bool hasShowed = false;

        public override void Init()
        {
            BindProperty();
            RigsterEvent();
        }

        public void BindProperty()
        {
            c_chooseArchives = mainCom.GetChild("c_chooseArchives").asCom;
            l_choose = c_chooseArchives.GetChild("l_choose").asList;
        }

        public void RigsterEvent()
        {
            //l_choose.onClickItem.Set()
        }

        public override void OnShow()
        {
            l_choose.RemoveChildrenToPool();
            for (int i = 0; i < GameProcesser.Instance.archives.Count; i++)
            {
                GComponent archiveItem = l_choose.AddItemFromPool().asCom;
                GTextField t_title = archiveItem.GetChild("t_title").asTextField;
                t_title.SetVar("name", (i + 1).ToString()).FlushVars();
            }
        }

        public override void OnClose()
        {
            hasShowed = true;
        }

        public override void OnHide()
        {
            hasShowed = true;
        }
    }
}

