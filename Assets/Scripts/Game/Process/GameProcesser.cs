using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;

//用于管理游戏进程，包括新建档，读档，存档等
public class GameProcesser:MonoSingleton<GameProcesser>
{
    public bool enable = false;

    private string _savePath;

    List<GameModel> archives = new List<GameModel>();

    void Awake()
    {
        if (enable == false)
            return;
        //Debug.Log(Application.persistentDataPath);
        _savePath = Application.persistentDataPath+"/Archives";
        if (!File.Exists(_savePath))
        {
            try
            {
                Directory.CreateDirectory(_savePath);
            }
            catch(Exception ex)
            {
                Debug.LogError("create directory fail" + ex.ToString());
            }

        }

        string[] files = Directory.GetFiles(_savePath);
        if (files.Length == 0)
        {
            GameModel model= new GameModel();
            SaveGame(model);
            LoadGame(model);
            Debug.Log("new archive");
        }
        else
        {
            for(int i = 0; i < files.Length; i++)
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = File.Open(_savePath + "/gamesave" + (i + 1).ToString() + ".txt",FileMode.Open);
                GameModel model = bf.Deserialize(fs) as GameModel;
                fs.Close();
                archives.Add(model);
                Debug.Log("id:"+model.id);
            }
        }
    }

    public void SaveGame(GameModel model)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(_savePath + "/gamesave"+model.id.ToString() + ".txt");
        bf.Serialize(file, model);
        file.Close();
    }

    public void LoadGame(GameModel model)
    {

    }

    public void NewGame()
    {
        GameModel model = new GameModel();
        model.id = archives.Count + 1;
        SaveGame(model);
    }

    void Update()
    {
        
    }
}
