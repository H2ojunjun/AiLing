using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    //参数为float time，该timer所剩余的时间
    public delegate void TimeChangeCallBack(float time);

    public delegate void TimerEndCallBack();

    public delegate void TimerStartCallBack();

    /// <summary>
    /// 对于要延迟一段时间后调用一次方法的设置delay=0，直接设置totalTime和loopTime,然后再设置timeEnd即可
    /// 对于要延迟一段时间后在一段时间内调用方法的，将delay设置为延迟时间，然后设置timeChange
    /// </summary>
    public class Timer
    {
        //参数为float time，该timer所剩余的时间
        private TimeChangeCallBack _timeChange;
        private TimerEndCallBack _timeEnd;
        private TimerStartCallBack _timeStart;

        public float totalTime;
        public float leftTime;
        public float totalDelay;
        //如果delay>0，则timeStart和后面的一系列操作都会在delay完后调用
        public float delay;
        public int id;
        //为false则不会帧调用update函数
        public bool enable;
        public bool isDelaying;
        //-1表示无限循环，直到被timeManager remove
        public int loopTime;

        public event TimeChangeCallBack timeChange { add { _timeChange += value; } remove { _timeChange -= value; } }
        public event TimerEndCallBack timeEnd { add { _timeEnd += value; } remove { _timeEnd -= value; } }
        public event TimerStartCallBack timeStart { add { _timeStart += value; } remove { _timeStart -= value; } }

        public Timer(float totalTime, float totalDelay, int loopTime)
        {
            this.totalTime = totalTime;
            this.leftTime = totalTime;
            this.totalDelay = totalDelay;
            this.delay = totalDelay;
            this.enable = true;
            this.loopTime = loopTime;
            if (totalDelay > 0)
                isDelaying = true;
        }

        /// <summary>
        /// 开始计时，但要在delay完之后才会调用timeStart和timeChange
        /// </summary>
        public void Start()
        {
            if (!isDelaying)
                _timeStart?.Invoke();
        }

        public void Loop()
        {
            this.leftTime = totalTime;
            this.delay = totalDelay;
            this.enable = true;
            if (totalDelay > 0)
                isDelaying = true;
            else
                _timeStart?.Invoke();
        }

        /// <summary>
        /// 更新Timer
        /// </summary>
        public void Update()
        {
            if (leftTime <= 0)
            {
                loopTime--;
                _timeEnd?.Invoke();
                enable = false;
                if (loopTime > 0)
                {
                    Loop();
                }
                return;
            }
            if (delay > 0)
            {
                delay -= Time.deltaTime;
                return;
            }
            else if (isDelaying)
            {
                _timeStart?.Invoke();
                isDelaying = false;
                return;
            }
            leftTime -= Time.deltaTime;
            if (leftTime < 0)
                leftTime = 0;
            _timeChange?.Invoke(leftTime);
        }

        /// <summary>
        /// 暂停,如还在delay期间就不管
        /// </summary>
        public void Pause()
        {
            if (delay > 0)
                return;
            enable = false;
        }

        /// <summary>
        /// 继续
        /// </summary>
        public void Continue()
        {
            if (delay > 0)
                return;
            enable = true;
        }

        //如果loopTime==0,则表示该Timer生命周期结束，需要被remove
        public bool isFinished()
        {
            return loopTime == 0;
        }
    }
}

