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

        // 3. Scan semua tombol di scene
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        Button foundBack = null;
        Button foundStart = null;

        foreach (var btn in allButtons)
        {
            // Pastikan hanya tombol di scene aktif (bukan asset prefab)
            if (btn.gameObject.scene != scene) continue;

            string nameLower = btn.gameObject.name.ToLower();

            // Deteksi Tombol Back (Kembali)
            if (nameLower.Contains("back") || nameLower.Contains("kembali") || nameLower.Contains("arrow") || nameLower.Contains("left"))
            {
                foundBack = btn;
            }
            // Deteksi Tombol Get Started
            else if (nameLower.Contains("start") || nameLower.Contains("get") || nameLower.Contains("play") || nameLower.Contains("mulai"))
            {
                foundStart = btn;
            }
        }

        // Jika tidak terdeteksi via nama, coba tebak via posisi di layar
        if (foundBack == null || foundStart == null)
        {
            foreach (var btn in allButtons)
            {
                if (btn.gameObject.scene != scene) continue;
                if (btn == foundBack || btn == foundStart) continue;

                RectTransform rect = btn.GetComponent<RectTransform>();
                if (rect != null)
                {
                    // Tombol Back biasanya di pojok kiri atas (x negatif / kecil, y positif)
                    if (rect.anchoredPosition.x < 0 && rect.anchoredPosition.y > 0 && foundBack == null)
                    {
                        foundBack = btn;
                    }
                    // Tombol Get Started biasanya di bawah tengah (y negatif / kecil)
                    else if (rect.anchoredPosition.y < 0 && foundStart == null)
                    {
                        foundStart = btn;
                    }
                }
            }
        }

        // 4. Masukkan referensi tombol ke Controller
        if (foundBack != null)
        {
            controller.backButton = foundBack;
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
}
