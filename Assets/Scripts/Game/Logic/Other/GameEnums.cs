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
        public GameEnumAttribute(string CNName, Type enumType = null)
        {
            this.CNName = CNName;
            this.enumType = enumType;
        }
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

    public enum EEaseType
    {
        [GameEnum("线性插值")]
        Liner
    }

    public enum EGate
    {
        [GameEnum("无")]
        None = 0,
        [GameEnum("与")]
        And = 1,
        [GameEnum("或")]
        Or,
        [GameEnum("异或")]
        XOr,
        [GameEnum("同或")]
        WithOr,
    }

    public enum ESetStatusTiming
    {
        [GameEnum("无")]
        None = 0,
        [GameEnum("结束时")]
        Finish = 1,
        [GameEnum("开始时")]
        Start = 2
    }

    //此枚举中字段对应的枚举必须严格按照从1开始连续的值！
    public enum EStatus
    {
        [GameEnum("两态开关", typeof(EDoubleSwitch))]
        EDoubleSwitch = 1,
        [GameEnum("三态电梯", typeof(EElevator3))]
        EElevator3,
        [GameEnum("是否下落", typeof(EFallen))]
        EFallen,
        [GameEnum("是否破碎", typeof(EBreak))]
        EBreak,
    }

    public enum EDoubleSwitch
    {
        [GameEnum("开")]
        Open = 1,
        [GameEnum("关")]
        Close
    }

    public enum EElevator3
    {
        [GameEnum("1")]
        One = 1,
        [GameEnum("2")]
        Two,
        [GameEnum("3")]
        Three
    }

    public enum EFallen
    {
        [GameEnum("未下落")]
        up = 1,
        [GameEnum("已下落")]
        down
    }

    public enum EBreak
    {
        [GameEnum("未破碎")]
        complete =1,
        [GameEnum("已破碎")]
        broken,
    }

    public enum EReferenceContent
    {
        [GameEnum("TransForm", typeof(TransformReferenceCache))]
        ETransform = 1,
        [GameEnum("Material", typeof(MaterialReferenceCache))]
        EMaterial,
        [GameEnum("Shatter", typeof(ShatterCache))]
        EShatter,
        [GameEnum("Active", typeof(SetActiveCache))]
        EActive,
        [GameEnum("EventTime", typeof(EventTimeCache))]
        EEventTime,
    }
}

