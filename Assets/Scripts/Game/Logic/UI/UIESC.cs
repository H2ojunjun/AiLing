using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace AiLing
{
    public class UIESC :UIBase
    {
        public override string PackageName { get { return "Pkg_ESC"; } }
        public override string componentName { get { return "UI_ESC"; } }

        private GButton btn_Continue;

        private GButton btn_loadGame;

        private GButton btn_quit;

        private GButton btn_back;

        public override void Init()
        {
            BindProperty();
            RigsterEvent();
        }

        public void BindProperty()
        {
            btn_Continue = mainCom.GetChild("btn_Continue").asButton;
            btn_loadGame = mainCom.GetChild("btn_loadGame").asButton;
            btn_quit = mainCom.GetChild("btn_quit").asButton;
            btn_back = mainCom.GetChild("btn_back").asButton;
        }

        public void RigsterEvent()
        {
            btn_Continue.onClick.Set(() => {
                UIManager.Instance.HideUI(this);
            });

            btn_loadGame.onClick.Set(() => {
                GameMarkPointManager.Instance.GetBackToClosestMark();
                UIManager.Instance.HideUI(this);
            });

            btn_quit.onClick.Set(() => {
                Application.Quit();
            });

            btn_back.onClick.Set(() => {
                UIManager.Instance.HideUI(this);
            });
        }

        public override void OnShow()
        {

        }

        public override void OnClose()
        {
        }

        public override void OnHide()
        {

        }
    }
}

