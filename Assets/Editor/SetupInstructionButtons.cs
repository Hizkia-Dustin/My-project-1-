using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public class SetupInstructionButtons : EditorWindow
{
    [MenuItem("Tools/Setup Tombol di Scene Aktif")]
    public static void AutoSetupScene()
    {
        var scene = EditorSceneManager.GetActiveScene();
        Debug.Log($"🔍 Memulai Setup Tombol Otomatis di Scene: '{scene.name}'...");

        // 1. Cari Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("❌ Tidak ditemukan Canvas di scene! Silakan buat Canvas terlebih dahulu.");
            return;
        }

        // 2. Tambahkan / Ambil komponen Controller & Sound Manager pada Canvas
        InstructionController controller = canvas.GetComponent<InstructionController>();
        if (controller == null)
        {
            controller = canvas.gameObject.AddComponent<InstructionController>();
            Debug.Log("➕ Menambahkan komponen 'InstructionController' ke Canvas.");
        }

        ButtonSoundManager soundManager = canvas.GetComponent<ButtonSoundManager>();
        if (soundManager == null)
        {
            soundManager = canvas.gameObject.AddComponent<ButtonSoundManager>();
            Debug.Log("➕ Menambahkan komponen 'ButtonSoundManager' ke Canvas.");
        }

        // 3. Scan semua tombol di scene & deteksi berdasarkan tinggi posisinya (paling tinggi = Back, paling rendah = Get Started)
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        System.Collections.Generic.List<Button> sceneButtons = new System.Collections.Generic.List<Button>();

        foreach (var btn in allButtons)
        {
            if (btn.gameObject.scene == scene)
            {
                sceneButtons.Add(btn);
            }
        }

        Button foundBack = null;
        Button foundStart = null;

        if (sceneButtons.Count >= 2)
        {
            // Urutkan tombol berdasarkan tinggi posisi Y di world space (Descending: paling tinggi di indeks 0)
            sceneButtons.Sort((a, b) =>
            {
                RectTransform rectA = a.GetComponent<RectTransform>();
                RectTransform rectB = b.GetComponent<RectTransform>();
                float yA = rectA != null ? rectA.position.y : 0f;
                float yB = rectB != null ? rectB.position.y : 0f;
                return yB.CompareTo(yA);
            });

            foundBack = sceneButtons[0]; // Tombol paling tinggi = Back (Kembali ke Homepage)
            foundStart = sceneButtons[sceneButtons.Count - 1]; // Tombol paling rendah = Start (Mulai Game)

            Debug.Log($"📊 [SmartHeightDetection] Mengidentifikasi {sceneButtons.Count} tombol berdasarkan tinggi posisinya:");
            Debug.Log($"   🔼 Paling Atas (Back Button): '{foundBack.gameObject.name}'");
            Debug.Log($"   🔽 Paling Bawah (Get Started Button): '{foundStart.gameObject.name}'");
        }
        else
        {
            // Fallback ke deteksi nama jika tombol kurang dari 2
            foreach (var btn in sceneButtons)
            {
                string nameLower = btn.gameObject.name.ToLower();
                if (nameLower.Contains("back") || nameLower.Contains("kembali") || nameLower.Contains("arrow") || nameLower.Contains("left"))
                {
                    foundBack = btn;
                }
                else if (nameLower.Contains("start") || nameLower.Contains("get") || nameLower.Contains("play") || nameLower.Contains("mulai"))
                {
                    foundStart = btn;
                }
            }
        }

        // 4. Masukkan referensi tombol ke Controller & Bersihkan listener yang menumpuk / rusak
        if (foundBack != null)
        {
            controller.backButton = foundBack;
            ClearPersistentListeners(foundBack);
            Debug.Log($"🎯 Berhasil mendeteksi Tombol Back: '{foundBack.gameObject.name}'");
            EditorUtility.SetDirty(controller);
        }
        else
        {
            Debug.LogWarning("⚠️ Tombol 'Back' tidak terdeteksi otomatis. Silakan tarik manual ke kolom 'Back Button' di Inspector Canvas.");
        }

        if (foundStart != null)
        {
            controller.getStartedButton = foundStart;
            ClearPersistentListeners(foundStart);
            Debug.Log($"🎯 Berhasil mendeteksi Tombol Get Started: '{foundStart.gameObject.name}'");
            EditorUtility.SetDirty(controller);
        }
        else
        {
            Debug.LogWarning("⚠️ Tombol 'Get Started' tidak terdeteksi otomatis. Silakan tarik manual ke kolom 'Get Started Button' di Inspector Canvas.");
        }

        // 5. Pastikan EventSystem & GraphicRaycaster terpasang dengan benar
        var es = FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (es == null)
        {
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("➕ Menambahkan EventSystem ke scene.");
        }

        if (canvas.GetComponent<GraphicRaycaster>() == null)
        {
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            Debug.Log("➕ Menambahkan GraphicRaycaster ke Canvas.");
        }

        // 6. Pastikan Camera memiliki AudioListener agar suara terdengar
        Camera mainCam = Camera.main;
        if (mainCam == null) mainCam = FindObjectOfType<Camera>();
        if (mainCam != null)
        {
            if (mainCam.GetComponent<AudioListener>() == null)
            {
                mainCam.gameObject.AddComponent<AudioListener>();
                Debug.Log("🔊 [Audio] Berhasil memasang AudioListener pada Kamera Utama agar suara tombol terdengar!");
                EditorUtility.SetDirty(mainCam.gameObject);
            }
        }

        // Matikan raycast target pada tulisan di dalam tombol
        int fixedTextCount = 0;
        foreach (var btn in new Button[] { foundBack, foundStart })
        {
            if (btn == null) continue;
            var texts = btn.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
            foreach (var txt in texts)
            {
                if (txt.raycastTarget)
                {
                    txt.raycastTarget = false;
                    EditorUtility.SetDirty(txt);
                    fixedTextCount++;
                }
            }
        }

        if (fixedTextCount > 0)
        {
            Debug.Log($"🔧 Mematikan Raycast Target pada {fixedTextCount} teks di dalam tombol agar klik lancar.");
        }

        // Tandai scene telah berubah agar bisa di-save
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("✅ SETUP SELESAI! Silakan jalankan game!");
    }

    private static void ClearPersistentListeners(Button btn)
    {
        if (btn == null) return;
        SerializedObject soBtn = new SerializedObject(btn);
        SerializedProperty persistentCalls = soBtn.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
        if (persistentCalls != null)
        {
            persistentCalls.ClearArray();
            soBtn.ApplyModifiedProperties();
            Debug.Log($"🧼 [Scrub] Membersihkan persistent click listeners pada tombol: '{btn.gameObject.name}'");
        }
    }
}
