using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    [GameEventInfo(3, "测试事件",new string[] { "测试参数1:string", "测试参数2:float", "测试参数3:int" },2,new string[] {"游戏物体1","游戏物体2" },isUnArtificial = true)]
    public class EventTest : GameEvent
    {
        public override void Excute(object[] normalPara, params object[] unartPara)
        {
            base.Excute(normalPara,unartPara);
            //foreach (var item in para)
            //{
            //    Debug.Log("call eventTest" + item.ToString());
            //}
            object[] unart = unartPara;
        }
    }

    [GameEventInfo(0,"测试事件2")]
    public class EventTest2 : GameEvent
    {
        public override void Excute(object[] normalPara, params object[] unartPara)
        {
            base.Excute(normalPara, unartPara);

        }
    }
}

