using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace AiLing
{
    //用于管理游戏进程，包括新建档，读档，存档等
    public class GameProcesser : MonoSingleton<GameProcesser>
    {
        private string _savePath;

        private string _firstScene= "Scene_00";

        public bool enable = false;

        public List<GameModel> archives = new List<GameModel>();

        [HideInInspector]
        public GameModel currGameModel;

        void Awake()
        {
            if (enable == false)
                return;
            _savePath = Application.persistentDataPath + "/Archives";
            //Debug.LogError(_savePath);
            if (!File.Exists(_savePath))
            {
                try
                {
                    Directory.CreateDirectory(_savePath);
                }
                catch (Exception ex)
                {
                    Debug.LogError("create directory fail" + ex.ToString());
                }
            }

            string[] files = Directory.GetFiles(_savePath);
            if (files.Length == 0)
            {
                currGameModel = NewGame();
                LoadGame(currGameModel);
            }
            else
            {
                for (int i = 0; i < files.Length; i++)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream fs = new FileStream(_savePath + "/gamesave" + (i + 1).ToString() + ".txt", FileMode.Open);
                    GameModel model = bf.Deserialize(fs) as GameModel;
                    fs.Close();
                    archives.Add(model);
                    Debug.Log("id:" + model.id+"sceneName:"+model.sceneName);
                }
                UIManager.Instance.CreateAndShow<UIMainMenu>();
            }
            DontDestroyOnLoad(transform.parent.gameObject);
        }

        public void SaveGame(GameModel model)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(_savePath + "/gamesave" + model.id.ToString() + ".txt",FileMode.OpenOrCreate);
            bf.Serialize(stream, model);
            stream.Close();
        }

        public void LoadGame(GameModel model)
        {
            SceneManager.LoadScene(model.sceneName);
            Debug.Log("load gameModel:" + model.id+"SceneName:"+model.sceneName);
        }

        public GameModel NewGame()
        {
            GameModel model = new GameModel();
            model.id = archives.Count + 1;
            model.sceneName = _firstScene;
            model.section = 1;
            SaveGame(model);
            Debug.Log("new archive");
            return model;
        }

        void Update()
        {

        }
    }
}


