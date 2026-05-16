using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Text;

public class HierarchyLogger
{
    [MenuItem("Tools/Log UI Hierarchy")]
    public static void LogUI()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Scene: " + EditorSceneManager.GetActiveScene().name);
        
        GameObject canvas = GameObject.Find("UI_Canvas");
        if (canvas != null)
        {
            sb.AppendLine("UI_Canvas found.");
            LogRecursive(canvas.transform, 0, sb);
        }
        else
        {
            sb.AppendLine("UI_Canvas NOT found.");
        }

        File.WriteAllText("UI_Hierarchy_Log.txt", sb.ToString());
        Debug.Log("Hierarchy logged to UI_Hierarchy_Log.txt");
    }

    private static void LogRecursive(Transform t, int depth, StringBuilder sb)
    {
        string indent = new string('-', depth * 2);
        sb.AppendLine($"{indent} {t.name} (Active: {t.gameObject.activeSelf})");
        foreach (Transform child in t)
        {
            LogRecursive(child, depth + 1, sb);
        }
    }
}
