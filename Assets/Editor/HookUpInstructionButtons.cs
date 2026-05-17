using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEditor.Events;

public class HookUpInstructionButtons : EditorWindow
{
    [MenuItem("Tools/Aktifkan Tombol di Scene Ini")]
    public static void HookUpButtons()
    {
        // Ambil scene yang sedang dibuka
        var scene = EditorSceneManager.GetActiveScene();

        // 1. Pastikan EventSystem ada agar tombol bisa ditekan
        var es = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (es == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // 2. Pastikan GraphicRaycaster ada di Canvas
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (var c in canvases)
        {
            if (c.GetComponent<GraphicRaycaster>() == null)
                c.gameObject.AddComponent<GraphicRaycaster>();
        }

        if (canvases.Length == 0)
        {
            Debug.LogError("Tidak ada Canvas di scene ini!");
            return;
        }
        
        // 3. Pasang SceneLoader & ButtonSoundManager ke Canvas utama
        GameObject managerObj = canvases[0].gameObject; 

        SceneLoader loader = managerObj.GetComponent<SceneLoader>();
        if (loader == null) loader = managerObj.AddComponent<SceneLoader>();

        ButtonSoundManager soundManager = managerObj.GetComponent<ButtonSoundManager>();
        if (soundManager == null) soundManager = managerObj.AddComponent<ButtonSoundManager>();

        // 4. Deteksi semua tombol yang Anda buat dan hidupkan!
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        int hooked = 0;
        foreach (var btn in buttons)
        {
            // Abaikan tombol yang ada di prefab / luar scene
            if (btn.gameObject.scene != scene) continue; 
            
            // Bersihkan fungsi tombol yang lama agar tidak dobel
            SerializedObject soBtn = new SerializedObject(btn);
            SerializedProperty persistentCalls = soBtn.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
            if (persistentCalls != null)
            {
                persistentCalls.ClearArray();
                soBtn.ApplyModifiedProperties();
            }

            // Sambungkan tombol untuk kembali ke HOMEPAGE
            UnityEventTools.AddPersistentListener(btn.onClick, loader.LoadHOMEPAGE);
            hooked++;
            
            // Matikan halangan raycast pada teks tombol (Penyebab tombol macet)
            var texts = btn.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            foreach(var txt in texts)
            {
                txt.raycastTarget = false;
                EditorUtility.SetDirty(txt);
            }
        }

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log($"✅ Berhasil mengaktifkan {hooked} tombol di scene '{scene.name}'!");
        Debug.Log("Tombol-tombol tersebut sekarang sudah tersambung ke Menu Utama (HOMEPAGE) dan memiliki efek suara otomatis.");
    }
}
