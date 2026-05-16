using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Events;

public class FixARSceneUI : EditorWindow
{
    [MenuItem("Tools/Fix AR Card UI")]
    public static void FixUI()
    {
        string scenePath = "Assets/Scenes/ARCardScan.unity";
        var scene = EditorSceneManager.OpenScene(scenePath);

        GameObject panel = null;
        var allPanels = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in allPanels)
        {
            if (go.name == "PanelInfoTanaman" && go.scene == scene)
            {
                panel = go;
                break;
            }
        }

        if (panel == null)
        {
            Debug.LogError("PanelInfoTanaman not found in scene! Pastikan panel tersebut ada di Hierarchy.");
            return;
        }

        // Find the button (usually the one used for Klaim)
        Button btn = panel.GetComponentInChildren<Button>();
        if (btn == null)
        {
            Debug.LogError("No Button found in PanelInfoTanaman!");
            return;
        }

        // Change Text
        TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
        {
            btnText.text = "Kembali ke Menu";
        }

        // Change OnClick
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        if (gm != null)
        {
            SerializedObject so = new SerializedObject(gm);
            so.FindProperty("panelInfoTanaman").objectReferenceValue = panel;
            
            var textNama = panel.transform.Find("Text_NamaTanaman");
            if (textNama != null) so.FindProperty("textNamaTanaman").objectReferenceValue = textNama.GetComponent<TextMeshProUGUI>();
            
            var textDesk = panel.transform.Find("Text_DeskripsiTanaman");
            if (textDesk != null) so.FindProperty("textDeskripsiTanaman").objectReferenceValue = textDesk.GetComponent<TextMeshProUGUI>();
            
            so.ApplyModifiedProperties();

            // Clear existing persistent listeners using SerializedObject
            SerializedObject soBtn = new SerializedObject(btn);
            SerializedProperty persistentCalls = soBtn.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
            if (persistentCalls != null)
            {
                persistentCalls.ClearArray();
                soBtn.ApplyModifiedProperties();
            }

            // Add the new listener
            UnityEventTools.AddPersistentListener(btn.onClick, gm.KembaliKeHomeDariAR);
            
            Debug.Log("Successfully updated Button and GameManager references in ARCardScan scene!");
        }
        else
        {
            Debug.LogWarning("GameManager not found in scene. Click listener not set.");
        }

        EditorSceneManager.SaveScene(scene);
        Debug.Log("AR Scene UI fixed and saved.");
    }
}
