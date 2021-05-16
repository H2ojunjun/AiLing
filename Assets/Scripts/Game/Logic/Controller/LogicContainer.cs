using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using System.Reflection;

namespace AiLing
{
    public class LogicContainer : MonoBehaviour
    {
        private List<LogicComponent> _components = new List<LogicComponent>();

        [LabelText("逻辑组件")]
        [ValueDropdown("_componentName")]
        [ListDrawerSettings(Expanded = true,DraggableItems = false)]
        public List<string> components = new List<string>();

        private ValueDropdownList<string> _componentName = new ValueDropdownList<string>();

        [Button("刷新")]
        private void Refresh()
        {
            _componentName.Clear();
            IEnumerable<Type> types =ReflectionHelper.GetSubtypes(Assembly.GetExecutingAssembly(),typeof(LogicComponent),false);
            foreach(var type in types)
            {
                string name = type.Name;
                string fullName = type.FullName;
                _componentName.Add(name,fullName);
            }
        }

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            foreach(var component in components)
            {
                Type t = Type.GetType(component);
                LogicComponent com = Activator.CreateInstance(t) as LogicComponent;
                com.OnCreate();
                com.container = this;
                com.Init(gameObject);
                _components.Add(com);
            }
        }

        public T AddNormalComponent<T>() where T : LogicComponent, new()
        {
            T component = new T();
            component.OnCreate();
            component.container = this;
            _components.Add(component);
            component.Init(gameObject);
            return component;
        }

        public T AddSingletonLogicComponent<T>() where T : LogicComponent, new()
        {
            foreach (var item in _components)
            {
                if (item is T)
                    return item as T;
            }
            T component = new T();
            component.OnCreate();
            component.container = this;
            component.Init(gameObject);
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
