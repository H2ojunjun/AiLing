using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class EventTimeCache : GameReferenceCache
    {
        GameObject _obj;
        Dictionary<GameEvent, int> _timeDic = new Dictionary<GameEvent, int>();
        public override void Cache()
        {
            _timeDic.Clear();
            GameEvent[] events = _obj.GetComponents<GameEvent>();
            foreach(var eve in events)
            {
                _timeDic.Add(eve,eve.leftTimes);
            }
        }

        public override void Init(GameObject obj)
        {
            _obj = obj;
        }

        public override void Read()
        {
            GameEvent[] events = _obj.GetComponents<GameEvent>();
            foreach (var eve in events)
            {
                int oldTimeValue;
                if(_timeDic.TryGetValue(eve,out oldTimeValue))
                {
                    eve.leftTimes = oldTimeValue;
                }
            }
        }
    }
}

