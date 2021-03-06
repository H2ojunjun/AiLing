using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace AiLing
{
    public class UIManager : MonoSingleton<UIManager>
    {
        public Dictionary<string, UIBase> uiDic = new Dictionary<string, UIBase>();

        public const string UI_PATH = "Prefabs/UI/ui_normal";

        public T CreateNewUI<T>() where T : UIBase, new()
        {
            UIBase t = new T();
            if (uiDic.ContainsKey(t.componentName) && t.isSingle && uiDic[t.componentName].owner != null)
                return uiDic[t.componentName] as T;
            UIPackage.AddPackage("UI/" + t.PackageName + "/" + t.PackageName);
            GameObject obj = Instantiate(Resources.Load<GameObject>(UI_PATH), Vector3.zero, Quaternion.identity) as GameObject;
            obj.layer = 5;
            obj.name = t.componentName;
            UIPanel panel = obj.GetComponent<UIPanel>();
            panel.packageName = t.PackageName;
            panel.componentName = t.componentName;
            panel.CreateUI();
            t.mainCom = panel.ui;
            if (uiDic.ContainsKey(t.componentName))
            {
                uiDic[t.componentName] = t;
            }
            else
                uiDic.Add(t.componentName, t);
            t.owner = obj;
            obj.SetActive(false);
            return t as T;
        }

        public void InitUI(UIBase ui)
        {
            if (ui == null)
            {
                DebugHelper.LogError("ui is null when InitUI");
                return;
            }
            ui.Init();
        }

        public void ShowUI(UIBase ui)
        {
            if (ui == null)
            {
                DebugHelper.LogError("ui is null when ShowUI");
                return;
            }
            ui.OnShow();
            ui.owner.SetActive(true);
            ui.isShowing = true;
        }

        public void CloseUI(UIBase ui)
        {
            if (ui == null)
            {
                DebugHelper.LogError("ui is null when CloseUI");
                return;
            }
            ui.OnClose();
            uiDic.Remove(ui.componentName);
            ui.isShowing = false;
            Destroy(ui.owner);
        }

        public void HideUI(UIBase ui)
        {
            if (ui == null)
            {
                DebugHelper.LogError("ui is null when HideUI");
                return;
            }
            ui.OnHide();
            ui.owner.SetActive(false);
            ui.isShowing = false;
        }

        public void CreateAndShow<T>() where T : UIBase, new()
        {
            T t = CreateNewUI<T>();
            InitUI(t);
            ShowUI(t);
        }
    }
}

