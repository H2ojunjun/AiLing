using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using System;

namespace AiLing
{
    [GameEventInfo("游戏结束")]
    public class GameOverEvent : GameEvent
    {
        public override void Excute(List<GameObject> unartPara)
        {
            base.Excute(unartPara);
            GameProcesser.Instance.ChangeScene("GameOver", GameOverSceneLoadedCallBack);
        }

        private void GameOverSceneLoadedCallBack(Scene arg0, LoadSceneMode arg1)
        {
            GameMainManager.Instance.mainCamera.enabled = false;
            GameMainManager.Instance.mainPlayerInput.enabled = false;
            UIManager.Instance.CreateAndShow<UIGameOver>();
            SceneManager.sceneLoaded -= GameOverSceneLoadedCallBack;
        }
    }
}

