using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor.Events;

public class SetupCreditScene : EditorWindow
{
    [MenuItem("Tools/Bikin Scene Credit (Otomatis)")]
    public static void CreateCreditScene()
    {
        Debug.Log("Mulai membuat scene Credit Portrait Premium...");

        // 1. Buat scene baru yang kosong
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 2. Tambahkan Kamera & AudioListener
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.035f, 0.094f, 0.055f); // Deep forest dark green
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
        scaler.referenceResolution = new Vector2(800, 600);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0f; // Match Width for perfect portrait responsive scale
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Pasang ButtonSoundManager & SceneLoader ke Canvas
        canvasObj.AddComponent<ButtonSoundManager>(); 
        SceneLoader loader = canvasObj.AddComponent<SceneLoader>();

        // 5. Load asset custom fonts & sprites
        TMP_FontAsset titleFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Fonts/LibreCaslonText-Bold SDF.asset");
        TMP_FontAsset bodyFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Fonts/Geist-VariableFont_wght SDF.asset");
        
        Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/bg_environment.png");
        if (bgSprite == null)
        {
            string[] guids = AssetDatabase.FindAssets("bg_environment t:Sprite");
            if (guids.Length > 0)
                bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        Sprite logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/title_agriquest_ar.png");
        if (logoSprite == null)
        {
            string[] guids = AssetDatabase.FindAssets("title_agriquest_ar t:Sprite");
            if (guids.Length > 0)
                logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        Sprite btnSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Button/btn_gold_pattern.png");
        if (btnSprite == null)
        {
            string[] guids = AssetDatabase.FindAssets("btn_gold_pattern t:Sprite");
            if (guids.Length > 0)
                btnSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        // 6. Buat Background (Responsive full screen)
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bgObj.AddComponent<Image>();
        if (bgSprite != null)
        {
            bgImg.sprite = bgSprite;
            bgImg.color = new Color(0.35f, 0.35f, 0.35f, 1f); // Darken forest bg slightly for readability
        }
        else
        {
            bgImg.color = new Color(0.04f, 0.08f, 0.05f, 1f);
        }
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // 7. Buat Logo/Title Branding di Atas (Responsive percentage-based)
        GameObject logoObj = new GameObject("Logo");
        logoObj.transform.SetParent(canvasObj.transform, false);
        Image logoImg = logoObj.AddComponent<Image>();
        if (logoSprite != null)
        {
            logoImg.sprite = logoSprite;
            logoImg.preserveAspect = true;
        }
        else
        {
            // Fallback to text title if sprite missing
            GameObject fallbackTitle = new GameObject("Text_Title_Fallback");
            fallbackTitle.transform.SetParent(logoObj.transform, false);
            var titleTxt = fallbackTitle.AddComponent<TextMeshProUGUI>();
            titleTxt.text = "AGRIQUEST AR";
            titleTxt.fontSize = 60;
            if (titleFont != null) titleTxt.font = titleFont;
            titleTxt.alignment = TextAlignmentOptions.Center;
            titleTxt.color = new Color(0.9f, 0.75f, 0.3f);
            RectTransform ftRect = fallbackTitle.GetComponent<RectTransform>();
            ftRect.anchorMin = Vector2.zero;
            ftRect.anchorMax = Vector2.one;
            ftRect.sizeDelta = Vector2.zero;
        }
        
        RectTransform logoRect = logoObj.GetComponent<RectTransform>();
        logoRect.anchorMin = new Vector2(0.08f, 0.80f);
        logoRect.anchorMax = new Vector2(0.92f, 0.92f);
        logoRect.sizeDelta = Vector2.zero;
        logoRect.anchoredPosition = Vector2.zero;

        // 8. Buat Viewport Container (Panel_Credits)
        GameObject panelObj = new GameObject("Panel_Credits");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0.04f, 0.08f, 0.05f, 0.88f); // High-contrast translucent dark forest card
        
        // Tambahkan Mask agar teks yang keluar dari panel ini terpotong (scrolling effect)
        panelObj.AddComponent<Mask>();
        
        // Outline Gold Premium
        Outline panelOutline = panelObj.AddComponent<Outline>();
        panelOutline.effectColor = new Color(0.9f, 0.75f, 0.3f, 0.35f);
        panelOutline.effectDistance = new Vector2(2, 2);

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.10f, 0.23f);
        panelRect.anchorMax = new Vector2(0.90f, 0.76f);
        panelRect.sizeDelta = Vector2.zero;
        panelRect.anchoredPosition = Vector2.zero;

        // 9. Buat Scrolling Text (Text_Credits) di dalam panel
        GameObject creditsTextObj = new GameObject("Text_Credits");
        creditsTextObj.transform.SetParent(panelObj.transform, false);
        TextMeshProUGUI creditsTxt = creditsTextObj.AddComponent<TextMeshProUGUI>();
        
        // Masukkan isi teks wajib berformat HTML premium lengkap dengan link
        string creditContent = "<align=center>\n" +
                               "<color=#D4AF37><b><size=40>DEVELOPMENT TEAM</size></b></color>\n" +
                               "<size=30><b>Hizkia Dustin Sitepu</b>\n" +
                               "<color=#B0C4DE>Lead Programmer & AR Specialist</color>\n\n" +
                               "<b>Muhammad Zharan Rabbani</b>\n" +
                               "<color=#B0C4DE>Co-Programmer & Game Logic</color></size>\n\n" +
                               "<voffset=20><color=#D4AF37><b><size=40>3D MODEL ATTRIBUTIONS</size></b></color></voffset>\n" +
                               "<size=25>\n" +
                               "<b><link=\"https://skfb.ly/oD7WZ\"><color=#62B6CB>\"Peanut\"</color></link></b>\n" +
                               "by Gewoelbe3DScan is licensed under\n" +
                               "<link=\"http://creativecommons.org/licenses/by/4.0/\"><color=#A0E8AF>Creative Commons Attribution</color></link>\n\n" +
                               "<b><link=\"https://skfb.ly/pzFrP\"><color=#62B6CB>\"Singkong / Cassava\"</color></link></b>\n" +
                               "by Abim Project is licensed under\n" +
                               "<link=\"http://creativecommons.org/licenses/by/4.0/\"><color=#A0E8AF>Creative Commons Attribution</color></link>\n\n" +
                               "<b><link=\"https://skfb.ly/ptXrH\"><color=#62B6CB>\"Oil Fruit Palm\"</color></link></b>\n" +
                               "by Aziz.Sisi.Akafomo is licensed under\n" +
                               "<link=\"http://creativecommons.org/licenses/by/4.0/\"><color=#A0E8AF>Creative Commons Attribution</color></link>\n\n" +
                               "<b><link=\"https://skfb.ly/ot6Vu\"><color=#62B6CB>\"Sugar Cane\"</color></link></b>\n" +
                               "by RayznGames is licensed under\n" +
                               "<link=\"http://creativecommons.org/licenses/by/4.0/\"><color=#A0E8AF>Creative Commons Attribution</color></link>\n\n" +
                               "<b><link=\"https://skfb.ly/6CuwT\"><color=#62B6CB>\"Coffee Plant Sasha An\"</color></link></b>\n" +
                               "by xan1 is licensed under\n" +
                               "<link=\"http://creativecommons.org/licenses/by/4.0/\"><color=#A0E8AF>Creative Commons Attribution</color></link>\n\n" +
                               "<b><link=\"https://skfb.ly/6YMsB\"><color=#62B6CB>\"Cocoa Pod\"</color></link></b>\n" +
                               "by RISD Nature Lab is licensed under\n" +
                               "<link=\"http://creativecommons.org/licenses/by/4.0/\"><color=#A0E8AF>Creative Commons Attribution</color></link>\n\n" +
                               "<b><link=\"https://skfb.ly/oHTZV\"><color=#62B6CB>\"Turmeric\"</color></link></b>\n" +
                               "by anuch is licensed under\n" +
                               "<link=\"http://creativecommons.org/licenses/by/4.0/\"><color=#A0E8AF>Creative Commons Attribution</color></link>\n" +
                               "</size>\n\n" +
                               "<voffset=20><color=#D4AF37><b><size=40>POWERED BY</size></b></color></voffset>\n" +
                               "<size=30><b>Vuforia Engine</b>\n" +
                               "<color=#B0C4DE>AR Target Tracking & Image Detection</color>\n\n" +
                               "<b>Unity Engine</b>\n" +
                               "<color=#B0C4DE>v2022 / UI Canvas Systems</color></size>\n\n" +
                               "<voffset=30><color=#A0E8AF><i>Thank you for playing!</i></color></voffset>\n" +
                               "</align>";
        
        creditsTxt.text = creditContent;
        if (bodyFont != null) creditsTxt.font = bodyFont;
        creditsTxt.alignment = TextAlignmentOptions.Top;
        creditsTxt.color = Color.white;
        creditsTxt.raycastTarget = true; // Diaktifkan agar link bisa di-klik!

        RectTransform textRect = creditsTextObj.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.05f, 1f);
        textRect.anchorMax = new Vector2(0.95f, 1f);
        textRect.pivot = new Vector2(0.5f, 1f);
        textRect.sizeDelta = new Vector2(0, 1800f); // Make height large enough to avoid vertical clamping
        textRect.anchoredPosition = new Vector2(0, -50f);

