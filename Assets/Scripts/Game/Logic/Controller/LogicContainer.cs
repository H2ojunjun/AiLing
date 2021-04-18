using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AiLing
{
    public class LogicContainer : MonoBehaviour
    {
        private List<LogicComponent> _components = new List<LogicComponent>();

        public T AddNormalComponent<T>() where T : LogicComponent, new()
        {
            T component = new T();
            component.OnCreate();
            component.container = this;
            _components.Add(component);
            return component;
        }

        public T AddSingletonLogicComponent<T>() where T : LogicComponent, new()
        {
            foreach (var item in _components)
            {
                if (item is T)
                    return null;
            }
            T component = new T();
            component.OnCreate();
            component.container = this;
            _components.Add(component);
            return component;
        }

        public T GetSingletonLogicCompoent<T>() where T : LogicComponent
        {
            foreach (var item in _components)
            {
                if (item is T)
                {
                    return item as T;
                }
            }
            return null;
        }

        public List<T> GetLogicComponents<T>() where T : LogicComponent
        {
            List<T> array = new List<T>();
            foreach (var item in _components)
            {
                if (item is T)
                {
                    array.Add(item as T);
                }
            }
            return array;
        }

        public void RemoveLogicComponent<T>()
        {
            List<int> _indexPrepareForDelete = new List<int>();
            for (int i = 0; i < _components.Count; i++)
            {
                if (_components[i] is T)
                {
                    _indexPrepareForDelete.Add(i);
                }
            }
            foreach (var index in _indexPrepareForDelete)
            {
                _components[index].OnDestory();
                _components.RemoveAt(index);
            }
        }

        public void RemoveAllLogicComponent()
        {
            for(int i = _components.Count - 1; i >= 0; i--)
            {
                _components[i].OnDestory();
                _components.RemoveAt(i);
            }
        }

        private void OnDestroy()
        {
            RemoveAllLogicComponent();
        }
    }
}
