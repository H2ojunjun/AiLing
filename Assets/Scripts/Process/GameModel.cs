using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class GameModel 
{
    public int id;
    public int chapter;
    public int section;

    //新档才调用构造函数
    public GameModel()
    {
        id = 1;
        chapter = 1;
        section = 1;
    }
}
