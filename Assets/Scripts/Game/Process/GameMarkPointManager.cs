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

        public List<GameObject> marks = new List<GameObject>();

        public void GetRoot()
        {
            _markRoot = GameObject.Find(_markRootName);
            if (_markRoot == null)
                Debug.LogError("cant find "+_markRootName+" in hierarchy,please mark sure you have created it!");
            InitAllMark();
        }

        private void InitAllMark()
        {
            marks.Clear();
            for(int i = 0; i < _markRoot.transform.childCount; i++)
            {
                GameObject mark = _markRoot.transform.Find("mark"+(i+1).ToString()).gameObject;
                marks.Add(mark);
            }
        }

        //当前场景为新场景时调用，设置_currMark为出生点
        public void SetStartMark()
        {
            _currMark =marks[0];
        }

        public void GoToMark(int index)
        {
            _currMark = marks[index-1];
            PhsicsHelper.TransPort(PlayerController.Instance.transform,_currMark.transform);
            Debug.Log("goto index:"+index);
        }
    }
}

