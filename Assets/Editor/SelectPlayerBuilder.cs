using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class SelectPlayerBuilder : EditorWindow
{
    [MenuItem("Tools/Build Select Player Screen")]
    public static void BuildUI()
    {
        // 1. Ensure Assets are Sprites
        SetAsSprite("Assets/Background/Background_SelectPlayer.png");
        SetAsSprite("Assets/Button/Button_StartQuest.png");
        SetAsSprite("Assets/Button/Panel_Codex_Grid.png");
        SetAsSprite("Assets/Icon/Icon_Castle.png");
        SetAsSprite("Assets/Icon/Icon_ARScanner_Active.png");
        SetAsSprite("Assets/Icon/Icon_Codex.png");
        SetAsSprite("Assets/Icon/Icon_Settings.png");

        AssetDatabase.Refresh();

        // 2. Setup Canvas
        GameObject canvasGo = GameObject.Find("Canvas_SelectPlayer");
        if (canvasGo == null)
        {
            canvasGo = new GameObject("Canvas_SelectPlayer");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();
        }

        // 3. Create Background
        CreateImage(canvasGo.transform, "Background", "Assets/Background/Background_SelectPlayer.png", new Vector2(0, 0), new Vector2(1440, 1024), true);

        // 4. Create Codex Panel (Grid)
        CreateImage(canvasGo.transform, "CodexPanel", "Assets/Button/Panel_Codex_Grid.png", new Vector2(0, 100), new Vector2(600, 600));

        // 5. Create Start Quest Button
        CreateButton(canvasGo.transform, "StartQuestButton", "Assets/Button/Button_StartQuest.png", new Vector2(0, -300), new Vector2(400, 100), true);

        // 6. Create Bottom Navigation Bar
        GameObject navBar = new GameObject("BottomNavBar");
        navBar.transform.SetParent(canvasGo.transform, false);
        RectTransform navRt = navBar.AddComponent<RectTransform>();
        navRt.anchoredPosition = new Vector2(0, -450);
        navRt.sizeDelta = new Vector2(800, 100);

        // Add Nav Icons
        CreateImage(navBar.transform, "Icon_Castle", "Assets/Icon/Icon_Castle.png", new Vector2(-200, 0), new Vector2(60, 60));
        CreateImage(navBar.transform, "Icon_ARScanner", "Assets/Icon/Icon_ARScanner_Active.png", new Vector2(-70, 0), new Vector2(80, 80));
        CreateImage(navBar.transform, "Icon_Codex", "Assets/Icon/Icon_Codex.png", new Vector2(70, 0), new Vector2(60, 60));
        CreateImage(navBar.transform, "Icon_Settings", "Assets/Icon/Icon_Settings.png", new Vector2(200, 0), new Vector2(60, 60));

        Debug.Log("Select Player UI Build Complete!");
    }

    private static void SetAsSprite(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null && importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.SaveAndReimport();
        }
    }

    private static GameObject CreateImage(Transform parent, string name, string assetPath, Vector2 pos, Vector2 size, bool isBg = false)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        Image img = go.AddComponent<Image>();
        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (s != null) img.sprite = s;
        img.color = Color.white;
        
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        if (isBg)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            go.transform.SetAsFirstSibling();
        }

        return go;
    }

    private static GameObject CreateButton(Transform parent, string name, string assetPath, Vector2 pos, Vector2 size, bool addEffect = false)
    {
        GameObject go = CreateImage(parent, name, assetPath, pos, size);
        go.AddComponent<Button>();
        
        if (addEffect)
        {
            // Adding our custom effect script
            go.AddComponent<ButtonEffect>();
        }
        
        return go;
    }
}
