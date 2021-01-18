using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoSingleton<TimerManager>
{
    public List<Timer> timerList = new List<Timer>();

    void Start()
    {
        
    }

    /// <summary>
    /// 返回的值是timer在timerList中的index,如果该timer是有限循环的，则不需要对该int有全局引用
    /// 如果是无限次的，请保留全局引用并且在不需要使用的时候调用RemoveTimer(id)
    /// </summary>
    /// <param name="totalTime"></param>
    /// <param name="delayTime"></param>
    /// <param name="loopTime"></param>
    /// <returns></returns>
    public int AddTimer(float totalTime,float totalDelay,int loopTime,TimerStartCallBack timeStart,TimeChangeCallBack timeChange,TimerEndCallBack timeEnd)
    {
        Timer timer = new Timer(totalTime, totalDelay, loopTime);
        if (timeStart != null)
            timer.timeStart += timeStart;
        if (timeChange != null)
            timer.timeChange += timeChange;
        if (timeEnd != null)
            timer.timeEnd += timeEnd;
        timerList.Add(timer);
        timer.Start();
        return timerList.Count;
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

    private Timer GetTimer(int id)
    {
        if (id < 0 || id > timerList.Count)
        {
            Debug.LogError("cant find timer by id:" + id);
            return null;
        }
        else
            return timerList[id];
    }

    public void RemoveTimer(int id)
    {
        timerList.RemoveAt(id);
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

    // Update is called once per frame
    void Update()
    {
        foreach(var timer in timerList)
        {
            if (timer.enable)
            {
                timer.Update();
                if (timer.isFinished())
                    RemoveTimer(timer.id);
            }
        }
    }
}
