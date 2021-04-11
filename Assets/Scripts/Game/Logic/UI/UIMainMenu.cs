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

        private GComponent c_chooseArchives;

        private GList l_choose;

        private GButton btn_newGame;

        private GButton btn_loadGame;

        private GButton btn_quit;

        private GButton btn_back;

        private Controller ctr_isChooseArchives;

        private bool hasShowed = false;

        public override void Init()
        {
            BindProperty();
            RigsterEvent();
        }

        public void BindProperty()
        {
            c_chooseArchives = mainCom.GetChild("c_chooseArchives").asCom;
            l_choose = c_chooseArchives.GetChild("l_choose").asList;
            btn_newGame = mainCom.GetChild("btn_newGame").asButton;
            btn_loadGame = mainCom.GetChild("btn_loadGame").asButton;
            btn_quit = mainCom.GetChild("btn_quit").asButton;
            btn_back = c_chooseArchives.GetChild("btn_back").asButton;
            ctr_isChooseArchives = mainCom.GetController("ctr_isChooseArchives");
        }

        public void RigsterEvent()
        {
            l_choose.onClickItem.Set((content) => {
                GTextField tf = (content.data as GComponent).GetChild("t_num").asTextField;
                GameProcesser.Instance.LoadGame(GameProcesser.Instance.archives[int.Parse(tf.text)-1]);
            });

            btn_newGame.onClick.Set(() => {
                GameModel model =  GameProcesser.Instance.NewGame();
                GameProcesser.Instance.LoadGame(model);
            });

            btn_loadGame.onClick.Set(() => {
                ctr_isChooseArchives.selectedPage = "yes";
            });

            btn_quit.onClick.Set(() => {
                Application.Quit();
            });

            btn_back.onClick.Set(() => {
                ctr_isChooseArchives.selectedPage = "no";
            });
        }

        public override void OnShow()
        {
            l_choose.RemoveChildrenToPool();
            for (int i = 0; i < GameProcesser.Instance.archives.Count; i++)
            {
                GComponent archiveItem = l_choose.AddItemFromPool().asCom;
                GTextField t_num = archiveItem.GetChild("t_num").asTextField;
                t_num.text = (i + 1).ToString();
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

