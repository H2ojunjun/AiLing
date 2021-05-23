using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public enum ESignal{
        Signal_TransForm_Cache_Read =1,
    }

    public delegate void SignalDelegate(params object[] para);
    public class SignalManager : MonoSingleton<SignalManager>
    {
        private Dictionary<ESignal,List<SignalDelegate>> _functionDic = new Dictionary<ESignal, List<SignalDelegate>>();

        public void Register(ESignal signal, SignalDelegate func)
        {
            List<SignalDelegate> funcs;
            if(_functionDic.TryGetValue(signal,out funcs))
            {
                if(!funcs.Contains(func))
                    funcs.Add(func);
                return;
            }
            funcs = new List<SignalDelegate>();
            funcs.Add(func);
            _functionDic.Add(signal, funcs);
        }

        public void RemoveFunction(ESignal signal, SignalDelegate func)
        {
            List<SignalDelegate> funcs;
            if (_functionDic.TryGetValue(signal, out funcs))
            {
                for(int i = 0; i < funcs.Count; i++)
                {
                    if(funcs[i] == func)
                        funcs.RemoveAt(i);
                    return;
                }
                DebugHelper.LogError("key: " + signal + "doesnt have value:" + func);
            }
            DebugHelper.LogError("key: " + signal + "doesnt exist in dictionary");
        }

        public void CallSignal(ESignal signal,params object[] para)
        {
            List<SignalDelegate> funcs;
            if (_functionDic.TryGetValue(signal, out funcs))
            {
                foreach(var item in funcs)
                {
                    item.Invoke(para);
                }
                return;
            }
            DebugHelper.LogError("key: " + signal + "doesnt exist in dictionary");
        }
    }
}

