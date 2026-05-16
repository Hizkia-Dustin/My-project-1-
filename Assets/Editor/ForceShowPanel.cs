using UnityEngine;
using UnityEditor;

public class ForceShowPanel
{
    [MenuItem("Tools/Select and Show Panel")]
    public static void Show()
    {
        GameObject panel = null;
        var all = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in all)
        {
            if (go.name.Contains("Panel") && go.name.Contains("Tanaman") && go.hideFlags == HideFlags.None)
            {
                panel = go;
                break;
            }
        }

        if (panel != null)
        {
            panel.SetActive(true);
            Selection.activeGameObject = panel;
            EditorGUIUtility.PingObject(panel);
            Debug.Log($"Ditemukan panel: {panel.name}. Telah diaktifkan!");
        }
        else
        {
            Debug.LogError("Panel dengan nama 'Panel' & 'Tanaman' TIDAK ditemukan! Mungkin terhapus?");
        }

        // Cek juga GameManager
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        if (gm == null) Debug.LogError("GameManager TIDAK ditemukan di scene!");
        else if (gm.panelInfoTanaman == null) Debug.LogWarning("GameManager ada, tapi kolom Panel Info Tanaman masih KOSONG!");
    }
}
