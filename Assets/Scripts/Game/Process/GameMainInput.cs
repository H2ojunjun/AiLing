using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    //负责接受非特殊的游戏输入
    public class GameMainInput : MonoSingleton<GameMainInput>
    {
        private void Update()
        {
            if(InputManager.Instance.GetKeyDown(KeyCode.Escape)|| InputManager.Instance.GetKeyDown(KeyCode.JoystickButton7))
            {
                UIBase ui;
                if (UIManager.Instance.uiDic.TryGetValue("UI_MainMenu",out ui))
                {
                    if (ui.isShowing == true)
                        return;
                }
                UIManager.Instance.CreateAndShow<UIESC>();
            }
        }
    }
}

