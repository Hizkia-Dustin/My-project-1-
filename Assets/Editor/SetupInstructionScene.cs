using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor.Events;

public class SetupInstructionScene : EditorWindow
{
    [MenuItem("Tools/Bikin Scene Instruction (Otomatis)")]
    public static void CreateInstructionScene()
    {
        Debug.Log("Mulai membuat scene instruksi...");

        // 1. Buat scene baru yang kosong
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 2. Tambahkan Kamera & AudioListener
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.1f, 0.15f, 0.1f); 
        camObj.tag = "MainCamera";
        camObj.AddComponent<AudioListener>();

        // 3. Tambahkan EventSystem
        GameObject esObj = new GameObject("EventSystem");
        esObj.AddComponent<EventSystem>();
        esObj.AddComponent<StandaloneInputModule>();

        // 4. Buat Canvas Utama
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // 5. Tambahkan Sistem Suara Tombol & Scene Loader
        canvasObj.AddComponent<ButtonSoundManager>(); 
        SceneLoader loader = canvasObj.AddComponent<SceneLoader>();

        // 6. Buat Background Gelap
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();
        bgImg.color = new Color(0.05f, 0.1f, 0.05f, 1f); 
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // 7. Buat Judul (Title)
        GameObject titleObj = new GameObject("Text_Title");
        titleObj.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI titleTxt = titleObj.AddComponent<TextMeshProUGUI>();
        titleTxt.text = "CARA BERMAIN";
        titleTxt.fontSize = 90;
        titleTxt.fontStyle = FontStyles.Bold;
        titleTxt.alignment = TextAlignmentOptions.Center;
        titleTxt.color = new Color(0.8f, 1f, 0.8f);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1);
        titleRect.anchorMax = new Vector2(0.5f, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.anchoredPosition = new Vector2(0, -150);
        titleRect.sizeDelta = new Vector2(1000, 150);

        // 8. Buat Isi Teks Instruksi
        GameObject contentObj = new GameObject("Text_Content");
        contentObj.transform.SetParent(canvasObj.transform, false);
        TextMeshProUGUI contentTxt = contentObj.AddComponent<TextMeshProUGUI>();
        contentTxt.text = "1. Scan <b>KARTU TANAMAN</b> untuk membaca ensiklopedia tanaman.\n\n2. Mulai Board Game, giliran bermainmu akan ditentukan.\n\n3. Scan <b>KARTU QUIZ</b> untuk menjawab soal dan raih poin!\n\n4. Hati-hati! <b>KARTU SERIGALA</b> akan mengurangi poin jika jawabanmu salah.\n\n<i>Selamat bermain dan kumpulkan poin terbanyak!</i>";
        contentTxt.fontSize = 45;
        contentTxt.alignment = TextAlignmentOptions.TopLeft;
        contentTxt.color = Color.white;
        RectTransform contentRect = contentObj.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.pivot = new Vector2(0.5f, 0.5f);
        contentRect.anchoredPosition = new Vector2(0, 0);
        contentRect.sizeDelta = new Vector2(1200, 600);

        // 9. Buat Tombol Kembali (Interaktif)
        GameObject btnObj = new GameObject("Button_Kembali");
        btnObj.transform.SetParent(canvasObj.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.6f, 0.2f);
        Button btn = btnObj.AddComponent<Button>();
        
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.3f, 0.8f, 0.3f);
        cb.pressedColor = new Color(0.1f, 0.4f, 0.1f);
        cb.selectedColor = new Color(0.2f, 0.6f, 0.2f);
        btn.colors = cb;

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0);
        btnRect.anchorMax = new Vector2(0.5f, 0);
        btnRect.pivot = new Vector2(0.5f, 0);
        btnRect.anchoredPosition = new Vector2(0, 200);
        btnRect.sizeDelta = new Vector2(400, 100);

        // Tambahkan Teks pada Tombol
        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI btnTxt = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnTxt.text = "KEMBALI";
        btnTxt.fontSize = 45;
        btnTxt.fontStyle = FontStyles.Bold;
        btnTxt.alignment = TextAlignmentOptions.Center;
        btnTxt.color = Color.white;
        btnTxt.raycastTarget = false;
        RectTransform btnTxtRect = btnTextObj.GetComponent<RectTransform>();
        btnTxtRect.anchorMin = Vector2.zero;
        btnTxtRect.anchorMax = Vector2.one;
        btnTxtRect.sizeDelta = Vector2.zero;

        // Pasang Event OnClick
        UnityEventTools.AddPersistentListener(btn.onClick, loader.LoadHOMEPAGE);

        // 10. Save Scene
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");
            
        string scenePath = "Assets/Scenes/InstructionSceneAuto.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);

        // Daftarkan ke Build Settings
        var original = EditorBuildSettings.scenes;
        bool exists = false;
        foreach (var s in original)
        {
            if (s.path == scenePath) exists = true;
        }

        if (!exists)
        {
            var newScenes = new EditorBuildSettingsScene[original.Length + 1];
            System.Array.Copy(original, newScenes, original.Length);
            newScenes[original.Length] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = newScenes;
        }

        Debug.Log("✅ SCENE INSTRUCTION BERHASIL DIBUAT (Disimpan sebagai InstructionSceneAuto)!");
    }
}
