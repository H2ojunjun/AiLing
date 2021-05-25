using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace AiLing
{
    public class GameTester : MonoSingleton<GameTester>
    {
        public string sceneName = "Scene_00";

        public int mark;

        [Button("切换场景")]
        private void ChangeScene()
        {
            SceneManager.LoadScene(sceneName);

        }

        [Button("前往标记点")]
        private void GoToMark()
        {
            GameMarkPointManager.Instance.GoToMark(mark);
        }
    }
}

