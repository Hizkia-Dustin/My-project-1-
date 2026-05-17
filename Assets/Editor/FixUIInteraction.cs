using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FixUIInteraction : EditorWindow
{
    [MenuItem("Tools/Auto Fix UI (Tombol Macet)")]
    public static void AutoFixUI()
    {
        Debug.Log("🔍 Mulai memindai dan memperbaiki masalah tombol UI...");

        // 1. Cek EventSystem (Otak deteksi sentuhan)
        EventSystem es = GameObject.FindObjectOfType<EventSystem>();
        if (es == null)
        {
            Debug.Log("⚠️ EventSystem hilang! Membuat EventSystem baru...");
            GameObject esObj = new GameObject("EventSystem");
            esObj.AddComponent<EventSystem>();
            esObj.AddComponent<StandaloneInputModule>();
        }

        // 2. Cek GraphicRaycaster di Canvas (Laser untuk mendeteksi UI)
        Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
        foreach (var canvas in canvases)
        {
            if (canvas.GetComponent<GraphicRaycaster>() == null)
            {
                Debug.Log("⚠️ Canvas '" + canvas.name + "' tidak punya GraphicRaycaster! Menambahkan...");
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        // 3. Matikan RaycastTarget pada teks tombol (Penyebab utama tombol terhalang teks sendiri)
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        int fixedTextCount = 0;
        foreach (var btn in allButtons)
        {
            // Pastikan tombolnya hidup dan menerima raycast
            btn.interactable = true;
            Image btnImage = btn.GetComponent<Image>();
            if (btnImage != null)
            {
                btnImage.raycastTarget = true;
                EditorUtility.SetDirty(btnImage);
            }

            // Matikan raycast pada teks agar klik menembus teks dan mengenai tombolnya
            TextMeshProUGUI[] texts = btn.GetComponentsInChildren<TextMeshProUGUI>(true);
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
            Debug.Log("🔧 Berhasil memperbaiki " + fixedTextCount + " Teks yang sebelumnya menghalangi klik tombol.");
        }

        // 4. Perbaiki urutan Z-Index (Supaya tombol tidak tenggelam di balik background)
        GameObject[] allPanels = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var go in allPanels)
        {
            if (go.name.Contains("PanelInfoTanaman") || go.name.Contains("PanelQuiz") || go.name.Contains("PanelSerigala"))
            {
                Button[] btns = go.GetComponentsInChildren<Button>(true);
                foreach (Button b in btns)
                {
                    // Memastikan grup tombol pindah ke urutan terbawah di Hierarchy (Digambar paling depan!)
                    b.transform.SetAsLastSibling();
                    EditorUtility.SetDirty(b.transform);
                }
            }
        }

        Debug.Log("✅ AUTO FIX SELESAI! Semua penghalang UI sudah dimusnahkan. Silakan Play!");
    }
}
