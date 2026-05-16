using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Text;

public class LogMarkerNames
{
    [MenuItem("Tools/Log Marker Names")]
    public static void Log()
    {
        string scenePath = "Assets/Scenes/ARCardScan.unity";
        var scene = EditorSceneManager.OpenScene(scenePath);
        
        var scripts = Object.FindObjectsOfType<KartuTanamanAR>();
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Scene: " + scene.name);
        sb.AppendLine("Found " + scripts.Length + " markers with KartuTanamanAR:");
        
        foreach (var s in scripts)
        {
            sb.AppendLine("- " + s.gameObject.name);
        }

        File.WriteAllText("Marker_Names_Log.txt", sb.ToString());
        Debug.Log("Marker names logged to Marker_Names_Log.txt. PLEASE READ THIS FILE.");
    }
}
