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

        // Find the GameManager
        GameManager gm = GameObject.FindObjectOfType<GameManager>();

        Transform klaimBtnTrans = panel.transform.Find("Btn_Klaim10Poin!");
        Transform kembaliBtnTrans = panel.transform.Find("Btn_KembaliKeMenu");

        if (kembaliBtnTrans == null && klaimBtnTrans != null)
        {
            // Duplikasi Btn_Klaim10Poin! untuk membuat Btn_KembaliKeMenu secara dinamis
            GameObject newBtnGo = GameObject.Instantiate(klaimBtnTrans.gameObject, panel.transform);
            newBtnGo.name = "Btn_KembaliKeMenu";
            kembaliBtnTrans = newBtnGo.transform;
        }

        if (kembaliBtnTrans == null)
        {
            Debug.LogError("Btn_KembaliKeMenu not found and could not be created from Btn_Klaim10Poin!");
            return;
        }

        // Atur agar Btn_KembaliKeMenu aktif dan diposisikan dengan benar
        kembaliBtnTrans.gameObject.SetActive(true);
        RectTransform rt = kembaliBtnTrans.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = Vector2.zero;
            rt.anchorMin = new Vector2(0.28f, 0.05f);
            rt.anchorMax = new Vector2(0.73f, 0.12f);
            rt.pivot = new Vector2(0.5f, 0.5f);
        }

        // Atur teks tombol kembali
        TextMeshProUGUI btnText = kembaliBtnTrans.GetComponentInChildren<TextMeshProUGUI>(true);
        if (btnText != null)
        {
            btnText.text = "Kembali ke Menu";
        }

        // Hubungkan referensi GameManager
        if (gm != null)
        {
            SerializedObject so = new SerializedObject(gm);
            so.FindProperty("panelInfoTanaman").objectReferenceValue = panel;
            
            // Cari teks menggunakan pencarian rekursif agar tidak rusak jika dipindah ke dalam grup layout
            var allTexts = panel.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var txt in allTexts)
            {
                if (txt.gameObject.name == "Text_NamaTanaman")
                    so.FindProperty("textNamaTanaman").objectReferenceValue = txt;
                else if (txt.gameObject.name == "Text_DeskripsiTanaman")
                    so.FindProperty("textDeskripsiTanaman").objectReferenceValue = txt;
            }
            so.ApplyModifiedProperties();

            // Atur listener klik untuk Btn_KembaliKeMenu
            Button btn = kembaliBtnTrans.GetComponent<Button>();
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
            
            Debug.Log("Successfully updated Btn_KembaliKeMenu and GameManager references in ARCardScan scene!");
        }
        else
        {
            Debug.LogWarning("GameManager not found in scene. Click listener not set.");
        }

        // Nonaktifkan Btn_Klaim10Poin! agar tidak tumpang tindih
        if (klaimBtnTrans != null)
        {
            klaimBtnTrans.gameObject.SetActive(false);
            Debug.Log("Deactivated Btn_Klaim10Poin! in AR Card UI.");
        }

        EditorSceneManager.SaveScene(scene);
        Debug.Log("AR Scene UI fixed and saved.");
    }
}