        // Tambahkan script CreditsController untuk scroll berjalan otomatis & click handler
        var scroller = creditsTextObj.AddComponent<CreditsController>();
        scroller.viewport = panelRect;
        scroller.scrollSpeed = 38f;

        // 10. Buat Tombol Kembali (Btn_Kembali)
        GameObject btnObj = new GameObject("Button_Kembali");
        btnObj.transform.SetParent(canvasObj.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        if (btnSprite != null)
        {
            btnImg.sprite = btnSprite;
        }
        else
        {
            btnImg.color = new Color(0.12f, 0.35f, 0.12f);
        }
        
        Button btn = btnObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        cb.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        cb.selectedColor = Color.white;
        btn.colors = cb;

        // ButtonEffect premium
        ButtonEffect effect = btnObj.AddComponent<ButtonEffect>();
        effect.scaleMultiplier = 1.08f;
        effect.animationDuration = 0.08f;
        effect.selectedColor = new Color(0.9f, 0.75f, 0.3f, 1f);
        effect.useGlow = true;

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.15f, 0.09f);
        btnRect.anchorMax = new Vector2(0.85f, 0.17f);
        btnRect.sizeDelta = Vector2.zero;
        btnRect.anchoredPosition = Vector2.zero;

        // Teks di dalam tombol
        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI btnTxt = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnTxt.text = "KEMBALI KE MENU";
        btnTxt.fontSize = 28;
        btnTxt.fontStyle = FontStyles.Bold;
        if (bodyFont != null) btnTxt.font = bodyFont;
        btnTxt.alignment = TextAlignmentOptions.Center;
        btnTxt.color = Color.white;
        btnTxt.raycastTarget = false;
        
