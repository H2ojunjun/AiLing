using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AiLing
{
    public class TimerManager : MonoSingleton<TimerManager>
    {
        //已经发放的id的最大值
        private int _allocID = 0;
        //保存所有Timer的列表
        private List<Timer> _allTimer = new List<Timer>();
        //保存Timer的所有下标
        private List<int> _timerIndexes = new List<int>();
        //保存在timerDic中空闲的index
        public Stack<int> freeTimerIndexes = new Stack<int>();

        /// <summary>
        /// 返回的值是timer在timerList中的index+1,保留的int的值为0则表示该timer还没被启动或者已经完成
        /// 如果是无限次的，请保留全局引用并且在不需要使用的时候调用RemoveTimer(id)
        /// </summary>
        /// <param name="totalTime"></param>
        /// <param name="delayTime"></param>
        /// <param name="loopTime"></param>
        /// <returns></returns>
        public int AddTimer(float totalTime, float totalDelay, int loopTime, TimerStartCallBack timeStart, TimeChangeCallBack timeChange, TimerEndCallBack timeEnd)
        {
            Timer timer = new Timer(totalTime, totalDelay, loopTime);
            if (timeStart != null)
                timer.timeStart += timeStart;
            if (timeChange != null)
                timer.timeChange += timeChange;
            if (timeEnd != null)
                timer.timeEnd += timeEnd;
            int timerKey = 0;
            if (freeTimerIndexes.Count == 0 || freeTimerIndexes == null)
            {
                timerKey = ++_allocID;
                _allTimer.Add(timer);
            }
            else
            {
                timerKey = freeTimerIndexes.Pop();
                _allTimer[timerKey-1] = timer;
            }
            timer.id = timerKey;
            timer.Start();
            return timerKey;
        }

        public void AddTimeStart(int id, TimerStartCallBack timeStart)
        {
            Timer timer = GetTimer(id);
            if (timer != null && timeStart != null)
                timer.timeStart += timeStart;
        }

        public void AddTimeChange(int id, TimeChangeCallBack timeChange)
        {
            Timer timer = GetTimer(id);
            if (timer != null && timeChange != null)
                timer.timeChange += timeChange;
        }
        public void AddTimeEnd(int id, TimerEndCallBack timeEnd)
        {
            Timer timer = GetTimer(id);
            if (timer != null && timeEnd != null)
                timer.timeEnd += timeEnd;
        }

        public Timer GetTimer(int id)
        {
            if (_allTimer.Count == 0 || _allTimer == null || id == 0)
            {
                return null;
            }

            if (id < 0 || id > _allTimer.Count)
            {
                DebugHelper.LogError("cant find timer by id:" + id);
                return null;
            }
            else
                return _allTimer[id];
        }

        public void RemoveTimer(int id)
        {
            _allTimer[id-1] = null;
            freeTimerIndexes.Push(id);
        }

        public void Pause(int id)
        {
            Timer timer = GetTimer(id);
            if (timer != null)
                timer.Pause();
        }

        public void Continue(int id)
        {
            Timer timer = GetTimer(id);
            if (timer != null)
                timer.Continue();
        }

        public bool IsLeft(int id)
        {
            Timer timer = GetTimer(id);
            if (timer != null)
                return timer.leftTime > 0;
            return false;
        }

        void Update()
        {
            _timerIndexes.Clear();
            for(int i=0;i<_allTimer.Count; i++)
            {
                if (_allTimer[i] != null)
                {
                    _timerIndexes.Add(i);
                }
            }
            foreach(var index in _timerIndexes)
            {
                Timer timer = _allTimer[index];
                //再一次判空是为了防止在改循环中timer被remove
                if(timer == null)
                {
                    continue;
                }
                if (timer.enable)
                {
                    timer.Update();
                }
                if (timer.isFinished())
                    RemoveTimer(timer.id);
            }
        }
    }

}
