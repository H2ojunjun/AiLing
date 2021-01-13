using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton <T>:MonoBehaviour where T:MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    private static bool isQuiting = false;
    //懒汉单例
    public static T Instance
    {
        get
        {
            if (isQuiting)
            {
                return null;
            }
            //线程锁
            lock (_lock)
            {
                if(_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (FindObjectsOfType<T>().Length > 1)
                    {
                        return _instance;
                    }

                    else if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString();
                        DontDestroyOnLoad(singleton);
                    }
                    else
                        DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }
    }


    private void OnDestroy()
    {
        isQuiting = true;
    }
}




