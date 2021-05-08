using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Threading;

namespace AiLing
{
    //用于管理游戏进程，包括新建档，读档，存档等
    public class GameProcesser : MonoSingleton<GameProcesser>
    {
        private string _savePath;

        public string _firstScene = "Scene_00";

        public bool enable = false;

        public List<GameModel> archives = new List<GameModel>();

        [HideInInspector]
        public GameModel currGameModel;

        private int saveGameAnimationTimer;

        private Thread saveGameThread;

        void Start()
        {
            if (enable == false)
                return;
            _savePath = Application.persistentDataPath + "/Archives";
            if (!File.Exists(_savePath))
            {
                try
                {
                    Directory.CreateDirectory(_savePath);
                }
                catch (Exception ex)
                {
                    DebugHelper.LogError("create directory fail" + ex.ToString());
                }
            }

            string[] files = Directory.GetFiles(_savePath);
            if (files.Length != 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream fs = new FileStream(_savePath + "/gamesave" + (i + 1).ToString() + ".txt", FileMode.Open);
                    GameModel model = bf.Deserialize(fs) as GameModel;
                    fs.Close();
                    archives.Add(model);
                }
            }
            UIManager.Instance.CreateAndShow<UIMainMenu>();
        }

        public void SaveGameAsyn(bool showAnim = true)
        {
            saveGameThread = new Thread(SaveGame);
            saveGameThread.Start();
            if (showAnim)
                UIManager.Instance.CreateAndShow<UISaving>();
        }

        private void SaveGame()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(_savePath + "/gamesave" + currGameModel.id.ToString() + ".txt", FileMode.OpenOrCreate);
            bf.Serialize(stream, currGameModel);
            stream.Close();
            return;
        }

        public void LoadGame(GameModel model)
        {
            currGameModel = model;
            GameMainManager.Instance.mainPlayerController.enabled = false;
            SceneManager.LoadScene(model.sceneName);
            SceneManager.sceneLoaded += (scene, loadMode) =>
            {
                GameMarkPointManager.Instance.GetRoot();
                GameMarkPointManager.Instance.GoToMark(model.mark);
                GameMainManager.Instance.mainPlayerController.enabled = true;
            };
        }

        public GameModel NewGame()
        {
            GameModel model = new GameModel();
            model.id = archives.Count + 1;
            model.sceneName = _firstScene;
            model.mark = 1;
            currGameModel = model;
            SaveGameAsyn(false);
            DebugHelper.Log("new archive");
            return model;
        }

        void Update()
        {

        }

        private void OnDestroy()
        {
            if (saveGameThread != null)
                saveGameThread.Abort();
        }
    }
}


