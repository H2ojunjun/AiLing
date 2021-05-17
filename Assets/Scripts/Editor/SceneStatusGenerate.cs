using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace AiLing
{
    public class SceneStatusGenerate
    {
        [MenuItem("工具/生成状态数据")]
        static void GenerateStatusID()
        {
            EditorSceneManager.SaveOpenScenes();
            foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
            {
                if (s.enabled)
                {
                    string name = s.path;
                    EditorSceneManager.OpenScene(name);
                    int id = 1;
                    foreach (StatusInfo sta in Object.FindObjectsOfType(typeof(StatusInfo)))
                    {
                        id++;
                    }
                }
            }
        }
    }
}

