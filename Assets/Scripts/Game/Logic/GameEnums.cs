using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GameEnumAttribute : Attribute
    {
        public string CNName;

        public Type enumType;
        public GameEnumAttribute(string CNName,Type enumType=null)
        {
            this.CNName = CNName;
            this.enumType = enumType;
        }
    }

    public enum EMusicName
    {
        [GameEnum("背景音乐")]
        BackGround,
        [GameEnum("汽车碰撞")]
        CarCrash,
    }

    public enum EOperation
    {
        [GameEnum("大于")]
        Bigger,
        [GameEnum("大于等于")]
        BiggerEqual,
        [GameEnum("等于")]
        Equal,
        [GameEnum("小于")]
        Lower,
        [GameEnum("小于等于")]
        LowerEqual,
    }

    public enum EStatus
    {
        [GameEnum("两态开关",typeof(EDoubleSwitch))]
        EDoubleSwitch,
        [GameEnum("三态电梯", typeof(EElevator3))]
        EElevator3,
    }

    public enum EDoubleSwitch
    {
        [GameEnum("开")]
        Open,
        [GameEnum("关")]
        Close
    }

    public enum EElevator3
    {
        [GameEnum("1")]
        One=1,
        [GameEnum("2")]
        Two,
        [GameEnum("3")]
        Three

    }
}

