using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AiLing
{
    public class GameMarkPointManager : MonoSingleton<GameMarkPointManager>
    {
        //当前场景的mark的根节点
        GameObject _markRoot;

        GameObject _currMark;

        string _markRootName = "markRoot";

        [HideInInspector]
        public List<GameObject> marks = new List<GameObject>();

        public void GetRoot()
        {
            _markRoot = GameObject.Find(_markRootName);
            if (_markRoot == null)
                Debug.LogError("cant find "+_markRootName+" in hierarchy,please mark sure you have created it!");
            InitAllMark();
        }

        //当前场景为新场景时调用，设置_currMark为出生点
        public void SetStartMark()
        {
            _currMark =marks[0];
        }

        public void GoToMark(int index)
        {
            _currMark = marks[index-1];
            //将之前的存档点禁用
            for (int i = 0; i <= index - 1; i++)
            {
                EventListener el = marks[i].GetComponent<EventListener>();
                if (el != null)
                    el.active = false;
            }
            PhsicsHelper.TransPort(GameMainManager.Instance.player.transform,_currMark.transform);
            Debug.Log("goto index:"+index);
        }

        private void InitAllMark()
        {
            marks.Clear();
            for (int i = 0; i < _markRoot.transform.childCount; i++)
            {
                GameObject mark = _markRoot.transform.Find("mark" + (i + 1).ToString()).gameObject;
                marks.Add(mark);
            }
        }
    }
}

