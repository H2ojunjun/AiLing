using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AiLing
{
    public class TimerManager : MonoSingleton<TimerManager>
    {
        //已经发放的id的最大值
        private int _allocID = -1;
        //用于保存要删除的timer的索引的列表
        private List<int> _removeIndexes = new List<int>();
        //保存所有Timer的列表
        private List<Timer> _allTimer = new List<Timer>();
        //保存在timerDic中空闲的index
        public Stack<int> freeTimerIndexes = new Stack<int>();

        /// <summary>
        /// 返回的值是timer在timerList中的index,如果该timer是有限循环的，则不需要对该int有全局引用
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
            int timerKey = -1;
            if (freeTimerIndexes.Count == 0 || freeTimerIndexes == null)
            {
                timerKey = ++_allocID;
                _allTimer.Add(timer);
            }
            else
            {
                timerKey = freeTimerIndexes.Pop();
                _allTimer[timerKey] = timer;
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
                //Debug.LogError("timerList is null or no element");
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
            _allTimer[id] = null;
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
            _removeIndexes.Clear();
            foreach (var timer in _allTimer)
            {
                if (timer == null)
                    continue;
                if (timer.enable)
                {
                    timer.Update();
                }
                if (timer.isFinished())
                    _removeIndexes.Add(timer.id);
            }

            foreach(var index in _removeIndexes)
            {
                RemoveTimer(index);
            }
        }
    }

}
