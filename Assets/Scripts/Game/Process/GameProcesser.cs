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
        private GameModel _currGameModel;

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
            FileStream stream = new FileStream(_savePath + "/gamesave" + _currGameModel.id.ToString() + ".txt", FileMode.OpenOrCreate);
            bf.Serialize(stream, _currGameModel);
            stream.Close();
            return;
        }

        public void LoadGame(GameModel model,Action finishCallBack = null)
        {
            _currGameModel = model;
            GameMainManager.Instance.mainPlayerInput.enabled = false;
            SceneManager.LoadScene(model.sceneName);
            SceneManager.sceneLoaded += (scene, loadMode) =>
            {
                GameMarkPointManager.Instance.GetRoot();
                GameMarkPointManager.Instance.GoToMark(model.mark);
                GameMainManager.Instance.mainPlayerInput.enabled = true;
                finishCallBack?.Invoke();
            };
        }

        public GameModel NewGame()
        {
            GameModel model = new GameModel();
            model.id = archives.Count + 1;
            model.sceneName = _firstScene;
            model.mark = 1;
            _currGameModel = model;
            SaveGameAsyn(false);
            DebugHelper.Log("new archive");
            return model;
        }

        public void ChangeCurrMark(int mark)
        {
            _currGameModel.mark = mark;
        }

        public int GetCurrMark()
        {
            return _currGameModel.mark;
        }

        public void ChangeItemInStatusDic(string key,Dictionary<string,int> status)
        {
            Dictionary<string, int> tem;
            if(_currGameModel.statusDic.TryGetValue(key,out tem))
            {
                tem = status;
                return;
            }
            _currGameModel.statusDic.Add(key,status);
        }

        public Dictionary<string, int> GetItemInStatusDic(string key)
        {
            Dictionary<string, int> tem;
            _currGameModel.statusDic.TryGetValue(key, out tem);
            return tem;
        }

        private void OnDestroy()
        {
            if (saveGameThread != null)
                saveGameThread.Abort();
        }
    }
}


