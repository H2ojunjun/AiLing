using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameModel 
{
    //存档ID
    public int id;
    //当前场景名
    public string sceneName;
    //当前小存档点
    public int section;

    //新档才调用构造函数
    public GameModel()
    {
        id = 1;
        //sceneName = ;
        section = 1;
    }
}