        RectTransform btnTxtRect = btnTextObj.GetComponent<RectTransform>();
        btnTxtRect.anchorMin = Vector2.zero;
        btnTxtRect.anchorMax = Vector2.one;
        btnTxtRect.sizeDelta = Vector2.zero;

        // Pasang Event OnClick
        UnityEventTools.AddPersistentListener(btn.onClick, loader.LoadHOMEPAGE);

        // 11. Save Scene
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");
            
        string scenePath = "Assets/Scenes/CreditScene.unity";
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

        Debug.Log("✅ SCENE CREDIT PORTRAIT PREMIUM BERHASIL DIBUAT!");
    }

    [MenuItem("Tools/Integrasikan Tombol Credit di Homepage")]
    public static void IntegrateCreditButtonOnHomepage()
    {
        Debug.Log("Mengintegrasikan tombol Credit di Homepage...");
        
        string homepagePath = "Assets/Scenes/HOMEPAGE.unity";
        var hpScene = EditorSceneManager.OpenScene(homepagePath, OpenSceneMode.Single);

        GameObject canvasObj = GameObject.Find("hompage");
        if (canvasObj == null)
        {
            Debug.LogError("Gagal menemukan Canvas dengan nama 'hompage' di scene HOMEPAGE!");
            return;
        }

        Transform existingBtn = canvasObj.transform.Find("Btn_Credit");
        if (existingBtn != null)
        {
            Debug.Log("Tombol Btn_Credit sudah ada, memperbarui posisinya...");
            DestroyImmediate(existingBtn.gameObject);
        }

        // Cari loader & button sprite di scene
        SceneLoader loader = FindObjectOfType<SceneLoader>();
        if (loader == null)
        {
            // Buat penampung baru jika tidak ditemukan
            GameObject slObj = GameObject.Find("SceneLoader");
            if (slObj == null) slObj = new GameObject("SceneLoader");
            loader = slObj.GetComponent<SceneLoader>();
            if (loader == null) loader = slObj.AddComponent<SceneLoader>();
        }

        Sprite btnSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Button/btn_gold_pattern.png");
        if (btnSprite == null)
        {
            string[] guids = AssetDatabase.FindAssets("btn_gold_pattern t:Sprite");
            if (guids.Length > 0)
                btnSprite = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        TMP_FontAsset bodyFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Fonts/LibreCaslonText-Bold SDF.asset");

        // Buat Tombol Credit
        GameObject btnObj = new GameObject("Btn_Credit");
        btnObj.transform.SetParent(canvasObj.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        if (btnSprite != null)
        {
            btnImg.sprite = btnSprite;
        }
        else
        {
            btnImg.color = new Color(0.12f, 0.35f, 0.12f);
        }

        Button btn = btnObj.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
        cb.pressedColor = new Color(0.7f, 0.7f, 0.7f);
        cb.selectedColor = Color.white;
        btn.colors = cb;

        // ButtonEffect premium
        ButtonEffect effect = btnObj.AddComponent<ButtonEffect>();
        effect.scaleMultiplier = 1.08f;
        effect.animationDuration = 0.08f;
        effect.selectedColor = new Color(0.9f, 0.75f, 0.3f, 1f);
        effect.useGlow = true;

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        // Tempatkan persis di bawah ARScanner & Instruction, di area vertical (0.08 - 0.23)
        btnRect.anchorMin = new Vector2(0.13f, 0.08f);
        btnRect.anchorMax = new Vector2(0.87f, 0.23f);
        btnRect.sizeDelta = Vector2.zero;
        btnRect.anchoredPosition = Vector2.zero;

        // Teks di dalam tombol
        GameObject btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI btnTxt = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnTxt.text = "CREDIT";
        btnTxt.fontSize = 28;
        btnTxt.fontStyle = FontStyles.Bold;
        if (bodyFont != null) btnTxt.font = bodyFont;
        btnTxt.alignment = TextAlignmentOptions.Center;
        btnTxt.color = Color.white;
        btnTxt.raycastTarget = false;

        RectTransform btnTxtRect = btnTextObj.GetComponent<RectTransform>();
        btnTxtRect.anchorMin = Vector2.zero;
        btnTxtRect.anchorMax = Vector2.one;
        btnTxtRect.sizeDelta = Vector2.zero;

        // Hubungkan klik tombol ke LoadCreditScene
        UnityEventTools.AddPersistentListener(btn.onClick, loader.LoadCreditScene);

        // Simpan scene
        EditorSceneManager.SaveScene(hpScene);
        Debug.Log("✅ TOMBOL CREDIT BERHASIL DIINTEGRASIKAN DI HOMEPAGE!");
    }
}
