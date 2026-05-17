using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Events;

public class SyncPanelToARScanner : EditorWindow
{
    [MenuItem("Tools/Sync Panel Ke AR Scanner")]
    public static void SyncPanel()
    {
        // 1. Buka SampleScene untuk mengambil PanelInfoTanaman terbaru
        string sampleScenePath = "Assets/Scenes/SampleScene.unity";
        var sampleScene = EditorSceneManager.OpenScene(sampleScenePath);
        
        GameObject sourcePanel = null;
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in allObjects)
        {
            if (go.name == "PanelInfoTanaman" && go.scene == sampleScene)
            {
                sourcePanel = go;
                break;
            }
        }

        if (sourcePanel == null)
        {
            Debug.LogError("PanelInfoTanaman tidak ditemukan di SampleScene! Pastikan namanya benar.");
            return;
        }

        // 2. Jadikan Prefab (Buat folder Prefabs jika belum ada)
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        string prefabPath = "Assets/Prefabs/PanelInfoTanaman.prefab";
        GameObject prefabAsset = null;
        
        // Jika sudah jadi prefab, Apply saja. Jika belum, jadikan prefab baru.
        if (PrefabUtility.IsPartOfAnyPrefab(sourcePanel))
        {
             string existingPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(sourcePanel);
             if (!string.IsNullOrEmpty(existingPath)) prefabPath = existingPath;
             PrefabUtility.ApplyPrefabInstance(sourcePanel, InteractionMode.UserAction);
             prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        }
        else
        {
             prefabAsset = PrefabUtility.SaveAsPrefabAssetAndConnect(sourcePanel, prefabPath, InteractionMode.UserAction);
        }

        Debug.Log("✅ Panel berhasil disave sebagai Prefab di: " + prefabPath);

        // 3. Buka scene ARCardScan
        string arScenePath = "Assets/Scenes/ARCardScan.unity";
        var arScene = EditorSceneManager.OpenScene(arScenePath);

        Canvas mainCanvas = GameObject.FindObjectOfType<Canvas>();
        if (mainCanvas == null)
        {
            Debug.LogError("Tidak ada Canvas di ARCardScan!");
            return;
        }

        // 4. Cari panel lama di ARCardScan dan Hapus
        GameObject oldPanel = null;
        var arObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in arObjects)
        {
            if (go.name == "PanelInfoTanaman" && go.scene == arScene)
            {
                oldPanel = go;
                break;
            }
        }

        if (oldPanel != null)
        {
            DestroyImmediate(oldPanel);
            Debug.Log("🗑️ Panel lama di ARCardScan berhasil dihapus.");
        }

        // 5. Munculkan Prefab baru di Canvas ARCardScan
        GameObject newPanel = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset);
        newPanel.transform.SetParent(mainCanvas.transform, false);
        newPanel.name = "PanelInfoTanaman"; // Pastikan namanya bersih tanpa (Clone)
        newPanel.transform.SetAsLastSibling(); // Taruh di paling bawah hierarchy canvas

        // 6. Ubah Teks Tombol khusus untuk ARCardScan
        Button btn = newPanel.GetComponentInChildren<Button>(true);
        if (btn != null)
        {
            TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>(true);
            if (btnText != null)
            {
                btnText.text = "Kembali ke Menu";
            }
        }

        // 7. Sambungkan ulang referensi di GameManager
        GameManager gm = GameObject.FindObjectOfType<GameManager>();
        if (gm != null)
        {
            SerializedObject so = new SerializedObject(gm);
            so.FindProperty("panelInfoTanaman").objectReferenceValue = newPanel;

            // SMART HEURISTIC DETECTION UNTUK TEKS
            var allTexts = newPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
            TextMeshProUGUI bestNameText = null;
            TextMeshProUGUI bestDescText = null;

            // Kumpulkan semua teks yang BUKAN bagian dari tombol
            System.Collections.Generic.List<TextMeshProUGUI> validTexts = new System.Collections.Generic.List<TextMeshProUGUI>();
            foreach(var t in allTexts) {
                if (t.GetComponentInParent<Button>(true) == null) validTexts.Add(t);
            }

            // 1. Coba tebak dari nama objeknya dulu
            foreach (var txt in validTexts)
            {
                string objName = txt.gameObject.name.ToLower();
                if (objName.Contains("nama") || objName.Contains("title") || objName.Contains("judul"))
                    bestNameText = txt;
                else if (objName.Contains("deskripsi") || objName.Contains("desc") || objName.Contains("info"))
                    bestDescText = txt;
            }

            // 2. Jika masih gagal (karena namanya diubah drastis), tebak dari ukuran font!
            if (bestNameText == null || bestDescText == null)
            {
                validTexts.Sort((a, b) => b.fontSize.CompareTo(a.fontSize)); // Urutkan dari font paling besar

                if (bestNameText == null && validTexts.Count > 0) 
                    bestNameText = validTexts[0]; // Biasanya judul pakai font paling besar
                
                if (bestDescText == null && validTexts.Count > 1) 
                    bestDescText = validTexts[1]; // Biasanya deskripsi fontnya lebih kecil
            }

            // Terapkan ke GameManager
            if (bestNameText != null) so.FindProperty("textNamaTanaman").objectReferenceValue = bestNameText;
            if (bestDescText != null) so.FindProperty("textDeskripsiTanaman").objectReferenceValue = bestDescText;
            
            so.ApplyModifiedProperties();

            // Ubah Fungsi Klik Tombol jadi KembaliKeHome
            if (btn != null)
            {
                SerializedObject soBtn = new SerializedObject(btn);
                SerializedProperty persistentCalls = soBtn.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
                if (persistentCalls != null)
                {
                    persistentCalls.ClearArray();
                    soBtn.ApplyModifiedProperties();
                }
                UnityEventTools.AddPersistentListener(btn.onClick, gm.KembaliKeHomeDariAR);
            }
        }

        newPanel.SetActive(false); // Sembunyikan secara default
        EditorSceneManager.SaveScene(arScene);
        Debug.Log("🚀 SUKSES! PanelInfoTanaman di ARCardScan telah diganti dengan Prefab dari SampleScene dan siap digunakan!");
    }
}
